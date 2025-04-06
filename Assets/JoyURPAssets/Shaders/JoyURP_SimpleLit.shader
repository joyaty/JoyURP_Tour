// 基于Blinn-Phong光照模型渲染的场景对象
Shader "JoyURP/JoyURP_SimpleLit"
{
    Properties
    {
        [MainTexture] _DiffuseMap ("Diffuse Map", 2D) = "white" { } // 漫反射贴图
        [MainColor] _DiffuseColor ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0) // 漫反射颜色
        _BumpMap ("Normal Map", 2D) = "bump" {} // 法线贴图
        _SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0) // 高光颜色
        _Glossy ("Glossy", Range(0, 1)) = 0.5 // 光滑度
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "JoyUniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            // 声明顶点着色器和像素着色器入库
            #pragma vertex VertexMain
            #pragma fragment FragmentMain

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // 声明漫反射纹理和采样器
            TEXTURE2D(_DiffuseMap);
            SAMPLER(sampler_DiffuseMap);
            // 声明法线纹理和采样器
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            // 声明材质相关的常量缓冲区
            CBUFFER_START(UnityPerMaterial)
            float4 _DiffuseMap_ST;
            half4 _DiffuseColor;
            half4 _SpecColor;
            float _Glossy;
            CBUFFER_END
            
            // 顶点属性(顶点着色器的输入)
            struct VertexAttributes
            {
                float3 positionOS : POSITION; // 模型空间顶点坐标
                float3 normalOS : NORMAL; // 模型空间顶点法线
                float4 tangentOS : TANGENT; // 模型空间顶点切线，原本切线是3维的，这里使用4维，w分量的作用是确定副切线的方向
                float2 texcoord : TEXCOORD; // 纹理坐标
            };
            // 顶点着色器输入，经过GPU光栅化插值处理后，同时也是像素着色器的输入
            struct Varyings
            {
                float4 positionCS : SV_POSITION; // 裁剪空间顶点坐标
                float3 positionWS : TEXCOORD1; //  世界空间坐标

                float2 uv : TEXCOORD;   // 纹理坐标
            };

            Varyings VertexMain(VertexAttributes input)
            {
                Varyings output;
                // 顶点变化
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                
                // uv偏移
                output.uv = TRANSFORM_TEX(input.texcoord, _DiffuseMap);

                return output;
            }

            half4 FragmentMain(Varyings input) : SV_TARGET
            {
                half4 finalColor = half4(1.0, 1.0, 1.0, 1.0);
                return finalColor;
            }

            ENDHLSL
        }
    }
}
