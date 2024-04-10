Shader "Unlit/GPUSkinning"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

		Pass
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
			#include "GPUSkinningBase.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 skin : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				#if defined(UNITY_INSTANCING_ENABLED)
					v.vertex = Skinning(v);
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);

				fixed4 col = tex2D(_MainTex, i.uv);
				col = col * _Color;
				return col;
			}
			ENDCG
        }
		Pass
		{
			Name "ShadowCaster"

			Tags {"LightMode" = "ShadowCaster"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_instancing
			#pragma multi_compile_shadowcaster

			#include "UnityCG.cginc"
			#include "GPUSkinningBase.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				#if defined(UNITY_INSTANCING_ENABLED)
					v.vertex = SkinningShadow(v);
				#endif
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
    }
}
