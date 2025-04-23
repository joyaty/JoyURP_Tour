
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源模块工作模式
    /// </summary>
    public enum EnumResWorkingMode : byte
    {
        EditorMode = 0,         // 编辑器模式
        LocalMode = 1,          // 本地资源模式
        HostMode = 2,           // 远程热更模式
        WebMode = 3,            // 小游戏模式
    }

    /// <summary>
    /// 资源管理模块的一些扩展方法定义
    /// </summary>
    public static class ResModuleExtension
    {
        /// <summary>
        /// EnumResModuleWorkingMode => YooAsset.EPlayMode
        /// </summary>
        /// <param name="workingMode"></param>
        /// <returns></returns>
        public static EPlayMode ToEPlayMode(this EnumResWorkingMode workingMode)
        {
            return workingMode switch
            {
                EnumResWorkingMode.EditorMode => EPlayMode.EditorSimulateMode,
                EnumResWorkingMode.LocalMode => EPlayMode.OfflinePlayMode,
                EnumResWorkingMode.HostMode => EPlayMode.HostPlayMode,
                EnumResWorkingMode.WebMode => EPlayMode.WebPlayMode,
                _ => EPlayMode.CustomPlayMode,
            };
        }
    }
}