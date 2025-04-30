
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 基于YooAssets资源管理插件封装的资源管理模块实现
    /// </summary>
    internal sealed class AssetModule : GameFrameworkModule, IAssetModule
    {
        internal override int Priority => 3;

        /// <summary>
        /// 资源管理模块是否初始化完成
        /// </summary>
        public bool IsInitialized => YooAssets.Initialized;

        /// <summary>
        /// 默认资源包名
        /// </summary>
        private string m_DefaultPackageName = "";

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
        }

        #region 初始化相关

        public void Initialize(string defaultPackageName, ILogger logHelper = null)
        {
            // 初始化YooAssets模块，
            YooAssets.Initialize(logHelper);
            // 设置默认资源包
            m_DefaultPackageName = defaultPackageName;
            ResourcePackage package = YooAssets.TryGetPackage(defaultPackageName);
            package ??= YooAssets.CreatePackage(defaultPackageName);
            YooAssets.SetDefaultPackage(package);
        }

        public async UniTask<InitializationOperation> InitializePackage(string packageName, EnumAssetWorkingMode workingMode, string[] remoteResURLs = null, IAssetDecryptionService resDecryption = null)
        {
#if !UNITY_EDITOR
            // 非编辑器下，禁止使用编辑器模式初始化资源模块
            if (workingMode == EnumResWorkingMode.EditorMode)
            {
                LogUtil.Error("非编辑器模式下, 禁止使用编辑器模式初始化资源模块, workingMode = {0}", workingMode);
            }
#endif
            ResourcePackage package = YooAssets.TryGetPackage(packageName);
            package ??= YooAssets.CreatePackage(packageName);
            // 初始化资源包
            return workingMode switch
            {
                EnumAssetWorkingMode.EditorMode => await InitializationWithEditorMode(package),
                EnumAssetWorkingMode.LocalMode => await InitializationWithLocalMode(package, resDecryption),
                EnumAssetWorkingMode.HostMode => await InitializationWithHostMode(package, remoteResURLs, resDecryption),
                // EnumAssetWorkingMode.WebMode => await InitializationWithWebMode(package),
                _ => null,
            };
        }

        public async UniTask<RequestPackageVersionOperation> RequestPackageVersion(string packageName = "")
        {
            // 获取资源包
            string name = string.IsNullOrEmpty(packageName) ? m_DefaultPackageName : packageName;
            ResourcePackage package = YooAssets.GetPackage(name);
            if (package == null)
            {
                LogUtil.Error("资源包【{0}】不存在，请先通过InitializePackage初始化资源包", name);
                return null;
            }
            // 更新获取(内置|远程)的资源版本号
            RequestPackageVersionOperation requestPackageVersionOperation = package.RequestPackageVersionAsync();
            await requestPackageVersionOperation.ToUniTask();
            return requestPackageVersionOperation;
        }

        public async UniTask<UpdatePackageManifestOperation> UpdatePackageManifest(string versionCode, string packageName = "")
        {
            // 获取资源包
            string name = string.IsNullOrEmpty(packageName) ? m_DefaultPackageName : packageName;
            ResourcePackage package = YooAssets.GetPackage(name);
            if (package == null)
            {
                LogUtil.Error("资源包【{0}】不存在，请先通过InitializePackage初始化资源包", name);
                return null;
            }
            // 更新资源包的资源清单文件，并且加载
            UpdatePackageManifestOperation updatePackageManifestOperation = package.UpdatePackageManifestAsync(versionCode);
            await updatePackageManifestOperation.ToUniTask();
            return updatePackageManifestOperation;
        }

        public ResourcePackage GetPackage(string packageName = "")
        {
            string name = string.IsNullOrEmpty(packageName) ? m_DefaultPackageName : packageName;
            ResourcePackage package = YooAssets.GetPackage(name);
            if (package == null)
            {
                LogUtil.Error("资源包【{0}】不存在，请先通过InitializePackage初始化资源包", name);
                return null;
            }
            return package;
        }

        /// <summary>
        /// 编辑器模式初始化资源资源包
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        private async UniTask<InitializationOperation> InitializationWithEditorMode(ResourcePackage package)
        {
            // 编辑器模拟构建资源包
            PackageInvokeBuildResult packageRoot = EditorSimulateModeHelper.SimulateBuild(package.PackageName);
            // 文件系统初始化参数
            FileSystemParameters fileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot.PackageRootDirectory);
            // 初始化参数，绑定编辑器模拟文件系统
            EditorSimulateModeParameters parameters = new EditorSimulateModeParameters();
            parameters.EditorFileSystemParameters = fileSystemParameters; // 文件系统初始化参数传递
            // 初始化资源包
            InitializationOperation initializationOperation = package.InitializeAsync(parameters);
            await initializationOperation.ToUniTask();
            return initializationOperation;
        }

        /// <summary>
        /// 单机模式初始化资源资源包(全部资源在包内，不需要热更新)
        /// </summary>
        /// <param name="package"></param>
        /// <param name="decryptionService"></param>
        private async UniTask<InitializationOperation> InitializationWithLocalMode(ResourcePackage package, IAssetDecryptionService decryptionService)
        {
            // YooAsset默认路径下构建文件系统参数(项目路径/Assets/StreamingAssets)
            FileSystemParameters fileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters(decryptionService);
            // 初始化参数，绑定内置文件系统
            OfflinePlayModeParameters parameters = new OfflinePlayModeParameters();
            parameters.BuildinFileSystemParameters = fileSystemParameters; // 文件系统初始化参数传递
            // 初始化资源包
            InitializationOperation initializationOperation = package.InitializeAsync(parameters);
            await initializationOperation.ToUniTask();
            return initializationOperation;
        }

        /// <summary>
        /// 远程模式初始化资源包(可以从远程更新资源到沙盒中)
        /// </summary>
        /// <param name="package"></param>
        /// <param name="remoteResURLs"></param>
        /// <param name="decryptionService"></param>
        private async UniTask<InitializationOperation> InitializationWithHostMode(ResourcePackage package, string[] remoteResURLs, IAssetDecryptionService decryptionService)
        {
            if (remoteResURLs == null || remoteResURLs.Length <= 0)
            {
                LogUtil.Error("Remote res url must not be empty when using HostMode！");
                return null;
            }
            // 构造远程服务Service
            string mainURL = remoteResURLs[0];
            string fallbackURL = remoteResURLs.Length > 1 ? remoteResURLs[1] : mainURL;
            IRemoteServices remoteServices = new DefaultRemoteService(mainURL, fallbackURL);
            // 创建内置文件系统初始化参数
            FileSystemParameters buildInParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters(decryptionService);
            // 创建缓存文件系统初始化参数
            FileSystemParameters cacheParamters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices, decryptionService);
            // 初始化参数，绑定内置文件系统和热更缓存文件系统
            HostPlayModeParameters parameters = new HostPlayModeParameters();
            parameters.BuildinFileSystemParameters = buildInParameters;
            parameters.CacheFileSystemParameters = cacheParamters;
            // 初始化资源包
            InitializationOperation initializationOperation = package.InitializeAsync(parameters);
            await initializationOperation.ToUniTask();
            return initializationOperation;
        }

        // /// <summary>
        // /// 小游戏模式
        // /// </summary>
        // private async UniTask<InitializationOperation> InitializationWithWebMode(ResourcePackage package)
        // {
        //     LogUtil.Fatal("InitializationWithWebMode, UnDefined!");
        //     return null;
        // }

        #endregion

        #region 资源加载相关

        /// <summary>
        /// 同步方式加载场景
        /// </summary>
        /// <param name="location"></param>
        /// <param name="packageName"></param>
        public UnityEngine.SceneManagement.Scene LoadSceneSync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "")
        {
            SceneHandle sceneHandle = null;
            if (string.IsNullOrEmpty(packageName))
            {
                sceneHandle = YooAssets.LoadSceneSync(location, loadSceneMode);
            }
            else
            {
                ResourcePackage package = GetPackage(packageName);
                sceneHandle = package.LoadSceneSync(location, loadSceneMode);
            }
            return sceneHandle.SceneObject;
        }

        #endregion
    }
}