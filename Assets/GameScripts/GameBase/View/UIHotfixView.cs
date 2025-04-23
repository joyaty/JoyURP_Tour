
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Joy.Base.UI
{
    /// <summary>
    /// 资源更新UI控制脚本
    /// </summary>
    public class UIHotfixView : MonoBehaviour
    {
        /// <summary>
        /// 背景图
        /// </summary>
        [SerializeField] private Image m_Background;

        /// <summary>
        /// 版本信息
        /// </summary>
        [SerializeField] private TextMeshProUGUI m_VersionCode;
        
        /// <summary>
        /// 进度条
        /// </summary>
        [SerializeField] private Slider m_Progress;

        /// <summary>
        /// 进度提示信息
        /// </summary>
        [SerializeField] private TextMeshProUGUI m_Tips;
    }
}
