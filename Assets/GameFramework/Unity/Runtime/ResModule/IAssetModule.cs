
namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理模块接口
    /// </summary>
    public interface IAssetModule
    {
        /// <summary>
        /// 初始化资源模块
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        /// <param name="workingMode">资源模块工作模式</param>
        /// <param name="remoteResURLs">远程资源更新地址集合</param>
        /// <param name="resDecryption">(可选)资源解密接口</param>
        void Initialize(string defaultPackageName, EnumResWorkingMode workingMode, string[] remoteResURLs = null, IResDecryptionService resDecryption = null);
    }
}