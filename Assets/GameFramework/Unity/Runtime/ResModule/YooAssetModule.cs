
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameFramework.Resource
{
    /// <summary>
    /// 基于YooAssets资源管理插件封装的资源管理模块实现
    /// </summary>
    public sealed class YooAssetModule : IAssetModule
    {
        /// <summary>
        /// 资源管理模块是否初始化完成
        /// </summary>
        private bool m_IsInitialized = false;
        /// <summary>
        /// 资源管理模块是否初始化完成
        /// </summary>
        public bool IsInitialized => m_IsInitialized;

        /// <summary>
        /// 资源是否需要更新
        /// </summary>
        private bool m_IsNeedUpdate = false;

        /// <summary>
        /// 是否需要更新资源
        /// </summary>
        public bool IsNeedUpdate => m_IsNeedUpdate;

        public void Initialize(string defaultPackageName, EnumResWorkingMode workingMode, IResDecryptionService resDecryption = null)
        {
#if !UNITY_EDITOR
            // 非编辑器下，禁止使用编辑器模式初始化资源模块
            if (workingMode == EnumResWorkingMode.EditorMode)
            {
                Log.Error("非编辑器模式下, 禁止使用编辑器模式初始化资源模块, workingMode = {0}", workingMode);
            }
#endif
            // 初始化YooAssets模块，并设置默认资源包
            m_IsInitialized = false;
            m_IsNeedUpdate = false;
            YooAssets.Initialize();
            ResourcePackage package = YooAssets.TryGetPackage(defaultPackageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(defaultPackageName);
            }
            YooAssets.SetDefaultPackage(package);
            // 初始化资源包
            switch (workingMode)
            {
                case EnumResWorkingMode.EditorMode:
                    InitializationWithEditorMode(package);
                    break;
                case EnumResWorkingMode.LocalMode:
                    InitializationWithLocalMode(package);
                    break;
                case EnumResWorkingMode.HostMode:
                    InitializationWithHostMode(package);
                    break;
                case EnumResWorkingMode.WebMode:
                    InitializationWithWebMode(package);
                    break;
            }
        }


        /// <summary>
        /// 编辑器模式初始化资源资源包
        /// </summary>
        private void InitializationWithEditorMode(ResourcePackage package)
        {

        }

        /// <summary>
        /// 单机模式初始化资源资源包(全部资源在包内，不需要热更新)
        /// </summary>
        private void InitializationWithLocalMode(ResourcePackage package)
        {

        }

        /// <summary>
        /// 远程模式初始化资源包(可以从远程更新资源到沙盒中)
        /// </summary>
        private void InitializationWithHostMode(ResourcePackage package)
        {

        }

        /// <summary>
        /// 小游戏模式
        /// </summary>
        private void InitializationWithWebMode(ResourcePackage package)
        {
            Log.Fatal("InitializationWithWebMode, UnDefined!");
        }
    }
}