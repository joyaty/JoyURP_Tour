
using UnityEngine;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using Cysharp.Threading.Tasks;
using YooAsset;
using UnityEngine.SceneManagement;

namespace UnityGameFramework
{
    /// <summary>
    /// 资源管理模块组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/AssetComponent")]
    public sealed class AssetComponent : GameFrameworkComponent
    {
        #region 脚本可配置项

        /// <summary>
        /// 资源管理模块工作模式
        /// </summary>
        [SerializeField] private EnumAssetWorkingMode m_WorkingMode;

        /// <summary>
        /// 默认资源包包名
        /// </summary>
        [SerializeField] private string m_DefaultPackageName = "";

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

        /// <summary>
        /// 获取默认资源包名称
        /// </summary>
        public string DefaultPackageName => m_DefaultPackageName;

        /// <summary>
        /// 资源包工作模式
        /// </summary>
        public EnumAssetWorkingMode WorkingMode => m_WorkingMode;

        private void Start()
        {
            if (IsInitialized)
            {
                LogUtil.Warning("AssetModule has initialized. forbidden duplicate Initialize.");
            }
            // 创建资源管理实现模块，并且初始化
            m_AssetModule = GameFrameworkEntry.GetModule<IAssetModule>();
            m_AssetModule.Initialize(m_DefaultPackageName, LogUtil.GetLogHelper() as YooAsset.ILogger);
        }

        /// <summary>
        /// 初始化资源包
        /// </summary>
        /// <param name="packageName">资源包名</param>
        /// <returns></returns>
        public async UniTask<bool> InitializePackage(string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? packageName : m_DefaultPackageName;
            string[] remoteURLs = null;
            if (m_WorkingMode == EnumAssetWorkingMode.HostMode)
            {
                remoteURLs = new string[] { m_RemoteMainURL, m_RemoteFallbackURL };
            }
            InitializationOperation initializationOperation = await m_AssetModule.InitializePackage(name, m_WorkingMode, remoteURLs);
            return initializationOperation.Status == EOperationStatus.Succeed;
        }

        /// <summary>
        /// 请求(本地|远程)最新的资源版本号
        /// </summary>
        /// <param name="packageName">资源包名</param>
        /// <returns></returns>
        public async UniTask<(bool, string)> RequestPackageVersion(string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? packageName : m_DefaultPackageName;
            RequestPackageVersionOperation requestPackageVersionOperation = await m_AssetModule.RequestPackageVersion(name);
            return (requestPackageVersionOperation.Status == EOperationStatus.Succeed, requestPackageVersionOperation.PackageVersion);
        }

        /// <summary>
        /// 更新资源列表文件
        /// </summary>
        /// <param name="resVersionCode">资源版本号</param>
        /// <param name="packageName">资源包名</param>
        /// <returns></returns>
        public async UniTask<bool> UpdatePackageManifest(string resVersionCode, string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? packageName : m_DefaultPackageName;
            UpdatePackageManifestOperation updatePackageManifestOperation = await m_AssetModule.UpdatePackageManifest(resVersionCode, name);
            return updatePackageManifestOperation.Status == EOperationStatus.Succeed;
        }

        public Scene LoadSceneSync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "")
        {
            return m_AssetModule.LoadSceneSync(location, loadSceneMode, packageName);
        }
    }
}