
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
        void InitializePackage(string packageName, EnumResWorkingMode workingMode, string[] remoteResURLs = null, IResDecryptionService resDecryption = null);

        /// <summary>
        /// 检查资源版本号
        /// </summary>
        /// <param name="packageName"></param>
        void RequestVersion(string packageName);

        /// <summary>
        /// 更新资源列表文件
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="versionCode"></param>
        void UpdateManifest(string packageName, string versionCode);
    }
}