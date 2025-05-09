
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Joy.Base.Event;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 资源清理流程节点
    /// </summary>
    public sealed class ProcedureUnuseResClean : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            LogUtil.Debug("清理未使用的缓存资源文件");
            ClearUnuseResource(procedureOwner).Forget();

        }

        private async UniTaskVoid ClearUnuseResource(IFsm<IProcedureManager> procedureOwner)
        {
            AssetComponent assetComponent = GameEntry.GetComponent<AssetComponent>();
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            eventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(Define.EnumHotfixKeyPoint.ASSET_CLEANUP));
            bool isSucces = await assetComponent.ClearCacheFile(GameFramework.Resource.EnumResCleanMode.ClearUnusedBundleFiles);
            if (isSucces)
            {
                await UniTask.Delay(300);
                // 切换到热更Dll加载流程
                ChangeState<ProcedureHotfixDLL>(procedureOwner);
            }
            else
            {
                LogUtil.Error("资源清理出现异常!");
            }
        }
    }
}