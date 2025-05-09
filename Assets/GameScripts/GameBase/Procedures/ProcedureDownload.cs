
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Joy.Base.Event;
using UnityGameFramework;
using UnityGameFramework.Runtime;

namespace Joy.Base.Procedure
{
    /// <summary>
    /// 远程资源下载流程节点
    /// </summary>
    public sealed class ProcedureDownload : ProcedureBase
    {
        /// <summary>
        /// 事件消息管理模块
        /// </summary>
        private EventComponent m_EventComponent;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            DownloadAsset(procedureOwner).Forget();
        }

        /// <summary>
        /// 启动下载远端游戏资源
        /// </summary>
        /// <param name="procedureOwner"></param>
        /// <returns></returns>
        private async UniTaskVoid DownloadAsset(IFsm<IProcedureManager> procedureOwner)
        {
            // 通知下载开始
            m_EventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(Define.EnumHotfixKeyPoint.ASSET_DOWNLOAD_BEGIN));
            AssetComponent assetComponent = GameEntry.GetComponent<AssetComponent>();
            bool isSuccess = await assetComponent.StartDownloadResource(OnDownloadProgress);
            if (isSuccess)
            {
                m_EventComponent.FireNow(this, EventHotfixProcessSyncArgs.Create(Define.EnumHotfixKeyPoint.ASSET_DOWNLOAD_OVER));
                // 切换到资源清理流程
                ChangeState<ProcedureUnuseResClean>(procedureOwner);
            }
            else
            {
                LogUtil.Error("游戏资源下载失败!");
            }
        }

        /// <summary>
        /// 下载过程信息回调
        /// </summary>
        /// <param name="currentDownloadCount"></param>
        /// <param name="totalDownloadCount"></param>
        /// <param name="currentDownloadBytes"></param>
        /// <param name="totalDownloadBytes"></param>
        private void OnDownloadProgress(int currentDownloadCount, int totalDownloadCount, long currentDownloadBytes, long totalDownloadBytes)
        {
            m_EventComponent.FireNow(this, EventDownloadInfoArgs.Create(currentDownloadCount, totalDownloadCount, currentDownloadBytes, totalDownloadBytes));
        }
    }
}