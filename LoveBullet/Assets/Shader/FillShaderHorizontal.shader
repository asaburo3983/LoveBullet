Shader "Hidden/SpriteRendererFillAmountHorizontal"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _FillAmount("FillAmount", Range(0, 1)) = 0
        [MaterialToggle]
        _Reverse("Reverse", float) = 0
    }

        HLSLINCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            ENDHLSL

            SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            Pass
            {
                Tags { "LightMode" = "Universal2D" }
                HLSLPROGRAM
                #pragma vertex CombinedShapeLightVertex
                #pragma fragment CombinedShapeLightFragment
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

                struct Attributes
                {
                    float3 positionOS   : POSITION;
                    float4 color        : COLOR;
                    float2  uv           : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4  positionCS  : SV_POSITION;
                    half4   color       : COLOR;
                    float2	uv          : TEXCOORD0;
                    half2	lightingUV  : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_MaskTex);
                SAMPLER(sampler_MaskTex);
                TEXTURE2D(_NormalMap);
                SAMPLER(sampler_NormalMap);
                half4 _MainTex_ST;
                half4 _NormalMap_ST;
                float _FillAmount;
                float _Reverse;

                #if USE_SHAPE_LIGHT_TYPE_0
                SHAPE_LIGHT(0)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_1
                SHAPE_LIGHT(1)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_2
                SHAPE_LIGHT(2)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_3
                SHAPE_LIGHT(3)
                #endif

                Varyings CombinedShapeLightVertex(Attributes v)
                {
                    Varyings o = (Varyings)0;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.positionCS = TransformObjectToHClip(v.positionOS);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    float4 clipVertex = o.positionCS / o.positionCS.w;
                    o.lightingUV = ComputeScreenPos(clipVertex).xy;
                    o.color = v.color;
                    return o;
                }

                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

                half4 CombinedShapeLightFragment(Varyings i) : SV_Target
                {
                    half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                    half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                    clip(
                        lerp(
                            -i.uv.x + _FillAmount,
                            i.uv.x - (1 - _FillAmount),
                            _Reverse
                        )
                    );

                    return CombinedShapeLightShared(main, mask, i.lightingUV);
                }
                ENDHLSL
            }
        }

            Fallback "Sprites/Default"
}