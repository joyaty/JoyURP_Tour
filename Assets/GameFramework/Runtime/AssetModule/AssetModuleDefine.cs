
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
    }
}