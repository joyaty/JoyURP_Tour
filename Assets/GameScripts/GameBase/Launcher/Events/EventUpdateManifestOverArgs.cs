
using GameFramework;
using GameFramework.Event;

namespace Joy.Base.Event
{
    /// <summary>
    /// 资源模块资源包清单文件更新结束事件参数
    /// </summary>
    public sealed class EventUpdateManifestOverArgs : GameEventArgs
    {
        private static readonly int EventId = typeof(EventUpdateManifestOverArgs).GetHashCode();

        public static EventUpdateManifestOverArgs Create(bool isSuccess)
        {
            EventUpdateManifestOverArgs eventUpdateManifestOver = ReferencePool.Acquire<EventUpdateManifestOverArgs>();
            eventUpdateManifestOver.IsSuccess = isSuccess;
            return eventUpdateManifestOver;
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