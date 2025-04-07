
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
        }

        private MaterialProperty m_DiffuseMapProp = null;
        private MaterialProperty m_DiffuseColorProp = null;
        private MaterialProperty m_SpecularColorProp = null;
        private MaterialProperty m_GlossyProp = null;
        private MaterialProperty m_NormalMapProp = null;

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
        }

        public override void DrawSurfaceOptions(Material material)
        {
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
        }

        public override void DrawAdvancedOptions(Material material)
        {
            base.DrawAdvancedOptions(material);
        }
    }
}

#endif


