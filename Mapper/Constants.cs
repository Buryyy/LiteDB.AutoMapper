using System;
using System.Runtime.InteropServices;

namespace Mapper
{
    internal static class Constants
    {
       internal static readonly bool IsNetCore = RuntimeInformation.FrameworkDescription
            .StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);
    }
}