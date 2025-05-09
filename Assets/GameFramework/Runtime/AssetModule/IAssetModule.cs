
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理模块接口
    /// </summary>
    public interface IAssetModule
    {
        /// <summary>
        /// 资源模块是否完成初始化
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// 初始化资源模块
        /// </summary>
        /// <param name="defaultPackageName">默认资源包</param>
        /// <param name="logHelper">调试信息输出工具</param>
        void Initialize(string defaultPackageName, YooAsset.ILogger logHelper = null);

        /// <summary>
        /// 初始化资源包
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        /// <param name="workingMode">资源模块工作模式</param>
        /// <param name="remoteResURLs">远程资源更新地址集合</param>
        /// <param name="resDecryption">(可选)资源解密接口</param>
        UniTask<InitializationOperation> InitializePackage(string packageName, EnumAssetWorkingMode workingMode, string[] remoteResURLs = null, IAssetDecryptionService resDecryption = null);

        /// <summary>
        /// 检查资源版本号
        /// </summary>
        /// <param name="packageName">资源包名称，空为默认资源包</param>
        /// <returns></returns>
        UniTask<RequestPackageVersionOperation> RequestPackageVersion(string packageName = "");

        /// <summary>
        /// 更新资源列表文件
        /// </summary>
        /// <param name="versionCode">资源版本号</param>
        /// <param name="packageName">资源包名称，空为默认资源包</param>
        /// <returns></returns>
        UniTask<UpdatePackageManifestOperation> UpdatePackageManifest(string versionCode, string packageName = "");

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="onDownloadProgress"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        UniTask<ResourceDownloaderOperation> StartDownload(System.Action<int, int, long, long> onDownloadProgress, string packageName = "");

        /// <summary>
        /// 移除资源
        /// </summary>
        /// <param name="clearMode"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        UniTask<ClearCacheFilesOperation> ClearCacheFile(EFileClearMode clearMode, string packageName);

        /// <summary>
        /// 获取资源包
        /// </summary>
        /// <param name="packageName">资源包名称，空为默认资源包</param>
        /// <returns></returns>
        ResourcePackage GetPackage(string packageName = "");

        /// <summary>
        /// 加载场景 - 同步方式
        /// </summary>
        /// <param name="location">场景资源定位符</param>
        /// <param name="loadSceneMode">场景加载模式</param>
        /// <param name="packageName">资源包名，空 = 默认资源包</param>
        /// <returns></returns>
        UnityEngine.SceneManagement.Scene LoadSceneSync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "");

        /// <summary>
        /// 加载场景 - 异步方式
        /// </summary>
        /// <param name="location">场景资源定位符</param>
        /// <param name="loadSceneMode">场景加载模式</param>
        /// <param name="packageName">资源包名，空 = 默认资源包</param>
        /// <returns></returns>
        UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(string location, LoadSceneMode loadSceneMode = LoadSceneMode.Single, string packageName = "");

        /// <summary>
        /// 同步方式加载UnityEngine.Object资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        T LoadAssetSync<T>(string location, string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载UnityEngine.Object资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        UniTask<T> LoadAssetAsync<T>(string location, string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 初始化场景GameObject
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        UniTask<GameObject> InstantiateGameObjectAsync(string location, Transform parent = null, string packageName = "");
    }
}