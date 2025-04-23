
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using YooAsset;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 初始化资源包流程
    /// </summary>
    public sealed class ProcedureInitPackage : ProcedureBase
    {
        /// <summary>
        /// 资源模块初始化模式数据字段
        /// </summary>
        public const string RES_MANAGER_INIT_MODE = "RES_MANAGER_INIT_MODE";



        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            VarInt32 initType = procedureOwner.GetData<VarInt32>(RES_MANAGER_INIT_MODE);

            YooAssets.Initialize();
            ResourcePackage package = YooAssets.TryGetPackage("DefaultPackage");
            if (package == null)
            {
                package = YooAssets.CreatePackage("DefaultPackage");
            }

            YooAssets.SetDefaultPackage(package);

            
        }
    }
}