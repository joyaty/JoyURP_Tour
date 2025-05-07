// using System.Collections;
// using UnityEngine;
// using YooAsset;

// public class Launcher : MonoBehaviour
// {
//     private const string kDefaulePackage = "DefaultPackage";

//     public EPlayMode playMode = EPlayMode.EditorSimulateMode;

//     /// <summary>
//     /// 不同平台资源更新地址
//     /// </summary>
// #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//     private const string m_ResUpdateURL = "http://192.168.31.104:9081/App/PC/v1.0/";
// #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
//     private const string m_ResUpdateURL = "http://192.168.31.104:9081/App/OSX/v1.0/";
// #elif UNITY_ANDROID
//     private const string m_ResUpdateURL = "http://192.168.31.104:9081/App/Android/v1.0/";
// #elif UNITY_IOS
//     private const string m_ResUpdateURL = "http://192.168.31.104:9081/App/IOS/v1.0/";
// #endif

//     private void Awake()
//     {
//         YooAssets.Initialize();
//         if (playMode == EPlayMode.EditorSimulateMode)
//         {
//             StartCoroutine(InitPackageWithEditorMode(OnAssetModuleInitSuccess));
//         }
//         else if (playMode == EPlayMode.OfflinePlayMode)
//         {
//             StartCoroutine(InitPackageWithOfflineMode(OnAssetModuleInitSuccess));
//         }
//         else if (playMode == EPlayMode.HostPlayMode)
//         {
//             StartCoroutine(InitPackageWithRemoteMode(OnAssetModuleInitSuccess));
//         }
//     }

//     /// <summary>
//     /// 编辑器模式下模拟模式
//     /// </summary>
//     private IEnumerator InitPackageWithEditorMode(System.Action onSuccessCallBack)
//     {
//         var package = YooAssets.TryGetPackage(kDefaulePackage);
//         if (package == null)
//         {
//             package = YooAssets.CreatePackage(kDefaulePackage);
//         }
//         var buildResult = EditorSimulateModeHelper.SimulateBuild(kDefaulePackage);
//         var initParamters = new EditorSimulateModeParameters();
//         initParamters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
//         InitializationOperation initOperation = package.InitializeAsync(initParamters);
//         yield return initOperation;
//         if (initOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log("资源包初始化成功!");
//         }
//         else
//         {
//             Debug.LogError("资源包初始化失败!");
//             yield break;
//         }
//         RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
//         yield return versionOperation;
//         if (versionOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"资源版本获取成功!, ReqVersion = {versionOperation.PackageVersion}");
//         }
//         else
//         {
//             Debug.LogError("资源版本获取失败!");
//             yield break;
//         }

//         UpdatePackageManifestOperation updatePackageManifest = package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
//         yield return updatePackageManifest;
//         if (updatePackageManifest.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"获取资源清单!, PackageVersion = {package.GetPackageVersion()}");
//         }
//         else
//         {
//             Debug.LogError("获取资源清单失败!");
//             yield break;
//         }
//         YooAssets.SetDefaultPackage(package);
//         // 设置默认的资源包
//         onSuccessCallBack?.Invoke();
//     }

//     /// <summary>
//     /// 离线单机模式，一般不热更
//     /// </summary>
//     private IEnumerator InitPackageWithOfflineMode(System.Action onSuccessCallBack)
//     {
//         ResourcePackage package = YooAssets.TryGetPackage(kDefaulePackage);
//         if (package == null)
//         {
//             package = YooAssets.CreatePackage(kDefaulePackage);
//         }
//         OfflinePlayModeParameters initializeParameters = new OfflinePlayModeParameters();
//         initializeParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
//         InitializationOperation initializationOperation = package.InitializeAsync(initializeParameters);
//         yield return initializationOperation;
//         if (initializationOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"资源包初始化成功!");
//         }
//         else
//         {
//             Debug.LogError("资源包初始化失败!");
//             yield break;
//         }
//         RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
//         yield return versionOperation;
//         if (versionOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"获取资源版本成功!, ReqVersion = {versionOperation.PackageVersion}");
//         }
//         else
//         {
//             Debug.LogError("获取资源版本失败!");
//             yield break;
//         }
//         UpdatePackageManifestOperation manifestOperation = package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
//         yield return manifestOperation;
//         if (versionOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"获取资源清单成功!, PackageVersion = {package.GetPackageVersion()}");
//         }
//         else
//         {
//             Debug.LogError("获取资源清单失败!");
//             yield break;
//         }
//         YooAssets.SetDefaultPackage(package);
//         onSuccessCallBack?.Invoke();
//     }

