
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine.SceneManagement;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 游戏开始流程
    /// </summary>
    public sealed class ProcedureGameStart : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            LoadGameMainScene().Forget();
        }

        private async UniTask LoadGameMainScene()
        {
            AssetComponent assetComponent = GameEntry.GetComponent<AssetComponent>();
            assetComponent.LoadSceneSync("GameMain", LoadSceneMode.Single);
            await UniTask.DelayFrame(10);
        }
    }
}