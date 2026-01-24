using System;

namespace Crimsilk.Utilities.Enums
{
    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = ~0
    }
}