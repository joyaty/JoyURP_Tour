
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using Joy.Base.Define;
using Joy.Base.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

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

        /// <summary>
        /// 进度条数值信息
        /// </summary>
        [SerializeField] private TextMeshProUGUI m_ProgressValue;

        /// <summary>
        /// 事件管理组件
        /// </summary>
        private EventComponent m_EventComponent;

        /// <summary>
        /// 当前完成的最新热更新关键节点
        /// </summary>
        private EnumHotfixKeyPoint m_HotfixKeyPoint;

        /// <summary>
        /// 进度最大值
        /// </summary>
        private int m_ProgressMax;

        /// <summary>
        /// 热更新关键节点和进度关系配置
        /// </summary>
        private static readonly Dictionary<EnumHotfixKeyPoint, int> s_DictProgresses = new Dictionary<EnumHotfixKeyPoint, int> {
            { EnumHotfixKeyPoint.ALL_START, 1 },
            { EnumHotfixKeyPoint.PACKAGE_INIT_OVER, 10 },
            { EnumHotfixKeyPoint.VERSION_CHECK_OVER, 20 },
            { EnumHotfixKeyPoint.MANIFEST_UPDATE, 30 },
            { EnumHotfixKeyPoint.ASSET_DOWNLOAD_BEGIN, 31 },
            { EnumHotfixKeyPoint.ASSET_DOWNLOAD_OVER, 80 },
            { EnumHotfixKeyPoint.ASSET_CLEANUP, 85 },
            { EnumHotfixKeyPoint.ALL_END, 100 },
        };

        private void Awake()
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
        }

        private void Start()
        {
            m_EventComponent.Subscribe(EventHotfixProcessSyncArgs.EventId, OnHotfixKeyPointChanged);
            m_HotfixKeyPoint = EnumHotfixKeyPoint.ALL_START;
            m_ProgressMax = s_DictProgresses[m_HotfixKeyPoint];
            m_Progress.value = 0;
            UpdateTipContent(m_HotfixKeyPoint);
        }

        private void OnDestroy()
        {
            m_EventComponent.Unsubscribe(EventHotfixProcessSyncArgs.EventId, OnHotfixKeyPointChanged);
        }

        private void Update()
        {
            if (m_Progress.value < m_ProgressMax)
            {
                ++m_Progress.value;
                m_ProgressValue.text = Utility.Text.Format("{0}%", m_Progress.value);
            }
        }

        /// <summary>
        /// 监听热更新流程状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="inEvt"></param>
        private void OnHotfixKeyPointChanged(object sender, GameEventArgs inEvt)
        {
            if (inEvt is not EventHotfixProcessSyncArgs evtData) { return; }
            if (!s_DictProgresses.ContainsKey(evtData.HotfixKeyPoint))
            {
                LogUtil.Error("热更新状态出错！KeyPoint = {0}", m_HotfixKeyPoint);
                return;
            }
            if (evtData.HotfixKeyPoint <= m_HotfixKeyPoint)
            {
                LogUtil.Error("热更新状态应该是递增的！KeyPoint = {0}, nextKeyPoint = {1}", m_HotfixKeyPoint, evtData.HotfixKeyPoint);
                return;
            }
            m_HotfixKeyPoint = evtData.HotfixKeyPoint;
            m_Progress.value = m_ProgressMax;
            m_ProgressMax = s_DictProgresses[m_HotfixKeyPoint];
            UpdateTipContent(m_HotfixKeyPoint);
            if (m_HotfixKeyPoint == EnumHotfixKeyPoint.ALL_END)
            {
                m_Progress.value = m_ProgressMax - 3;
            }
            m_ProgressValue.text = Utility.Text.Format("{0}%", m_Progress.value);
        }

        /// <summary>
        /// 更新提示信息文本
        /// </summary>
        /// <param name="keyPoint"></param>
        private void UpdateTipContent(EnumHotfixKeyPoint keyPoint)
        {
            switch (keyPoint)
            {
                case EnumHotfixKeyPoint.ALL_START:
                    m_Tips.text = "初始化资源包...";
                    break;

                case EnumHotfixKeyPoint.PACKAGE_INIT_OVER:
                    m_Tips.text = "检查资源版本...";
                    break;

                case EnumHotfixKeyPoint.VERSION_CHECK_OVER:
                    m_Tips.text = "更新资源清单...";
                    break;

                case EnumHotfixKeyPoint.ASSET_DOWNLOAD_BEGIN:
                    m_Tips.text = "资源下载...";
                    break;
                case EnumHotfixKeyPoint.ASSET_CLEANUP:
                    m_Tips.text = "缓存清理...";
                    break;

                case EnumHotfixKeyPoint.ALL_END:
                    m_Tips.text = "资源准备完成";
                    break;

                default:
                    break;
            }
        }
    }
}
