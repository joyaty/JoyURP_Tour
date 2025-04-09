
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace Joy.ShaderEditor
{
    /// <summary>
    /// 自定义的JoySimpleLitShader材质数据面板
    /// </summary>
    public class JoySimpleLitShaderEditor : BaseShaderGUI
    {
        /// <summary>
        /// Shader的Properties name定义
        /// </summary>
        internal static class JoySimpleLitPropDefine
        {
            /// <summary>
            /// 漫反射贴图
            /// </summary>
            internal static readonly string DIFFUSE_MAP = "_DiffuseMap";
            /// <summary>
            /// 漫反射颜色
            /// </summary>
            internal static readonly string DIFFUSE_COLOR = "_DiffuseColor";
            /// <summary>
            /// 高光颜色
            /// </summary>
            internal static readonly string SPECULAR_COLOR = "_SpecColor";
            /// <summary>
            /// 光滑度
            /// </summary>
            internal static readonly string GLOSSY = "_Glossy";

            /// <summary>
            /// 凹凸(法线)贴图
            /// </summary>
            internal static readonly string BUMP_MAP = "_BumpMap";

            /// <summary>
            /// 渐变纹理
            /// </summary>
            internal static readonly string RAMP_MAP = "_RampMap";

            /// <summary>
            /// 是否启用渐变纹理开关
            /// </summary>
            internal static readonly string ENABLE_RAMP_MAP = "_EnableRampMap";

            /// <summary>
            /// 是否开启AlphaTest
            /// </summary>
            internal static readonly string ENABLE_ALPHA_TEST = "_AlphaTest";

            /// <summary>
            /// AlphaTest的临界值
            /// </summary>
            internal static readonly string ALPHA_TEST_THRESHOLD = "_ApphaThreshold";
        }

        /// <summary>
        /// 材质面板文本标签定义
        /// </summary>
        internal static class JoySimpleLitPropGUI
        {
            /// <summary>
            /// 漫反射项标签
            /// </summary>
            internal static readonly GUIContent DIFFUSE = new GUIContent("Diffuse");
            /// <summary>
            /// 高光项标签
            /// </summary>
            internal static readonly GUIContent SPECULAR = new GUIContent("Specular");
            /// <summary>
            /// 光滑度项标签
            /// </summary>
            internal static readonly GUIContent GLOSSY = new GUIContent("Glossy");
            /// <summary>
            /// 法线贴图标签
            /// </summary>
            internal static readonly GUIContent NORMAL = new GUIContent("Normal");

            /// <summary>
            /// 渐变纹理配置项
            /// </summary>
            internal static readonly GUIContent RAMPTEX = new GUIContent("Ramp Texture");

            /// <summary>
            /// 是否启用渐变纹理标签
            /// </summary>
            internal static readonly GUIContent ENABLE_RAMPTEX = new GUIContent("Enable RampTex");

            /// <summary>
            /// 是否开启AlphaTest标签
            /// </summary>
            internal static readonly GUIContent ENABLE_ALPHATEST = new GUIContent("Alpha Test");
        }

        internal enum EnumRampMapOption
        {
            DISABLE = 0,
            ENABLE = 1,
        }

        internal enum EnumAlphaTestOption
        {
            DISABLE = 0,
            ENABLE = 1,
        }

        private MaterialProperty m_DiffuseMapProp = null;
        private MaterialProperty m_DiffuseColorProp = null;
        private MaterialProperty m_SpecularColorProp = null;
        private MaterialProperty m_GlossyProp = null;
        private MaterialProperty m_NormalMapProp = null;
        private MaterialProperty m_RampMapProp = null;
        private MaterialProperty m_EnableRampMapProp = null;
        private MaterialProperty m_EnableAlphaTestProp = null;
        private MaterialProperty m_AlphaTestThresholdProp = null;

        /// <summary>
        /// 绑定Shader中声明的材质属性
        /// </summary>
        /// <param name="properties"></param>
        public override void FindProperties(MaterialProperty[] properties)
        {
            m_DiffuseMapProp = FindProperty(JoySimpleLitPropDefine.DIFFUSE_MAP, properties, false);
            m_DiffuseColorProp = FindProperty(JoySimpleLitPropDefine.DIFFUSE_COLOR, properties, false);
            m_SpecularColorProp = FindProperty(JoySimpleLitPropDefine.SPECULAR_COLOR, properties, false);
            m_GlossyProp = FindProperty(JoySimpleLitPropDefine.GLOSSY, properties, false);
            m_NormalMapProp = FindProperty(JoySimpleLitPropDefine.BUMP_MAP, properties, false);
            m_RampMapProp = FindProperty(JoySimpleLitPropDefine.RAMP_MAP, properties, false);
            m_EnableRampMapProp = FindProperty(JoySimpleLitPropDefine.ENABLE_RAMP_MAP, properties, false);
            m_EnableAlphaTestProp = FindProperty(JoySimpleLitPropDefine.ENABLE_ALPHA_TEST, properties, false);
            m_AlphaTestThresholdProp = FindProperty(JoySimpleLitPropDefine.ALPHA_TEST_THRESHOLD, properties, false);
            base.FindProperties(properties);
        }

        /// <summary>
        /// 材质属性发生变更时回调
        /// </summary>
        /// <param name="material"></param>
        public override void ValidateMaterial(Material material)
        {
            if (material.HasProperty(JoySimpleLitPropDefine.BUMP_MAP))
            { // 根据是否有配置法线贴图决定是否开启 _ENABLE_NORMAL_MAP 
                bool hasValue = material.GetTexture(JoySimpleLitPropDefine.BUMP_MAP);
                CoreUtils.SetKeyword(material, "_ENABLE_NORMAL_MAP", hasValue);
            }
            if (material.HasProperty(JoySimpleLitPropDefine.RAMP_MAP))
            { // 开关控制是否开启渐变纹理
                EnumRampMapOption option = material.HasProperty(JoySimpleLitPropDefine.ENABLE_RAMP_MAP)
                    ? (EnumRampMapOption)material.GetFloat(JoySimpleLitPropDefine.ENABLE_RAMP_MAP) : EnumRampMapOption.ENABLE;
                CoreUtils.SetKeyword(material, "_ENABLE_RAMP_MAP", option == EnumRampMapOption.ENABLE);
            }
            if (material.HasProperty(JoySimpleLitPropDefine.ENABLE_ALPHA_TEST))
            { // 是否开启AlphaTest
                EnumAlphaTestOption option = (EnumAlphaTestOption)material.GetFloat(JoySimpleLitPropDefine.ENABLE_ALPHA_TEST);
                CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_TEST", option == EnumAlphaTestOption.ENABLE);
                int renderQueue = material.shader.renderQueue; // 默认使用shader的渲染队列
                if (option == EnumAlphaTestOption.ENABLE)
                {
                    renderQueue = (int)RenderQueue.AlphaTest;
                }
                else if (option == EnumAlphaTestOption.DISABLE)
                {
                    renderQueue = (int)RenderQueue.Geometry;
                }
                material.renderQueue = renderQueue;
            }
        }

        public override void DrawSurfaceOptions(Material material)
        {
            if (m_EnableAlphaTestProp != null)
            {
                EnumAlphaTestOption option = (EnumAlphaTestOption)m_EnableAlphaTestProp.floatValue;
                EditorGUI.BeginChangeCheck();
                bool isToggle = EditorGUILayout.Toggle(JoySimpleLitPropGUI.ENABLE_ALPHATEST, option == EnumAlphaTestOption.ENABLE);
                if (EditorGUI.EndChangeCheck())
                {
                    m_EnableAlphaTestProp.floatValue = (float)(isToggle ? EnumAlphaTestOption.ENABLE : EnumAlphaTestOption.DISABLE);
                }
                bool isDisable = (EnumAlphaTestOption)m_EnableAlphaTestProp.floatValue == EnumAlphaTestOption.DISABLE;
                if (!isDisable)
                {
                    EditorGUI.BeginDisabledGroup((EnumAlphaTestOption)m_EnableAlphaTestProp.floatValue == EnumAlphaTestOption.DISABLE);
                    materialEditor.RangeProperty(m_AlphaTestThresholdProp, "AlphaTest Threshold");
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        public override void DrawSurfaceInputs(Material material)
        {
            if (m_DiffuseMapProp != null)
            { // 漫反射纹理和颜色
                if (m_DiffuseColorProp != null)
                {
                    materialEditor.TexturePropertySingleLine(JoySimpleLitPropGUI.DIFFUSE, m_DiffuseMapProp, m_DiffuseColorProp);
                }
                else
                {
                    materialEditor.TexturePropertySingleLine(JoySimpleLitPropGUI.DIFFUSE, m_DiffuseMapProp);
                }
            }
            if (m_SpecularColorProp != null)
            { // 高光颜色项
                materialEditor.ColorProperty(m_SpecularColorProp, "Specular Color");
            }
            if (m_GlossyProp != null)
            { // 光滑度
                materialEditor.RangeProperty(m_GlossyProp, "Glossy");
            }
            if (m_NormalMapProp != null)
            { // 凹凸贴图
                materialEditor.TexturePropertySingleLine(JoySimpleLitPropGUI.NORMAL, m_NormalMapProp);
            }
            if (m_DiffuseMapProp != null)
            { // 所有的纹理坐标使用相同的偏移参数
                DrawTileOffset(materialEditor, m_DiffuseMapProp);
            }
            if (m_RampMapProp != null)
            {
                EnumRampMapOption rampMapOption = (EnumRampMapOption)m_EnableRampMapProp.floatValue;
                EditorGUI.BeginDisabledGroup(rampMapOption == EnumRampMapOption.DISABLE);
                materialEditor.TexturePropertySingleLine(JoySimpleLitPropGUI.RAMPTEX, m_RampMapProp);
                EditorGUI.EndDisabledGroup();
            }
        }

        public override void DrawAdvancedOptions(Material material)
        {
            EnumRampMapOption rampMapOption = (EnumRampMapOption)m_EnableRampMapProp.floatValue;
            EditorGUI.showMixedValue = m_EnableRampMapProp.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            bool isToggle = EditorGUILayout.Toggle(JoySimpleLitPropGUI.ENABLE_RAMPTEX, rampMapOption == EnumRampMapOption.ENABLE);
            if (EditorGUI.EndChangeCheck())
            {
                m_EnableRampMapProp.floatValue = (float)(isToggle ? EnumRampMapOption.ENABLE : EnumRampMapOption.DISABLE);
            }
            EditorGUI.showMixedValue = false;
            base.DrawAdvancedOptions(material);
        }
    }
}

#endif


