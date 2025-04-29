
using Cysharp.Threading.Tasks;
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
        void Initialize(string defaultPackageName);

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
        /// 获取资源包
        /// </summary>
        /// <param name="packageName">资源包名称，空为默认资源包</param>
        /// <returns></returns>
        ResourcePackage GetPackage(string packageName = "");
    }
}