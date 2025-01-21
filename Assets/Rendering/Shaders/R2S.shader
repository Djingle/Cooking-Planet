Shader "Custom/R2S"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off       // Do not write to the depth buffer
        LOD 100
        Pass
        {
            Name "R2SPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #pragma vertex Vert
            #pragma fragment frag


            TEXTURE2D(_CameraOpaqueTexture);
            SamplerState sampler_point_repeat;

            half4 frag(Varyings i) : SV_Target
            {
                //return tex2D(_MainTex, i.pos.xy); // Direct texture output
                return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_point_repeat, i.texcoord);
                //return float4(0,1,0,1); // Supposed to render everything green
            }
            ENDHLSL
        }
    }
}
