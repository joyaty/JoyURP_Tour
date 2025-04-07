// 基于Blinn-Phong光照模型渲染的场景对象
Shader "JoyURP/JoyURP_SimpleLit"
{
    Properties
    {
        [MainTexture] _DiffuseMap ("Diffuse Map", 2D) = "white" { }// 漫反射贴图
        [MainColor] _DiffuseColor ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0) // 漫反射颜色
        _BumpMap ("Normal Map", 2D) = "bump" { }// 法线贴图
        _SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0) // 高光颜色
        _Glossy ("Glossy", Range(0, 1)) = 0.5 // 光滑度
        
        // _Surface ("Surface Type", Float) = 1.0 // 表面类型，Opaque或者Transparent
        _QueueOffset ("RenderQueue Offset", Float) = 0.0 // 渲染队列顺序偏移

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

            // 材质Keywords
            #pragma shader_feature_local _ENABLE_NORMAL_MAP // 是否启用法线贴图

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
                float4 positionCS : SV_POSITION;    // 裁剪空间顶点坐标
                float3 positionWS : TEXCOORD1;      // 世界空间坐标
                #if _ENABLE_NORMAL_MAP
                    float3 normalWS : TEXCOORD2;    // 世界空间法线方向
                    float4 tangentWS : TEXCOORD3;   // xyz为世界空间切线方向，w分量用于确定副切线方向
                #else
                    float3 normalWS : TEXCOORD2;
                #endif
                float2 uv : TEXCOORD;   // 纹理坐标

            };
            
            // 顶点着色器入口
            Varyings VertexMain(VertexAttributes input)
            {
                Varyings output;
                // 顶点变换
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                // 法线变换
                #if _ENABLE_NORMAL_MAP
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = normalInput.normalWS;
                    float w = input.tangentOS.w * GetOddNegativeScale();
                    output.tangentWS = float4(normalInput.normalWS, w);
                #else
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = normalInput.normalWS;
                #endif
                // uv偏移
                output.uv = TRANSFORM_TEX(input.texcoord, _DiffuseMap);

                return output;
            }
            
            // 像素着色器入口
            half4 FragmentMain(Varyings input) : SV_TARGET
            {
                Light mainLight = GetMainLight();
                float3 l = normalize(mainLight.direction);
                #if _ENABLE_NORMAL_MAP
                    half4 normal4TS = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv); 
                    half3 normalTS = UnpackNormal(normal4TS); // 法线纹理的法线数据为切线空间法线
                    float sign = input.tangentWS.w;
                    float3 bitangentWS = cross(input.normalWS.xyz, input.tangentWS.xyz) * sign;
                    float3x3 tangentToWorldMatrix = float3x3(input.tangentWS.xyz, bitangentWS.xyz, input.normalWS.xyz);
                    float3 n = TransformTangentToWorld(normalTS, tangentToWorldMatrix);
                #else
                    float3 n = normalize(input.normalWS);
                #endif
                float3 r = GetWorldSpaceNormalizeViewDir(input.positionWS);
                // 漫反射项
                float NdotL = saturate(dot(n, l));
                half3 albedo = SAMPLE_TEXTURE2D(_DiffuseMap, sampler_DiffuseMap, input.uv).xyz * _DiffuseColor.xyz;
                half3 diffuse = mainLight.color * NdotL * albedo.xyz;
                // 高光项
                float3 h = normalize(l + r); // 中间向量
                float NdotH = saturate(dot(n, h)); // 法线和中间向量的夹角余弦
                float glossy = 1 + _Glossy * 256; // [0, 1]区间的光滑度映射到[1, 256]区间
                half3 specular = mainLight.color * (8 + glossy) / 8 * pow(NdotH, glossy) * _SpecColor.xyz;
                // 环境光项
                float3 ambient = _GlossyEnvironmentColor.xyz * albedo.xyz;
                // 合成最终颜色
                half4 finalColor = half4(diffuse + ambient + specular, 1);
                return finalColor;
            }

            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Joy.ShaderEditor.JoySimpleLitShaderEditor"
}
