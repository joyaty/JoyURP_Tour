
using GameFramework;
using GameFramework.Event;
using Joy.Base.Define;

namespace Joy.Base.Event
{
    /// <summary>
    /// 资源热更新流程关键节点同步消息
    /// </summary>
    public sealed class EventHotfixProcessSyncArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(EventHotfixProcessSyncArgs).GetHashCode();

        public static EventHotfixProcessSyncArgs Create(EnumHotfixKeyPoint hotfixKeyPoint)
        {
            EventHotfixProcessSyncArgs eventInitPackageOver = ReferencePool.Acquire<EventHotfixProcessSyncArgs>();
            eventInitPackageOver.HotfixKeyPoint = hotfixKeyPoint;
            return eventInitPackageOver;
        }

        public override int Id => EventId;

        /// <summary>
        /// 资源热更新关键节点
        /// </summary>
        public EnumHotfixKeyPoint HotfixKeyPoint { get; private set; }

        public override void Clear()
        {
            HotfixKeyPoint = EnumHotfixKeyPoint.INVALID;
        }
    }
}