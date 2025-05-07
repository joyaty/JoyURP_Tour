
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Joy.Base.Event;
using UnityEngine;
using UnityGameFramework;
using UnityGameFramework.Runtime;

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
            OpenHotfixUIPanel();
            InitializePackage(procedureOwner).Forget();
        }

        /// <summary>
        /// 初始化资源包
        /// </summary>
        /// <param name="procedureOwner"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        private async UniTaskVoid InitializePackage(IFsm<IProcedureManager> procedureOwner)
        {
            AssetComponent resComponent = GameEntry.GetComponent<AssetComponent>();
            bool isSuccess = await resComponent.InitializePackage(resComponent.DefaultPackageName);
            if (isSuccess)
            {
                LogUtil.Debug("初始化资源包成功，PackageName = {0}，Frame = {1}", resComponent.DefaultPackageName, Time.frameCount);
                // 通知外部资源包初始化完成
                EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
                eventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(Define.EnumHotfixKeyPoint.PACKAGE_INIT_OVER));
                // 给表现层一段进度更新时间
                await UniTask.Delay(500);
                // 切换到资源版本号校验节点
                ChangeState<ProcedureRequestVersion>(procedureOwner);
            }
            else
            {
                LogUtil.Error("初始化资源包失败，PackageName = {0}", resComponent.DefaultPackageName);
                // TODO 弹窗重试或者退出
            }
        }

        // 打开热更新UI
        private void OpenHotfixUIPanel()
        {
            // TODO 使用UI框架打开热更新UI
            GameObject canvas = GameObject.Find("Canvas");
            GameObject hotfixUI = Resources.Load<GameObject>("HotfixPanel/Prefabs/UI_HotfixPanel");
            GameObject.Instantiate(hotfixUI, canvas.transform);
        }
    }
}