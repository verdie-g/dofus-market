using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace DofusMarket.Bot.Input;

internal static class Screen
{
    public static Color GetPixel(Point point)
    {
        var deviceContextHandle = PInvoke.GetDC(HWND.Null);
        Win32Helper.ThrowIfZero(deviceContextHandle.Value, nameof(PInvoke.GetDC), false);

        var colorRef = PInvoke.GetPixel(deviceContextHandle, point.X, point.Y);
        Win32Helper.ThrowIfZero(colorRef.Value != 0xFFFFFFFF, nameof(PInvoke.GetPixel), false);

        bool ok = PInvoke.ReleaseDC(HWND.Null, deviceContextHandle) == 1;
        Win32Helper.ThrowIfZero(ok, nameof(PInvoke.ReleaseDC), true);

        return Color.FromArgb(
            alpha: 255,
            red: (int)(colorRef.Value >> 0) & 0xff,
            green: (int)(colorRef.Value >> 8) & 0xff,
            blue: (int)(colorRef.Value >> 16) & 0xff);
    }
}