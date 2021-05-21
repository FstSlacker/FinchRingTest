Shader "BackGround"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RadiusAlpha("Alpha Radius", Range(0,0.5)) = 1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 100
		Cull Off

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _RadiusAlpha;
			float _BaseColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				_BaseColor = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				bool hide = false;
			    
				if (i.uv.x < _RadiusAlpha && i.uv.y < _RadiusAlpha)
				{
					float x = i.uv.x - _RadiusAlpha;
					float y = i.uv.y - _RadiusAlpha;
					hide = x * x + y * y > _RadiusAlpha * _RadiusAlpha;
				}
				
				if (i.uv.x < _RadiusAlpha && i.uv.y > 1 - _RadiusAlpha)
				{
					float x = i.uv.x - _RadiusAlpha;
					float y = i.uv.y - 1 + _RadiusAlpha;
					hide = x * x + y * y > _RadiusAlpha * _RadiusAlpha;
				}

				if (i.uv.x > 1 - _RadiusAlpha && i.uv.y > 1 - _RadiusAlpha)
				{
					float x = i.uv.x - 1 + _RadiusAlpha;
					float y = i.uv.y - 1 + _RadiusAlpha;
					hide = x * x + y * y > _RadiusAlpha * _RadiusAlpha;
				}

				if (i.uv.x > 1 - _RadiusAlpha && i.uv.y < _RadiusAlpha)
				{
					float x = i.uv.x - 1 + _RadiusAlpha;
					float y = i.uv.y - _RadiusAlpha;
					hide = x * x + y * y > _RadiusAlpha * _RadiusAlpha;
				}

				if (hide)
				{
					return float4 (0, 0, 0, 0);
				}
				else
				{
					return tex2D(_MainTex, i.uv) * i.color;
				}
			}

			ENDCG
		}


	}
}
