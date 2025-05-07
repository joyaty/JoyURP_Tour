
using System.Reflection;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    public sealed class ProcedureHotfixDLL : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            AssetComponent assetComponent = GameEntry.GetComponent<AssetComponent>();
            Assembly assembly = Assembly.Load("");


            // assembly.GetType()
        }
    }
}