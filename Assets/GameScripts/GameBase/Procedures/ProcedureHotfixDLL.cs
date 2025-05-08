
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using HybridCLR;
using Joy.Base.Config;
using UnityEngine;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    public sealed class ProcedureHotfixDLL : ProcedureBase
    {
        /// <summary>
        /// Dll清单文件资源定位符
        /// </summary>
        private const string DLL_MANIFEST_LOCATION = "DllManifest";

        /// <summary>
        /// 热更新代码入口预制体资源定位符
        /// </summary>
        private const string HOTFIX_ENTRY_LOCATION = "HotfixEntry";

        /// <summary>
        /// 资源管理组件
        /// </summary>
        private AssetComponent m_AssetComponent = null;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            m_AssetComponent = GameEntry.GetComponent<AssetComponent>();
            // 打包运行，使用热更新dll资源
            LoadDll().Forget();
        }

        /// <summary>
        /// 加载DLL文件
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid LoadDll()
        {
            // 加载Dll清单描述文件
            TextAsset dllManifestAsset = await m_AssetComponent.LoadAssetAsync<TextAsset>(DLL_MANIFEST_LOCATION);
            DllManifest manifest = JsonUtility.FromJson<DllManifest>(dllManifestAsset.text);
            // 补充AOT泛型元数据
            await LoadMetadataForAOTAssemblies(manifest.aotDlls);
            // 加载热更新Dll
            await LoadHotfixAssemblies(manifest.hotfixDlls);
            // 加载热更新入口Prefab，进入热更新后的游戏流程
            RunHotfixEntry().Forget();
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private async UniTask LoadMetadataForAOTAssemblies(string[] aotDlls)
        {
            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in aotDlls)
            {
                TextAsset dllAsset = await m_AssetComponent.LoadAssetAsync<TextAsset>(aotDllName);
                if (dllAsset == null)
                {
                    LogUtil.Error("LoadMetadataForAOTAssembly Error!, dllAsset is null, dllName = {0}", aotDllName);
                    continue;
                }
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode errorCode = RuntimeApi.LoadMetadataForAOTAssembly(dllAsset.bytes, mode);
                LogUtil.Debug("LoadMetadataForAOTAssembly:{0}. mode:{1} ret:{2}", aotDllName, mode, errorCode);
            }
        }

        /// <summary>
        /// 加载热更新的Dll资源，生成程序集
        /// </summary>
        /// <param name="hotfixDlls"></param>
        /// <returns></returns>
        private async UniTask LoadHotfixAssemblies(string[] hotfixDlls)
        {
            foreach (var hotfixDllName in hotfixDlls)
            {
                // 加载热更新Dll资源
                TextAsset dllAsset = await m_AssetComponent.LoadAssetAsync<TextAsset>(hotfixDllName);
                if (dllAsset == null)
                {
                    LogUtil.Error("LoadHotfixAssemblies Error!, dllAsset is null, dllName = {0}", hotfixDllName);
                    continue;
                }
                // 加载热更新Dll
                var hotfixDll = Assembly.Load(dllAsset.bytes);
                LogUtil.Debug("LoadHotfixAssemblies:{0}.", hotfixDll.FullName);
            }
        }

        /// <summary>
        /// 启动热更新代码入口
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid RunHotfixEntry()
        {
            GameObject hotfixEntryPrefab = await m_AssetComponent.LoadAssetAsync<GameObject>(HOTFIX_ENTRY_LOCATION);
            // TODO 挂载热更新代码入口到当前场景上
            // m_AssetComponent.InitializeObject(hotfixEntryPrefab);
        }
    }
}