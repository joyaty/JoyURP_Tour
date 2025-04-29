
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Joy.Base.Define;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 获取(本地或远程)的资源版本号流程节点
    /// </summary>
    public sealed class ProcedureRequestVersion : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            RequestPackageVersion(procedureOwner).Forget();
        }

        private async UniTaskVoid RequestPackageVersion(IFsm<IProcedureManager> procedureOwner)
        {
            AssetComponent resComponent = GameEntry.GetComponent<AssetComponent>();
            var (isSuccess, versionCode) = await resComponent.RequestPackageVersion(resComponent.DefaultPackageName);
            if (isSuccess)
            {
                LogUtil.Debug("请求资源版本失败, PackageName = {0}, Version = {1}", resComponent.DefaultPackageName, versionCode);
                // 写入资源版本号到流程管理器中，用于后续流程使用
                procedureOwner.SetData<VarString>(GlobalDefine.kProcedurePackageVersionKey, versionCode);
                // 切换到更新资源列表文件流程节点
                ChangeState<ProcedureUpdateManifest>(procedureOwner);
            }
            else
            {
                LogUtil.Error("请求资源版本失败, PackageName = {0}", resComponent.DefaultPackageName);
            }
        }
    }
}