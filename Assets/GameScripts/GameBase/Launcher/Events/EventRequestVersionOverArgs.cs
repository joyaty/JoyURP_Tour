
using GameFramework;
using GameFramework.Event;

namespace Joy.Base.Event
{
    /// <summary>
    /// 资源模块资源包版本获取结束事件参数
    /// </summary>
    public sealed class EventRequestVersionOverArgs : GameEventArgs
    {
        private static readonly int EventId = typeof(EventRequestVersionOverArgs).GetHashCode();

        public static EventRequestVersionOverArgs Create(bool isSuccess, string versionCode)
        {
            EventRequestVersionOverArgs eventRequestVersionOver = ReferencePool.Acquire<EventRequestVersionOverArgs>();
            eventRequestVersionOver.IsSuccess = isSuccess;
            eventRequestVersionOver.VersionCode = versionCode;
            return eventRequestVersionOver;
        }

        public override int Id => EventId;

        public bool IsSuccess
        {
            get;
            private set;
        }

        public string VersionCode
        {
            get;
            private set;
        }

        public override void Clear()
        {
            IsSuccess = false;
            VersionCode = null;
        }

    }
}