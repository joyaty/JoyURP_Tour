using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class Launcher : MonoBehaviour
{
    private const string kDefaulePackage = "DefaultPackage";

    private void Awake()
    {
        YooAssets.Initialize();
        StartCoroutine(InitPackageWithEditorMode(OnAssetModuleInitSuccess));
    }

    /// <summary>
    /// 编辑器模式下模拟模式
    /// </summary>
    private IEnumerator InitPackageWithEditorMode(System.Action onSuccessCallBack)
    {
        var package = YooAssets.TryGetPackage(kDefaulePackage);
        if (package == null)
        {
            package = YooAssets.CreatePackage(kDefaulePackage);
        }
        var buildResult = EditorSimulateModeHelper.SimulateBuild(kDefaulePackage);
        var initParamters = new EditorSimulateModeParameters();
        initParamters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
        InitializationOperation initOperation = package.InitializeAsync(initParamters);
        yield return initOperation;
        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功!");
        }
        else
        {
            Debug.LogError("资源包初始化失败!");
            yield break;
        }
        RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
        yield return versionOperation;
        if (versionOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源版本请求成功!");
        }
        else
        {
            Debug.LogError("资源版本请求失败!");
            yield break;
        }

        UpdatePackageManifestOperation updatePackageManifest = package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
        yield return updatePackageManifest;
        if (updatePackageManifest.Status == EOperationStatus.Succeed)
        {
            Debug.Log("更新资源清单成功!");
        }
        else
        {
            Debug.LogError("更新资源清单失败!");
            yield break;
        }
        YooAssets.SetDefaultPackage(package);
        // 设置默认的资源包
        onSuccessCallBack?.Invoke();
    }

    /// <summary>
    /// 离线单机模式，一般不热更
    /// </summary>
    private void InitPackageWithOfflineMode()
    {

    }

    /// <summary>
    /// 远程联机模式，一般会有热更功能
    /// </summary>
    private void InitPackageWithRemoteMode()
    {

    }

    private void OnAssetModuleInitSuccess()
    {
        AssetHandle handle = YooAssets.LoadAssetSync<GameObject>("Prefabs_Craft");
        if (handle.AssetObject != null)
        {
            Instantiate(handle.GetAssetObject<GameObject>());
        }
    }
}
