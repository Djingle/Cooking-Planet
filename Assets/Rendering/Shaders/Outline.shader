Shader "Hidden/Outline"
{
    Properties
    {
        _OutlineThickness("Outline Thickness", Float) = 1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _DepthThreshold("Depth Threshold", Float) = .005
        _NormalThreshold("Normal Threshold", Float) = .25
        _LuminanceThreshold("Luminance Threshold", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "OutlinePass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            float _OutlineThickness;
            float4 _OutlineColor;
            float _DepthThreshold;
            float _NormalThreshold;
            float _LuminanceThreshold;

            #pragma vertex Vert
            #pragma fragment frag

            float RobertsCross(float3 samples[4]) {
                const float3 difference1 = samples[1] - samples[2];
                const float3 difference2 = samples[0] - samples[3];
                return sqrt(dot(difference1, difference1) + dot(difference2, difference2));
            }

            float RobertsCross(float samples[4]) {
                const float difference1 = samples[1] - samples[2];
                const float difference2 = samples[0] - samples[3];
                return sqrt(difference1 * difference1 + difference2 * difference2);
            }

            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SampleSceneColor(uv);
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                const float half_width_f = floor(_OutlineThickness * 0.5);
                const float half_width_c = ceil(_OutlineThickness * 0.5);

                float2 neighbours[4];
                neighbours[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);  // top left
                neighbours[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);   // top right
                neighbours[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1); // bottom left
                neighbours[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);  // bottom right

                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];

                for (int i=0; i<4; i++) {
                    depth_samples[i] = SampleSceneDepth(neighbours[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(neighbours[i]);
                    luminance_samples[i] = SampleSceneLuminance(neighbours[i]);
                }

                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);

                edge_depth = edge_depth > _DepthThreshold ? 1 : 0;
                edge_normal = edge_normal > _NormalThreshold ? 1 : 0;
                edge_luminance = edge_luminance > _LuminanceThreshold ? 1 : 0;

                float edge = max(edge_depth, max(edge_normal, edge_luminance));
                float3 color = lerp(SampleSceneColor(uv), _OutlineColor, edge);
                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
