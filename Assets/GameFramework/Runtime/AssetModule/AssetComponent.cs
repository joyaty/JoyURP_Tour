
using UnityEngine;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using Cysharp.Threading.Tasks;
using YooAsset;
using UnityEngine.SceneManagement;
using System;

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
                remoteURLs = new string[]
                {
                    m_RemoteMainURL + GetURLSuffix(Application.platform),
                    m_RemoteFallbackURL + GetURLSuffix(Application.platform)
                };
                LogUtil.Debug("远程资源地址: {0}, {1}", remoteURLs[0], remoteURLs[1]);
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

        /// <summary>
        /// 清理缓存资源
        /// </summary>
        /// <param name="cleanMode"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public async UniTask<bool> ClearCacheFile(EnumResCleanMode cleanMode, string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? packageName : m_DefaultPackageName;
            ClearCacheFilesOperation clearCacheFilesOperation = await m_AssetModule.ClearCacheFile(cleanMode.ToEFileClearMode(), name);
            return clearCacheFilesOperation.Status == EOperationStatus.Succeed;
        }

        /// <summary>
        /// 启动下载远端资源
        /// </summary>
        /// <param name="onDownloadProgress"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public async UniTask<bool> StartDownloadResource(Action<int, int, long, long> onDownloadProgress, string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? packageName : m_DefaultPackageName;
            ResourceDownloaderOperation downloaderOperation = await m_AssetModule.StartDownload(onDownloadProgress, name);
            // 需要下载的资源数为空，或者资源下载是否成功
            return downloaderOperation.TotalDownloadCount <= 0 || downloaderOperation.Status == EOperationStatus.Succeed;
        }

        public Scene LoadSceneSync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "")
        {
            return m_AssetModule.LoadSceneSync(location, loadSceneMode, packageName);
        }

        public UniTask<Scene> LoadSceneAsync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "")
        {
            return m_AssetModule.LoadSceneAsync(location, loadSceneMode, packageName);
        }

        public T LoadAssetSync<T>(string location, string packageName = "") where T : UnityEngine.Object
        {
            return m_AssetModule.LoadAssetSync<T>(location, packageName);
        }

        public UniTask<T> LoadAssetAsync<T>(string location, string packageName = "") where T : UnityEngine.Object
        {
            return m_AssetModule.LoadAssetAsync<T>(location, packageName);
        }

        public UniTask<GameObject> InstantiateGameObject(string location, Transform parent = null, string packageName = "")
        {
            return m_AssetModule.InstantiateGameObjectAsync(location, parent, packageName);
        }

        /// <summary>
        /// 根据平台类别，获取远程地址后缀
        /// </summary>
        /// <param name="ePlatform">平台枚举</param>
        /// <returns></returns>
        private static string GetURLSuffix(RuntimePlatform ePlatform)
        {
            return ePlatform switch
            {
                RuntimePlatform.Android => "/App/Android/v1.0/",
                RuntimePlatform.WindowsPlayer => "/App/PC/v1.0/",
                RuntimePlatform.WindowsEditor => "/App/PC/v1.0/",
                RuntimePlatform.OSXEditor => "/App/OSX/v1.0/",
                RuntimePlatform.OSXPlayer => "/App/OSX/v1.0/",
                RuntimePlatform.IPhonePlayer => "/App/IOS/v1.0/",
                _ => "/App/Unknow/v1.0/",
            };
        }
    }
}