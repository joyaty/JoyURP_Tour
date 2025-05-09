
namespace GameFramework.Resource
{
    /// <summary>
    /// 资源模块工作模式
    /// </summary>
    public enum EnumAssetWorkingMode : byte
    {
        EditorMode = 0,         // 编辑器模式
        LocalMode = 1,          // 本地资源模式
        HostMode = 2,           // 远程热更模式
        WebMode = 3,            // 小游戏模式
    }
    
    /// <summary>
    /// 资源清理模式
    /// </summary>
    public enum EnumResCleanMode : byte
    {
        /// <summary>
        /// 清理所有文件
        /// </summary>
        ClearAllBundleFiles = 0,

        /// <summary>
        /// 清理未在使用的文件
        /// </summary>
        ClearUnusedBundleFiles,

        /// <summary>
        /// 清理指定标签的文件
        /// 说明：需要指定参数，可选：string, string[], List<string>
        /// </summary>
        ClearBundleFilesByTags,

        /// <summary>
        /// 清理所有清单
        /// </summary>
        ClearAllManifestFiles,

        /// <summary>
        /// 清理未在使用的清单
        /// </summary>
        ClearUnusedManifestFiles,
    }

    /// <summary>
    /// 资源管理模块的一些扩展方法定义
    /// </summary>
    public static class AssetModuleExtension
    {
        /// <summary>
        /// EnumAssetWorkingMode => YooAsset.EPlayMode
        /// </summary>
        /// <param name="workingMode"></param>
        /// <returns></returns>
        public static YooAsset.EPlayMode ToEPlayMode(this EnumAssetWorkingMode workingMode)
        {
            return workingMode switch
            {
                EnumAssetWorkingMode.EditorMode => YooAsset.EPlayMode.EditorSimulateMode,
                EnumAssetWorkingMode.LocalMode => YooAsset.EPlayMode.OfflinePlayMode,
                EnumAssetWorkingMode.HostMode => YooAsset.EPlayMode.HostPlayMode,
                EnumAssetWorkingMode.WebMode => YooAsset.EPlayMode.WebPlayMode,
                _ => YooAsset.EPlayMode.CustomPlayMode,
            };
        }

        /// <summary>
        /// EnumResCleanMode => YooAsset.EFileClearMode
        /// </summary>
        /// <param name="workingMode"></param>
        /// <returns></returns>
        public static YooAsset.EFileClearMode ToEFileClearMode(this EnumResCleanMode workingMode)
        {
            return workingMode switch
            {
                EnumResCleanMode.ClearAllBundleFiles => YooAsset.EFileClearMode.ClearAllBundleFiles,
                EnumResCleanMode.ClearUnusedBundleFiles => YooAsset.EFileClearMode.ClearUnusedBundleFiles,
                EnumResCleanMode.ClearBundleFilesByTags => YooAsset.EFileClearMode.ClearBundleFilesByTags,
                EnumResCleanMode.ClearAllManifestFiles => YooAsset.EFileClearMode.ClearAllManifestFiles,
                EnumResCleanMode.ClearUnusedManifestFiles => YooAsset.EFileClearMode.ClearUnusedManifestFiles,
                _ => YooAsset.EFileClearMode.ClearAllBundleFiles,
            };
        }
    }
}