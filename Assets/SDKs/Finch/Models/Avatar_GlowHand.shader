Shader "Finch/Avatar_GlowHand"
{
    Properties
    {
        _ColorAmbeintLight("Color Ambient Light", Color) = (1,1,1,1)
        
        _ColorFresnel   ("Fresnel Color", Color) = (1,1,1,1)
        _PowerFresnel  ("Freshel Power", Float) = 3.0
        
        _ColorWaveMinColor   ("Wave 0 Color", Color) = (1,1,1,1)
        _ColorWaveMaxColor   ("Wave 1 Color", Color) = (1,1,1,1)
        
        
        _PowerWaveFresnel  ("Wave Freshel Power", Float) = 3.0
        _AddWaveFresnel  ("Wave Ambient", Float) = 0.5


        _WaveSpeed("Wave Speed", Float) = 0.25
        _WaveScale("Wave Scale", Float) = 2.0
        _WavePower("Wave Power", Float) = 2.0
        
        _WaveSpeed_2("Negative Wave Speed", Float) = 0.25
        _WaveScale_2("Negative Wave Scale", Float) = 4.0
        _WavePower_2("Negative Wave Power", Float) = 2.0

        _SecondaryWavesAmount("(2nd wave) intensity", Float) = 1.0

        _MaskTex("Mask Texture RGB(Mask, 2nd wave, Linear grad)", 2D) = "white" {}

        /*_MaskTex_LinearGrad("Linear grad", 2D) = "white" {}*/


        _Visability("Visability", Float) = 1.0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" }
            
            //Tags { "Queue" = "Geometry" }
            //Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
            //Blend One OneMinusSrcAlpha // Premultiplied transparency
            //Blend One One // Additive
            Blend OneMinusDstColor One // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor SrcColor // 2x Multiplicative

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
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;

                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 color : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    float NoV : TEXCOORD2;
                };

                float4 _ColorAmbeintLight;
                float4 _ColorFresnel;
                float _PowerFresnel;

                float _PowerWaveFresnel;
                float _AddWaveFresnel;

                float4 _ColorWaveMinColor;
                float4 _ColorWaveMaxColor;

                float _WaveSpeed;
                float _WaveScale;
                float _WavePower;
                
                float _WaveSpeed_2;
                float _WaveScale_2;
                float _WavePower_2;

                float _SecondaryWavesAmount;
                float _Visability;


                sampler2D _MaskTex;
                sampler2D _MaskTex_LinearGrad;


                float freshnel(float VdotN)
                {
                    float k = saturate(1 - VdotN);
                    return k;
                }
                

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;

                    float3 wpos = mul(UNITY_MATRIX_M, v.vertex);
                    float3 V = normalize(_WorldSpaceCameraPos - wpos);

                    float3 N = mul(unity_ObjectToWorld, v.normal);
                    N = normalize(N);

                     // Freshnel
                    float NoV = dot(N, V);
                    float F = max(0, freshnel(NoV));

                    float3 C    = _ColorAmbeintLight 
                                +  pow(F, _PowerFresnel ) * _ColorFresnel.rgb * _ColorFresnel.a * 10;
                    
                    float WF =  pow(F, _PowerWaveFresnel ) * 2 + _AddWaveFresnel;

                    o.NoV = NoV;
                    o.color = float4(C, WF);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    if(i.NoV < 0)
                        discard;

                    float2  v = i.uv;
                    float3 C = i.color.rgb;

                    float4 MT = tex2D(_MaskTex, i.uv); // RGB, Mask, 2nd wave, Linear grad
                    
                    /*MT.b = tex2D(_MaskTex_LinearGrad, i.uv).r;*/
                    
                    float Mask          = MT.r;
                    float SecondWave    = MT.g * _SecondaryWavesAmount;
                    float LinearGrad    = MT.b;



                    float2 Offset = float2(_WaveSpeed, _WaveSpeed_2) * _Time.y;

                    float Wave          = sin(LinearGrad * _WaveScale   + Offset.x);
                    float WaveNegative  = sin(LinearGrad * _WaveScale_2 + Offset.y);
                    WaveNegative = saturate(WaveNegative * 0.5 + 0.5);
                    WaveNegative = pow(WaveNegative, _WavePower_2);

                    Wave = saturate(Wave);
                    Wave = pow(Wave, _WavePower);

                    Wave *= WaveNegative * i.color.a * (1 + SecondWave);

                    C += lerp(_ColorWaveMinColor.rgb*_ColorWaveMinColor.a, _ColorWaveMaxColor.rgb * _ColorWaveMaxColor.a, Wave) * Wave;


                    C *= Mask;

                    return float4(C, 1);
                }
                ENDCG
            }
        }
}