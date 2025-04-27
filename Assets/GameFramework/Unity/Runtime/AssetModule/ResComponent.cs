
using UnityEngine;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework
{
    /// <summary>
    /// 资源管理模块组件
    /// </summary>
    public sealed class ResComponent : GameFrameworkComponent
    {
        #region 脚本可配置项

        /// <summary>
        /// 资源管理模块工作模式
        /// </summary>
        [SerializeField] private EnumResWorkingMode m_WorkingMode;

        /// <summary>
        /// 默认资源包包名
        /// </summary>
        [SerializeField] private string m_DefaultPackage;

        /// <summary>
        /// 资源更新远程地址
        /// </summary>
        [SerializeField] private string m_RemoteMainURL;

        /// <summary>
        /// 资源更新远程备用地址
        /// </summary>
        [SerializeField] private string m_RemoteFallbackURL;

        #endregion

        /// <summary>
        /// 资源管理具体实现模块
        /// </summary>
        private IAssetModule m_AssetModule;

        /// <summary>
        /// 资源模块是否初始化完成
        /// </summary>
        public bool IsInitialized => m_AssetModule != null && m_AssetModule.IsInitialized;

        private void Start()
        {
            if (IsInitialized)
            {
                LogUtil.Warning("AssetModule has initialized. forbidden duplicate Initialize.");
            }
            // 创建资源管理实现模块，并且初始化
            m_AssetModule = GameFrameworkEntry.GetModule<IAssetModule>();
            m_AssetModule.Initialize(m_DefaultPackage);
        }
    }
}