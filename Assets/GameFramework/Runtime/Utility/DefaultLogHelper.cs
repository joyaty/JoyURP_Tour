//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 默认游戏框架日志辅助器。
    /// </summary>
    internal class DefaultLogHelper : ILogHelper, YooAsset.ILogger
    {
        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="message">日志内容。</param>
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:
                    Debug.Log(Utility.Text.Format("<color=#888888>{0}</color>", message));
                    break;

                case GameFrameworkLogLevel.Info:
                    Debug.Log(message.ToString());
                    break;

                case GameFrameworkLogLevel.Warning:
                    Debug.LogWarning(message.ToString());
                    break;

                case GameFrameworkLogLevel.Error:
                    Debug.LogError(message.ToString());
                    break;

                default:
                    throw new GameFrameworkException(message.ToString());
            }
        }

        #region YooAssets插件接口定义的调试信息初始接口

        /// <summary>
        /// 输出调试级别日志输出
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            LogUtil.Info(message);
        }

        /// <summary>
        /// 输出警告级别日志输出
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            LogUtil.Warning(message);
        }

        /// <summary>
        /// 输出错误级别日志信息
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            LogUtil.Error(message);
        }

        /// <summary>
        /// 输出异常级别日志信息
        /// </summary>
        /// <param name="exception"></param>
        public void Exception(Exception exception)
        {
            LogUtil.Fatal(exception);
        }
        
        #endregion
    }
}
