using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DofusMarket.Bot.Input;

internal static class Win32Helper
{
    public static void ThrowIfFalse(bool ret, string functionName, bool useLastError)
    {
        ThrowIfZero(ret ? 1 : 0, functionName, useLastError);
    }

    public static void ThrowIfZero<T>(T ret, string functionName, bool useLastError) where T : INumberBase<T>
    {
        if (!T.IsZero(ret))
        {
            return;
        }

        if (useLastError)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Win32Exception win32Exception = new(errorCode);
            throw new Win32Exception(errorCode, $"{functionName}: {win32Exception.Message}");
        }

        throw new Win32Exception($"{functionName} returned {ret}");
    }
}