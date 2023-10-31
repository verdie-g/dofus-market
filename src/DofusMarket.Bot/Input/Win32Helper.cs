using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DofusMarket.Bot.Input;

internal static class Win32Helper
{
    public static void ThrowIfZero(bool ret, string functionName, bool useLastError)
    {
        ThrowIfZero(ret ? 1 : 0, functionName, useLastError);
    }

    public static void ThrowIfZero<T>(T ret, string functionName, bool useLastError) where T : INumberBase<T>
    {
        if (!T.IsZero(ret))
        {
            return;
        }

        string message = $"{functionName} returned {ret}";
        if (useLastError)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorCode, message);
        }

        throw new Win32Exception(message);
    }
}