
using GameFramework;
using GameFramework.Event;

namespace Joy.Base.Event
{
    public sealed class EventDownloadInfoArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(EventDownloadInfoArgs).GetHashCode();

        public override int Id => EventId;

        /// <summary>
        /// 当前已下载的资源数
        /// </summary>
        public int CurrentDownloadCount { get; private set; }

        /// <summary>
        /// 总共需要下载的资源数
        /// </summary>
        public int TotalDownloadCount { get; private set; }

        /// <summary>
        /// 当前已下载的字节数
        /// </summary>
        public long CurrentDownloadBytes { get; private set; }

        /// <summary>
        /// 总共需要下载的字节数
        /// </summary>
        public long TotalDownloadBytes { get; private set; }

        public override void Clear()
        {
            CurrentDownloadCount = 0;
            TotalDownloadCount = 0;
            CurrentDownloadBytes = 0;
            TotalDownloadBytes = 0;
        }

        /// <summary>
        /// 创建资源下载信息同步事件消息
        /// </summary>
        /// <param name="currentDownloadCount">当前资源下载数</param>
        /// <param name="totalDownloadCount">总共资源下载数</param>
        /// <param name="currentDownloadBytes">当前下载字节数</param>
        /// <param name="totalDownloadBytes">总共资源下载数</param>
        /// <returns></returns>
        public static EventDownloadInfoArgs Create(int currentDownloadCount, int totalDownloadCount, long currentDownloadBytes, long totalDownloadBytes)
        {
            EventDownloadInfoArgs eventArgs = ReferencePool.Acquire<EventDownloadInfoArgs>();
            eventArgs.CurrentDownloadCount = currentDownloadCount;
            eventArgs.TotalDownloadCount = totalDownloadCount;
            eventArgs.CurrentDownloadBytes = currentDownloadBytes;
            eventArgs.TotalDownloadBytes = totalDownloadBytes;
            return eventArgs;
        }
    }
}