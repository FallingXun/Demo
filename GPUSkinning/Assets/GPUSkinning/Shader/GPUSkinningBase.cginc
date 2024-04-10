// 定义插值宏，如果不需要插值则注释
#ifndef GPUSKINNING_LERP
	#define GPUSKINNING_LERP
#endif


UNITY_INSTANCING_BUFFER_START(Props)
	UNITY_DEFINE_INSTANCED_PROP(float, _PlaySpeed)
	UNITY_DEFINE_INSTANCED_PROP(float, _StartTime)
	UNITY_DEFINE_INSTANCED_PROP(float, _StopTime)
	UNITY_DEFINE_INSTANCED_PROP(int, _Start)
	UNITY_DEFINE_INSTANCED_PROP(int, _Count)
	UNITY_DEFINE_INSTANCED_PROP(int, _Loop)
UNITY_INSTANCING_BUFFER_END(Props)

struct appdata
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 uv : TEXCOORD0;
	float4 uv2 : TEXCOORD1;
	float4 uv3 : TEXCOORD2;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

int _Width;
int _Height;
int _BoneCount;
int _SampleFPS;
sampler2D _AnimationTex;

half4x4 loadMatrix(uint frameIndex, uint boneIndex, float texelSizeX, float texelSizeY)
{
	uint index = frameIndex * _BoneCount + boneIndex;

	half x = index % _Width * texelSizeX;
	half y = index / _Width * 4 * texelSizeY;
	half4 uv = half4(x, y, 0, 0);
	half4 c0 = tex2Dlod(_AnimationTex, uv);
	uv.y = uv.y + texelSizeY;
	half4 c1 = tex2Dlod(_AnimationTex, uv);
	uv.y = uv.y + texelSizeY;
	half4 c2 = tex2Dlod(_AnimationTex, uv);

	half4 c3 = half4(0, 0, 0, 1);
	half4x4 m = half4x4(c0, c1, c2, c3);
	return m;
}

float4 Skinning(inout appdata i)
{

	// start 起始帧
	// count 动画帧数
	// loop 是否循环
	// startTime 动画开始时间
	int start = UNITY_ACCESS_INSTANCED_PROP(Props, _Start);
	int count = UNITY_ACCESS_INSTANCED_PROP(Props, _Count);
	int loop = UNITY_ACCESS_INSTANCED_PROP(Props, _Loop);
	float startTime = UNITY_ACCESS_INSTANCED_PROP(Props, _StartTime);
	float stopTime = UNITY_ACCESS_INSTANCED_PROP(Props, _StopTime);
	float playSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _PlaySpeed);

	float frameTime = ((_Time.y - startTime) * playSpeed + clamp(stopTime - startTime, 0, stopTime)) * _SampleFPS;
	int frame = floor(frameTime);

	float texelSizeX = 1.0f / (float)_Width;
	float texelSizeY = 1.0f / (float)_Height;

	// 动画不循环，播完则停在最后一帧
	int curFrame = lerp(clamp(frame, 0, count - 1), (frame % count) , loop) + start;
	half4x4 mat = (loadMatrix(curFrame, i.uv2.x, texelSizeX, texelSizeY) * i.uv2.y);
	mat += (loadMatrix(curFrame, i.uv2.z, texelSizeX, texelSizeY) * i.uv2.w);
	mat += (loadMatrix(curFrame, i.uv3.x, texelSizeX, texelSizeY) * i.uv3.y);
	mat += (loadMatrix(curFrame, i.uv3.z, texelSizeX, texelSizeY) * i.uv3.w);

	float4 vertex = mul(mat, i.vertex);
	float3 normal = mul((half3x3)mat, i.normal.xyz);
	float4 tangent = mul(mat, i.tangent);

#ifdef GPUSKINNING_LERP
	// 如果开启插值，则需要采样下一帧的状态，再进行插值
	frame = frame + 1;
	int nextFrame = lerp(clamp(frame, 0, count - 1), (frame % count), loop) + start;
	half4x4 mat2 = (loadMatrix(nextFrame, i.uv2.x, texelSizeX, texelSizeY) * i.uv2.y);
	mat2 += (loadMatrix(nextFrame, i.uv2.z, texelSizeX, texelSizeY) * i.uv2.w);
	mat2 += (loadMatrix(nextFrame, i.uv3.x, texelSizeX, texelSizeY) * i.uv3.y);
	mat2 += (loadMatrix(nextFrame, i.uv3.z, texelSizeX, texelSizeY) * i.uv3.w);
	float4 vertex2 = mul(mat2, i.vertex);
	float3 normal2 = mul((half3x3)mat2, i.normal.xyz);
	float4 tangent2 = mul(mat2, i.tangent);

	float l = frac(frameTime);
	i.vertex = lerp(vertex, vertex2, l);
	i.normal = normalize(lerp(normal, normal2, l));
	i.tangent = normalize(lerp(tangent, tangent2, l));
#else
	i.vertex = vertex;
	i.normal = normalize(normal);
	i.tangent = normalize(tangent);
#endif

	return  i.vertex;
}

float4 SkinningShadow(inout appdata i)
{

	// start 起始帧
	// count 动画帧数
	// loop 是否循环
	// startTime 动画开始时间
	int start = UNITY_ACCESS_INSTANCED_PROP(Props, _Start);
	int count = UNITY_ACCESS_INSTANCED_PROP(Props, _Count);
	int loop = UNITY_ACCESS_INSTANCED_PROP(Props, _Loop);
	float startTime = UNITY_ACCESS_INSTANCED_PROP(Props, _StartTime);
	float stopTime = UNITY_ACCESS_INSTANCED_PROP(Props, _StopTime);
	float playSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _PlaySpeed);

	float frameTime = ((_Time.y - startTime) * playSpeed + clamp(stopTime - startTime, 0, stopTime)) * _SampleFPS;
	int frame = floor(frameTime);

	float texelSizeX = 1.0f / (float)_Width;
	float texelSizeY = 1.0f / (float)_Height;

	// 动画不循环，播完则停在最后一帧
	int curFrame = lerp(clamp(frame, 0, count - 1), (frame % count), loop) + start;
	half4x4 mat = (loadMatrix(curFrame, i.uv2.x, texelSizeX, texelSizeY) * i.uv2.y);

	float4 vertex = mul(mat, i.vertex);

#ifdef GPUSKINNING_LERP
	// 如果开启插值，则需要采样下一帧的状态，再进行插值
	frame = frame + 1;
	int nextFrame = lerp(clamp(frame, 0, count - 1), (frame % count), loop) + start;
	half4x4 mat2 = (loadMatrix(nextFrame, i.uv2.x, texelSizeX, texelSizeY) * i.uv2.y);

	float4 vertex2 = mul(mat2, i.vertex);

	float l = frac(frameTime);
	i.vertex = lerp(vertex, vertex2, l);
#else
	i.vertex = vertex;
#endif

	return  i.vertex;
}