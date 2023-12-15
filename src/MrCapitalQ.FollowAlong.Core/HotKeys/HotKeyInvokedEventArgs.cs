using System;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    public class HotKeyInvokedEventArgs : EventArgs
    {
        public HotKeyInvokedEventArgs(HotKeyType hotKeyType) => HotKeyType = hotKeyType;

        public HotKeyType HotKeyType { get; }
    }
}
