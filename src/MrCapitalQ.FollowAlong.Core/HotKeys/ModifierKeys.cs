﻿using System;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    [Flags]
    public enum ModifierKeys
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        WinKey = 8
    }
}
