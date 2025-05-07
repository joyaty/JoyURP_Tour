
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Joy.Base.Define;
using Joy.Base.Event;
using UnityEngine;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 更新资源清单
    /// </summary>
    public sealed class ProcedureUpdateManifest : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            UpdatePackageManifest(procedureOwner).Forget();
        }

        private async UniTaskVoid UpdatePackageManifest(IFsm<IProcedureManager> procedureOwner)
        {
            AssetComponent resComponent = GameEntry.GetComponent<AssetComponent>();
            VarString resVersionCode = procedureOwner.GetData<VarString>(GlobalDefine.kProcedurePackageVersionKey);
            bool isSuccess = await resComponent.UpdatePackageManifest(resVersionCode.Value, resComponent.DefaultPackageName);
            if (isSuccess)
            {
                LogUtil.Debug("更新资源清单文件成功, PackageName = {0}, ResVersion = {1}, Frame = {2}", resComponent.DefaultPackageName, resVersionCode.Value, Time.frameCount);
                // 通知外部资源清单更新完成
                EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
                eventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(EnumHotfixKeyPoint.MANIFEST_UPDATE));
                // 等待一小段时间作为表现层的表现时长
                await UniTask.Delay(500);
                if (resComponent.WorkingMode == GameFramework.Resource.EnumAssetWorkingMode.HostMode)
                { // 可能需要远程更新游戏资源，进入热更新流程

                }
                else
                { // 资源准备完成，进入游戏开始流程
                    eventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(EnumHotfixKeyPoint.ALL_END));
                    await UniTask.Delay(200);
                    ChangeState<ProcedureGameStart>(procedureOwner);
                }
            }
            else
            {
                LogUtil.Error("更新资源列表清单文件失败, PackageName = {0}, ResVersion = {1}", resComponent.DefaultPackageName, resVersionCode.Value);
            }
        }
    }
}