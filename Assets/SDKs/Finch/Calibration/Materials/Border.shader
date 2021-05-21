Shader "Border"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_BorderColor("Border Color", Color) = (1,1,1,1)
		_ButtonColor("Button Color", Color) = (0,0,0,0)
		_RadiusAlpha("Alpha Radius", Range(0,0.5)) = 1
		_BorderWidth("Border width", Range(0,1)) = 0.1
		_RatioX("Texture Ratio X",Range(0,1)) = 1
		_RatioY("Texture Ratio Y",Range(0,1)) = 1

		_LeftUpCorner("Corner left up",Range(0,1)) = 1
		_RightUpCorner("Corner right up",Range(0,1)) = 1
		_LeftDownCorner("Corner left down",Range(0,1)) = 1
		_RightDownCorner("Corner right down",Range(0,1)) = 1
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

				bool OutCircle(float x, float y, float radius, float offset);

				float4 _BorderColor;
				float4 _ButtonColor;
				float4 _MainTex_ST;
				float _RadiusAlpha;
				float _BorderWidth;
				float _BaseColor;
				float _RatioX;
				float _RatioY;

				float _LeftUpCorner;
				float _RightUpCorner;
				float _LeftDownCorner;
				float _RightDownCorner;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = v.uv;
					_BaseColor = v.color;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float x = i.uv.x;
					float y = i.uv.y;
					
					if (x > 0.5 && y > 0.5 && _LeftUpCorner < 0.5)
					{
						if (x > 1 - _BorderWidth * _RatioY * 0.5f || y > 1 - _BorderWidth * _RatioX* 0.5f)
						{
							return _BorderColor;
						}

						return i.color;
					}

					if (x > 0.5 && y < 0.5 && _LeftDownCorner < 0.5)
					{
						if (x > 1 - _BorderWidth * _RatioY * 0.5f || y < _BorderWidth * _RatioX * 0.5f)
						{
							return _BorderColor;
						}

						return i.color;
					}

					if (x < 0.5 && y > 0.5 && _RightUpCorner < 0.5)
					{
						if (x < _BorderWidth * _RatioY * 0.5f || y > 1 - _BorderWidth * _RatioX* 0.5f)
						{
							return _BorderColor;
						}

						return i.color;
					}

					if (x < 0.5 && y < 0.5 && _RightDownCorner < 0.5)
					{
						if (x < _BorderWidth * _RatioY * 0.5f || y < _BorderWidth * _RatioX* 0.5f)
						{
							return _BorderColor;
						}

						return i.color;
					}

					bool inButton = !OutCircle(x, y, _RadiusAlpha, _BorderWidth);

					if (inButton)
					{
						return i.color;
					}
					else if (!OutCircle(x, y, _RadiusAlpha, 0))
					{
						return _BorderColor;
					}

					return float4 (0, 0, 0, 0);
				}

				bool OutCircle(float x, float y, float radius, float offset)
				{
					x = (x - offset * 0.5 * _RatioY) / (1 - offset * _RatioY);
					y = (y - offset * 0.5 * _RatioX) / (1 - offset * _RatioX);
					bool outSide = x < 0 || y < 0 || x > 1 || y > 1;

					bool xCorner = x < radius * _RatioY || x > 1 - radius * _RatioY;
					bool yCorner = y < radius * _RatioX || y > 1 - radius * _RatioX;
					float xPos = x < radius * _RatioY ? (x / _RatioY - radius) : ((x - 1) / _RatioY + radius);
					float yPos = y < radius * _RatioX ? (y / _RatioX - radius) : ((y - 1) / _RatioX + radius);

					return xCorner && yCorner &&  xPos * xPos + yPos * yPos > radius * radius || outSide;
				}

				ENDCG
			}
		}
}
