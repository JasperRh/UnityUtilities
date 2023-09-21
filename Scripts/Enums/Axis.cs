using System;

namespace Crimsilk.Utilities.Enums
{
    [Flags]
    public enum Axis
    {
        X = 1,
        Y = 2,
        Z = 4,
        All = ~0
    }
}