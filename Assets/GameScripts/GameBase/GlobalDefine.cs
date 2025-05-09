
namespace Joy.Base.Define
{
    /// <summary>
    /// 全局字段定义
    /// </summary>
    public static class GlobalDefine
    {
        /// <summary>
        /// 流程管理器的数据Key - 资源包版本号
        /// </summary>
        public const string kProcedurePackageVersionKey = "PROCEDURE_PACKAGE_VER_KEY";
    }

    /// <summary>
    /// 游戏热更新流程关键节点枚举
    /// </summary>
    public enum EnumHotfixKeyPoint : short
    {
        INVALID = -1,                   // 非法流程
        ALL_START = 0,                  // 热更流程无状态
        PACKAGE_INIT_OVER = 1,          // 资源包初始化完成(绑定文件系统)
        VERSION_CHECK_OVER = 2,         // 资源版本号检查完成
        MANIFEST_UPDATE = 3,            // 资源清单文件更新完成
        ASSET_DOWNLOAD_BEGIN = 4,       // 资源下载开始
        ASSET_DOWNLOAD_OVER = 5,        // 资源下载结束
        ASSET_CLEANUP = 6,              // 资源清理
        ASSET_DLL_LOAD = 7,             // 加载热更Dll
        ALL_END = 10,                   // 热更新流程结束
    }
}