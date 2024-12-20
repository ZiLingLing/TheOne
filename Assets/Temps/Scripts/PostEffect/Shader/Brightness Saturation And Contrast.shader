Shader "URP/Brightness Saturation And Contrast"
{
    Properties
    {
        // 基础纹理
        _MainTex ("Base (RGB)", 2D) = "white" { }
        // 亮度
        _Brightness ("Brightness", Float) = 1
        // 饱和度
        _Saturation ("Saturation", Float) = 1
        // 对比度
        _Contrast ("Contrast", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half _Brightness;
        half _Saturation;
        half _Contrast;
        CBUFFER_END
        
        ENDHLSL
        
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            struct a2v
            {
                float4 vertex: POSITION;
                float4 texcoord: TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos: SV_POSITION;
                half2 uv: TEXCOORD0;
            };
            
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                return o;
            }
            
            half4 frag(v2f i): SV_Target
            {
                half4 renderTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                half3 finalColor = renderTex.rgb * _Brightness;
                
                half luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                half3 luminanceColor = half3(luminance, luminance, luminance);

                finalColor = lerp(luminanceColor, finalColor, _Saturation);
                
                half3 avgColor = half3(0.5, 0.5, 0.5);
                finalColor = lerp(avgColor, finalColor, _Contrast);
                
                return half4(finalColor, renderTex.a);
            }            
            ENDHLSL            
        }
    }
    
    Fallback Off
}
