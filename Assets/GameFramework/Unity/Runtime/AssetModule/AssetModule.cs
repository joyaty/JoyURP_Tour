
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 基于YooAssets资源管理插件封装的资源管理模块实现
    /// </summary>
    public sealed class AssetModule : IAssetModule
    {
        /// <summary>
        /// 资源管理模块是否初始化完成
        /// </summary>
        public bool IsInitialized => YooAssets.Initialized;

        public void Initialize(string defaultPackageName)
        {
            // 初始化YooAssets模块，
            YooAssets.Initialize();
            // 设置默认资源包
            ResourcePackage package = YooAssets.TryGetPackage(defaultPackageName);
            package ??= YooAssets.CreatePackage(defaultPackageName);
            YooAssets.SetDefaultPackage(package);
        }

        public void InitializePackage(string packageName, EnumResWorkingMode workingMode, string[] remoteResURLs = null, IResDecryptionService resDecryption = null)
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
            switch (workingMode)
            {
                case EnumResWorkingMode.EditorMode:
                    InitializationWithEditorMode(package);
                    break;
                case EnumResWorkingMode.LocalMode:
                    InitializationWithLocalMode(package, resDecryption);
                    break;
                case EnumResWorkingMode.HostMode:
                    InitializationWithHostMode(package, remoteResURLs, resDecryption);
                    break;
                case EnumResWorkingMode.WebMode:
                    InitializationWithWebMode(package);
                    break;
            }
        }

        public void RequestVersion(string packageName)
        {

        }

        public void UpdateManifest(string packageName, string versionCode)
        {

        }

        /// <summary>
        /// 编辑器模式初始化资源资源包
        /// </summary>
        private void InitializationWithEditorMode(ResourcePackage package)
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
            initializationOperation.Completed += OnPackageInitializationCompleted;
        }

        /// <summary>
        /// 单机模式初始化资源资源包(全部资源在包内，不需要热更新)
        /// </summary>
        /// <param name="package"></param>
        /// <param name="decryptionService"></param>
        private void InitializationWithLocalMode(ResourcePackage package, IResDecryptionService decryptionService)
        {
            // YooAsset默认路径下构建文件系统参数(项目路径/Assets/StreamingAssets)
            FileSystemParameters fileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters(decryptionService);
            // 初始化参数，绑定内置文件系统
            OfflinePlayModeParameters parameters = new OfflinePlayModeParameters();
            parameters.BuildinFileSystemParameters = fileSystemParameters; // 文件系统初始化参数传递
            // 初始化资源包
            InitializationOperation initializationOperation = package.InitializeAsync(parameters);
            initializationOperation.Completed += OnPackageInitializationCompleted;
        }

        /// <summary>
        /// 远程模式初始化资源包(可以从远程更新资源到沙盒中)
        /// </summary>
        /// <param name="package"></param>
        /// <param name="remoteResURLs"></param>
        /// <param name="decryptionService"></param>
        private void InitializationWithHostMode(ResourcePackage package, string[] remoteResURLs, IResDecryptionService decryptionService)
        {
            if (remoteResURLs == null || remoteResURLs.Length <= 0)
            {
                LogUtil.Error("Remote res url must not be empty when using HostMode！");
                return;
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
            initializationOperation.Completed += OnPackageInitializationCompleted;
        }

        /// <summary>
        /// 小游戏模式
        /// </summary>
        private void InitializationWithWebMode(ResourcePackage package)
        {
            LogUtil.Fatal("InitializationWithWebMode, UnDefined!");
        }

        #region 异步回调函数

        /// <summary>
        /// 包初始化结束回调
        /// </summary>
        /// <param name="operation"></param>
        private void OnPackageInitializationCompleted(AsyncOperationBase operation)
        {

        }

        #endregion
    }
}