
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Joy.Hotfix.Entry
{
    /// <summary>
    /// 游戏流程入口(热更新脚本)
    /// </summary>
    public sealed class GameMainEntry : MonoBehaviour
    {
        private ProcedureComponent m_ProcedureComponent = null;

        private void Start()
        {
            m_ProcedureComponent = GameEntry.GetComponent<ProcedureComponent>();
        }
    }
}