//     /// <summary>
//     /// 远程联机模式，一般会有热更功能
//     /// </summary>
//     private IEnumerator InitPackageWithRemoteMode(System.Action onSuccessCallBack)
//     {
//         ResourcePackage package = YooAssets.TryGetPackage(kDefaulePackage);
//         if (package == null)
//         {
//             package = YooAssets.CreatePackage(kDefaulePackage);
//         }
//         string remoteURL = m_ResUpdateURL;
//         string fallbackURL = m_ResUpdateURL;
//         IRemoteServices remoteServices = new RemoteServices(remoteURL, fallbackURL);
//         HostPlayModeParameters initializeParamters = new HostPlayModeParameters();
//         initializeParamters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
//         initializeParamters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
//         InitializationOperation initializationOperation = package.InitializeAsync(initializeParamters);
//         yield return initializationOperation;
//         if (initializationOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log("资源包初始化成功!");
//         }
//         else
//         {
//             Debug.LogError("资源包初始化失败!");
//             yield break;
//         }
//         RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
//         yield return versionOperation;
//         if (versionOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"获取资源版本成功!, ReqVersion = {versionOperation.PackageVersion}");
//         }
//         else
//         {
//             Debug.LogError("获取资源版本失败!");
//             yield break;
//         }
//         UpdatePackageManifestOperation manifestOperation = package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
//         yield return manifestOperation;
//         if (manifestOperation.Status == EOperationStatus.Succeed)
//         {
//             Debug.Log($"获取资源清单成功!, PackageVersion = {package.GetPackageVersion()}");
//         }
//         else
//         {
//             Debug.LogError("获取资源清单失败!");
//             yield break;
//         }

//         var downloader = package.CreateResourceDownloader(10, 3);

//         int needDownloadCount = downloader.TotalDownloadCount;
//         long totalDownloadBytes = downloader.TotalDownloadBytes;
//         Debug.Log($"资源下载, needDownloadCount = {needDownloadCount}, totalDownloadBytes = {totalDownloadBytes}");
//         if (needDownloadCount > 0)
//         {
//             downloader.BeginDownload();
//             yield return downloader;
//             if (downloader.Status == EOperationStatus.Succeed)
//             {
//                 Debug.Log($"资源下载成功");
//             }
//             else
//             {
//                 Debug.LogError("资源下载失败");
//                 yield break;
//             }
//         }

//         YooAssets.SetDefaultPackage(package);
//         onSuccessCallBack?.Invoke();
//     }

//     /// <summary>
//     /// 小游戏模式
//     /// </summary>
//     private void InitPackageWithWebGLMode()
//     {

//     }

//     private void OnAssetModuleInitSuccess()
//     {
//         AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Prefabs_Craft");
//         if (handle.AssetObject != null)
//         {
//             Instantiate(handle.GetAssetObject<GameObject>());
//         }
//     }

//     private sealed class RemoteServices : IRemoteServices
//     {
//         public readonly string m_RemoteMainURL = null;
//         private readonly string m_FallbackURL = null;

//         public RemoteServices(string remoteMainURL, string fallbackURL)
//         {
//             m_RemoteMainURL = remoteMainURL;
//             m_FallbackURL = fallbackURL;
//         }

//         public string GetRemoteFallbackURL(string fileName)
//         {
//             return $"{m_FallbackURL}/{fileName}";
//         }

//         public string GetRemoteMainURL(string fileName)
//         {
//             return $"{m_RemoteMainURL}/{fileName}";
//         }
//     }
// }
