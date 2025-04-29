
using GameFramework.Fsm;
using GameFramework.Procedure;
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
            LogUtil.Debug("Game Start");
        }
    }
}