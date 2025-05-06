
using GameFramework;
using GameFramework.Event;

namespace Joy.Base.Event
{
    /// <summary>
    /// 资源模块初始化资源包结构事件参数
    /// </summary>
    public sealed class EventInitPackageOverArgs : GameEventArgs
    {
        private static readonly int EventId = typeof(EventInitPackageOverArgs).GetHashCode();

        public static EventInitPackageOverArgs Create(bool isSuccess)
        {
            EventInitPackageOverArgs eventInitPackageOver = ReferencePool.Acquire<EventInitPackageOverArgs>();
            eventInitPackageOver.IsSuccess = isSuccess;
            return eventInitPackageOver;
        }

        public override int Id => EventId;

        public bool IsSuccess
        {
            get;
            private set;
        }

        public override void Clear()
        {
            IsSuccess = false;
        }

    }
}