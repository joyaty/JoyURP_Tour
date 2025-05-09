
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
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

        private async UniTaskVoid LoadGameMainScene()
        {
            AssetComponent assetComponent = GameEntry.GetComponent<AssetComponent>();
            await assetComponent.LoadSceneAsync("GameMain", LoadSceneMode.Single);
            GameObject obj = await assetComponent.InstantiateGameObject("Craft");
            RotaAnimation rotaAnim = obj.GetComponent<RotaAnimation>();
            rotaAnim.transform.position = new Vector3(0.0f, 0.0f, 1.0f);
        }
    }
}