using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DofusMarket.Bot.Input;

internal static class Mouse
{
    public static void Click(Point point, int clickCount = 1)
    {
        SendMouseEvent(point, clickCount, 0,
            clickCount > 0 ? MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN | MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP : 0);
    }

    public static void Scroll(Point point, int count = 1)
    {
        const uint wheelDelta = 120;

        SendMouseEvent(point, 1, wheelDelta * (uint)count,
            MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL);
    }

    private static void SendMouseEvent(Point point, int count, uint mouseData, MOUSE_EVENT_FLAGS extraMouseEventFlags)
    {
        int screenWidth = PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSCREEN);
        int screenHeight = PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSCREEN);

        int dx = point.X * (65536 / screenWidth);
        int dy = point.Y * (65536 / screenHeight);

        var inputs = new INPUT[count == 0 ? 1 : count];
        for (int i = 0; i < inputs.Length; i += 1)
        {
            inputs[i] = new INPUT
            {
                type = INPUT_TYPE.INPUT_MOUSE,
                Anonymous = new INPUT._Anonymous_e__Union
                {
                    mi = new MOUSEINPUT
                    {
                        dx = dx,
                        dy = dy,
                        mouseData = mouseData,
                        dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE
                                  | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE
                                  | extraMouseEventFlags,
                        time = 0,
                        dwExtraInfo = 0,
                    },
                },
            };
        }

        uint sentEvents = PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        Win32Helper.ThrowIfZero(sentEvents, nameof(PInvoke.SendInput), true);
    }
}