
using GameFramework.Procedure;
using Joy.Base.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Joy.Hotfix.Entry
{
    /// <summary>
    /// 游戏内容流程入口(热更新脚本)
    /// </summary>
    public sealed class GameMainEntry : MonoBehaviour
    {
        /// <summary>
        /// 热更程序集中的流程节点集合
        /// </summary>
        private readonly ProcedureBase[] m_HotfixProcedures = new ProcedureBase[]
        {
            new ProcedureGameStart()
        };

        private void Start()
        {
            ProcedureComponent procedureComponent = GameEntry.GetComponent<ProcedureComponent>();
            procedureComponent.ResetProcedures(m_HotfixProcedures);
            procedureComponent.StartProcedure<ProcedureGameStart>();
        }
    }
}