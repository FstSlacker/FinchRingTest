Shader "Finch/Avatar1"
{
    Properties
    {
        _ColorMain("Color Main", Color) = (1,1,1,1)
        _ColorAmbeintLight("Color Ambient Light", Color) = (1,1,1,1)
        _ColorLight("Color Light", Color) = (1,1,1,1)
        _ColorLightSecondary("Color Light Secondary", Color) = (1,1,1,1)
        _ColorFresnel0   ("Fresnel Color 0", Color) = (1,1,1,1)
        _ColorFresnel1   ("Fresnel Color 1", Color) = (1,1,1,1)
        

        _PowerFresnel0  ("Freshel Power 0", Float) = 2.0
        _PowerFresnel1  ("Freshel Power 1", Float) = 7.0


        _Roughness("Roughness", Float) = 0.25

       // _LightDirection ("Light Direction", Vector) = (0.5, -1, 0,0)



        _MainTex("Main Tex", 2D) = "white" {}
        _ClippingMaskTex0("Clipping Mask Tex0", 2D) = "white" {}
        _ClippingMaskTex1("Clipping Mask Tex1", 2D) = "white" {}
        _ClippingMaskIndex("Clipping Mask Index", Float) = 1.0
        _ClippingMask("Clipping Mask", Float) = 1.0

        _Dissolve("Dissolve", Float) = 1.0
        _Alpha("Alpha", Float) = 1.0
        _Fade("Fade", Float) = 1.0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" }
            //Tags { "Queue" = "Geometry" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
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
                    float3 color : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                };

                float4 _ColorMain;
                float4 _ColorLight;
                float4 _ColorLightSecondary;
                float4 _ColorAmbeintLight;
                float4 _ColorFresnel0;
                float4 _ColorFresnel1;

                float _PowerFresnel0;
                float _PowerFresnel1;

                //float4 _LightDirection;

                
                float _Roughness;

                float _Dissolve;
                float _Alpha;
                float _Fade;

                sampler2D _ClippingMaskTex0;
                sampler2D _ClippingMaskTex1;
                sampler2D _MainTex;
                float _ClippingMaskIndex;
                float _ClippingMask;



                float3 g_vAvatarLightDirection;
                float3 g_vAvatarLightDirectionSecondary;



                float2 LightingFuncGGX_FV(float dotLH, float roughness)
                {
                    float alpha = roughness * roughness;

                    // F
                    float F_a, F_b;
                    float dotLH5 = pow(1.0f - dotLH, 5);
                    F_a = 1.0f;
                    F_b = dotLH5;

                    // V
                    float vis;
                    float k = alpha / 2.0f;
                    float k2 = k * k;
                    float invK2 = 1.0f - k2;
                    vis = rcp(dotLH * dotLH * invK2 + k2);

                    return float2(F_a * vis, F_b * vis);
                }

                float LightingFuncGGX_D(float dotNH, float roughness)
                {
                    float alpha = roughness * roughness;
                    float alphaSqr = alpha * alpha;
                    float pi = 3.14159f;
                    float denom = dotNH * dotNH * (alphaSqr - 1.0) + 1.0f;

                    float D = alphaSqr / (pi * denom * denom);
                    return D;
                }

                float LightingFuncGGX_OPT3(float3 N, float3 V, float3 L, float roughness, float F0)
                {
                    float3 H = normalize(V + L);

                    float dotNL = saturate(dot(N, L));
                    float dotLH = saturate(dot(L, H));
                    float dotNH = saturate(dot(N, H));

                    float D = LightingFuncGGX_D(dotNH, roughness);
                    float2 FV_helper = LightingFuncGGX_FV(dotLH, roughness);
                    float FV = F0 * FV_helper.x + (1.0f - F0) * FV_helper.y;
                    float specular = dotNL * D * FV;

                    return specular;
                }



                float freshnel(float3 N, float3 V)
                {
                    float VdotN = dot(N, V);
                    float k = saturate(1 - VdotN);
                    return k;
                }


                // [Burley 2012, "Physically-Based Shading at Disney"]
                float Diffuse_Burley(float roughness, float NoV, float NoL, float NoH)
                {
                    float alpha = roughness * roughness;
                    float a2 = alpha * alpha;

                    float d = (NoH * a2 - NoH) * NoH + 1;	// 2 mad
                    return a2 / (3.1415 * d * d);				// 4 mul, 1 rcp
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


                    //float F = max(0, freshnel(N, V));
                    //F *= max(0, 1.0 - 0.5 * vert_colors.b);


                    float3 C = 0;

                    float F0 = 0;
                    float roughness = _Roughness;
                    float3 color = pow(_ColorMain,2.2);
                    float3 L = normalize(-g_vAvatarLightDirection);
                    float3 L2 = normalize(-g_vAvatarLightDirectionSecondary);
                    
                    // Light
                    float LdotN = max(0, dot(N, L));

                    float VdotN = max(0, dot(N, V));


                    float3 H1= normalize(V + L);
                    float3 H2 = normalize(V + L2);

                    float NdotH = max(0, dot(N, H1));
                    float NdotH2 = max(0, dot(N, H2));
                    float LdotN2 = max(0, dot(N, L2));

                    
                    float brdf_specular = LightingFuncGGX_OPT3(N, V, L, roughness, F0);
                    float brdf_specular2 = LightingFuncGGX_OPT3(N, V, L2, roughness, F0);


                    float IntensityScale = 5;
                    
                    float SpecScale = 4;

                    C += _ColorAmbeintLight.rgb * _ColorAmbeintLight.a  * color;


                    // primary

                    float diffuse0 = Diffuse_Burley(roughness, VdotN, LdotN, NdotH);
                    float diffuse1 = Diffuse_Burley(roughness, VdotN, LdotN2, NdotH2);
                    

                    C += _ColorLight.rgb * _ColorLight.a * IntensityScale * color * LdotN * diffuse0;
                    C += _ColorLight.rgb * _ColorLight.a * IntensityScale * color * brdf_specular* SpecScale;

                    // secondary
                    C += 0.4*_ColorLightSecondary.rgb * _ColorLightSecondary.a * IntensityScale * color * LdotN2 * diffuse1;
                    C += 2*_ColorLightSecondary.rgb * _ColorLightSecondary.a * IntensityScale * color * brdf_specular2 * SpecScale;

                    

                     // Freshnel
                    float F = max(0, freshnel(N, V));

                    C = C * (1 +  4*pow(F, _PowerFresnel0) * _ColorFresnel0.a * _ColorFresnel0.rgb );
                    C += pow(F, _PowerFresnel1) * _ColorFresnel1.a * _ColorFresnel1.rgb;



                    C = pow(C*4.0,1.2);

                    o.color = float4(C, 1);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float2  uv = i.uv;
                    float3 c = i.color.rgb;

                    float4 MT = tex2D(_MainTex, i.uv);

                    float4 CM0 = tex2D(_ClippingMaskTex0, i.uv);
                    float4 CM1 = tex2D(_ClippingMaskTex1, i.uv);


                    int IDX = _ClippingMaskIndex;
                    IDX = clamp(IDX, 0, 7);
                    float CM = IDX < 4 ? CM0[IDX] : CM1[IDX - 4];
                    CM = lerp(1, CM, _ClippingMask);

                    /*CM = pow(CM, 4);*/

                    float alpha = CM * _Dissolve * _Alpha * MT.b * _Fade;

                    c *= alpha;

                    if (alpha < 0.001)
                        discard;

                    return float4(c, 1);
                }
                ENDCG
            }
        }
}