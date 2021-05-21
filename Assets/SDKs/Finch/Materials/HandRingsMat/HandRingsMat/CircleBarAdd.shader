// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Finch/circleBarAdd"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1,1,1,1)
        //_GradientAnimationSpeed("Gradient Animation Speed", Float) = 1.0
        _Power("Power", Float) = 1.0

        _Dissolve("Dissolve", Float) = 1.0
        _Rotation("Rotation", Float) = 0.0

        /*g_vOrigin("Origin", Vector) = (0, 0, 0, 0)*/

        /*_ColorAddR("Color Add R", Color) = (1,1,1,1)
        _ColorAddG("Color Add G", Color) = (1,1,1,1)
        _ColorAddB("Color Add B", Color) = (1,1,1,1)*/
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
            //Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
            Blend OneMinusDstColor One
            Cull Off
            ZWrite Off
            ZTest LEqual

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                   // float4 texcoord : TEXCOORD1;
                    float2 uv : TEXCOORD0;

                };

                struct v2f
                {
                    float2 uv    : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                float4 _Tint;
                float _Dissolve;
                float _Power;
                float _Rotation;
                float _OffsetX;
                float _OffsetY;
                

                float3 g_vOrigin;
                fixed Alpha;


                v2f vert(appdata v)
                {
                    float s = sin(_Rotation * _Time.x);
                    float c = cos(_Rotation * _Time.x);
                    float2x2 rotationMatrix = float2x2(c, -s, s, c);

                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    float offsetX = .5; //_MainTex_ST.z +_MainTex_ST.x / 2;
                    float offsetY = .5; //_MainTex_ST.w +_MainTex_ST.y / 2;

                    float x = v.uv.x - offsetX; //* _MainTex_ST.x + _MainTex_ST.z - offsetX;
                    float y = v.uv.y - offsetY; //* _MainTex_ST.y + _MainTex_ST.w - offsetY;

                    o.uv = mul(float2(x, y), rotationMatrix) + float2(offsetX, offsetY);

                    return o;
                }

                /*float freshnel(float3 N, float3 V)
                {
                    float VdotN = dot(N, V);
                    float k = saturate(1 - VdotN);
                    return pow(k, 3);
                }*/

                float4 frag(v2f i) : SV_Target
                {
                   /* float2 offset = float2(_Time.x * _UVAnimationSpeed, 0);*/
                    float2  uv = i.uv;


                    float4 tex = tex2D(_MainTex, uv).r;
                    float3 C = tex * _Tint*2.5;
                   /* float4 mask = tex2D(_MainTex, uv);*/

                   /* C *= i.color.a;*/

                   /* float4 _colorAdd = lerp(_ColorAddR, _ColorAddG, mask.g);
                    _colorAdd = lerp(_colorAdd,  _ColorAddB, mask.g);*/
                    //_colorAdd = lerp(_colorAdd,  _ColorAddB, mask.b);
                   /* float4 Flicker = abs((tex2D(_MainTex, uv).a - 0.5) * 2 * sin(((_Time.x * _GradientAnimationSpeed + (tex2D(_MainTex, uv).a + 0.25)) * 4) + uv.g));*/



                    /*C *= Flicker;*/
                    //C *= saturate(tex2D(_MainTex, uv).r - Flicker.a);

                    //C *= _colorAdd;
                    C *= _Power;
                    C *= _Dissolve;

                    return float4(C, 1);
                }
                ENDCG
            }
        }
}
