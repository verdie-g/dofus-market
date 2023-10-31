using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using DofusMarket.Bot.Logging;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Input;

internal class Window
{
    private static readonly ILogger Logger = LoggerProvider.CreateLogger<Window>();

    public static Window WaitForWindow(string title, TimeSpan timeout)
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(WaitForWindow)}(\"{title}\", {timeout})");

        var iterationDelay = TimeSpan.FromMilliseconds(50);
        int maxIterations = (int)(timeout / iterationDelay);

        int i = 0;
        Window? window;
        do
        {
            window = GetWindow();
            Thread.Sleep(iterationDelay);
        } while (window == null && i < maxIterations);

        if (window == null)
        {
            throw new TimeoutException($"Window with title \"{title}\" was not found after {timeout}");
        }

        // Here we assume the window handle of the process never changes, but according to the doc, it is not guaranteed.
        return window;

        Window? GetWindow()
        {
            const int maxTitleSize = 256;

            HWND windowHandleMatch = HWND.Null;
            PInvoke.EnumWindows((handle, _) =>
            {
                char[] titleChars = new char[maxTitleSize];
                int titleCharsLength;
                unsafe
                {
                    fixed (char* titleCharsPtr = titleChars)
                    {
                        titleCharsLength = PInvoke.GetWindowText(handle, new PWSTR(titleCharsPtr), maxTitleSize);
                        if (titleCharsLength == 0)
                        {
                            // Skip the window in case of error.
                            return true;
                        }
                    }
                }

                string titleStr = new(titleChars, 0, titleCharsLength);
                if (titleStr.Contains(title, StringComparison.OrdinalIgnoreCase))
                {
                    windowHandleMatch = handle;
                    return false;
                }

                return true;
            }, new LPARAM(IntPtr.Zero));

            if (windowHandleMatch == HWND.Null)
            {
                return null;
            }

            return new Window(windowHandleMatch);
        }
    }

    private readonly HWND _handle;

    private Window(HWND handle)
    {
        _handle = handle;
    }

    public void Show()
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(Show)}()");

        PInvoke.ShowWindow(_handle, SHOW_WINDOW_CMD.SW_RESTORE);
    }

    public void Focus()
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(Focus)}()");

        bool ok = PInvoke.SetForegroundWindow(_handle);
        Win32Helper.ThrowIfZero(ok, nameof(PInvoke.SetForegroundWindow), true);
    }

    public void MoveWindow(Rectangle rec)
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(MoveWindow)}({rec})");

        bool ok = PInvoke.MoveWindow(_handle, rec.X, rec.Y, rec.Width, rec.Height, true);
        Win32Helper.ThrowIfZero(ok, nameof(PInvoke.MoveWindow), true);
    }

    public void MouseClick(Point clientPoint, int clickCount = 1)
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(MouseClick)}({clientPoint}, {clickCount})");

        Point screenPoint = ConvertPointFromClientToScreen(clientPoint);
        Mouse.Click(screenPoint, clickCount);
    }

    public void MouseScroll(Point clientPoint, int count)
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(MouseScroll)}({clientPoint}, {count})");

        Point screenPoint = ConvertPointFromClientToScreen(clientPoint);
        Mouse.Scroll(screenPoint, count);
    }

    public Color GetPixel(Point clientPoint)
    {
        Point screenPoint = ConvertPointFromClientToScreen(clientPoint);
        var color = Screen.GetPixel(screenPoint);
        Logger.LogDebug($"{nameof(Window)}.{nameof(GetPixel)}({clientPoint}) -> {color}");
        return color;
    }

    public void WaitForPixel(Point clientPoint, Color expectedColor, TimeSpan? timeout = default)
    {
        Logger.LogDebug($"{nameof(Window)}.{nameof(WaitForPixel)}({clientPoint}, {expectedColor}, {timeout})");

        var iterationDelay = TimeSpan.FromMilliseconds(50);
        int maxIterations = timeout.HasValue ? (int)(timeout / iterationDelay) : int.MaxValue;

        int i = 0;
        Color actualColor;
        do
        {
            actualColor = GetPixel(clientPoint);
            Thread.Sleep(iterationDelay);
        } while (actualColor != expectedColor && i < maxIterations);

        if (i >= maxIterations)
        {
            throw new TimeoutException($"Pixel ({clientPoint.X}, {clientPoint.Y}) color was not {expectedColor} after {timeout}");
        }
    }

    private Point ConvertPointFromClientToScreen(Point clientPoint)
    {
        Point screenPoint = clientPoint;
        bool ok = PInvoke.ClientToScreen(_handle, ref screenPoint);
        Win32Helper.ThrowIfZero(ok, nameof(PInvoke.ClientToScreen), false);
        return screenPoint;
    }
}