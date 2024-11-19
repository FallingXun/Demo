Shader "Unlit/TestShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"IgnoreProjector" = "True"
		}

        Pass
        {
			Cull Off
			ZTest Off
			ZWrite Off
			ZClip Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float4 tangent : TANGENT;
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
				float2 uv5 : TEXCOORD5;
				float2 uv6 : TEXCOORD6;
				float2 uv7 : TEXCOORD7;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 color : TEXCOORD0;
            };


            v2f vert (appdata v)
            {
                v2f o;
				float s = 1;
				o.vertex = float4(0, 0, 0, 0);
				o.vertex = UnityObjectToClipPos(v.vertex);
				s *= step(v.vertex.x, 0);
				s *= step(v.normal.x, 0);
				s *= step(v.tangent.x, 0);
				s *= step(v.color.x, 0);
				s *= step(v.uv0.x, 0);
				s *= step(v.uv1.y, 0);
				s *= step(v.uv2.y, 0);
				s *= step(v.uv3.y, 0);
				s *= step(v.uv4.y, 0);
				s *= step(v.uv5.y, 0);
				s *= step(v.uv6.y, 0);
				s *= step(v.uv7.y, 0);
				o.color = float4(0.1, 0.1, 0.1, 1) * s;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1,1,1,1);
                return col;
            }
            ENDCG
        }
    }
}
