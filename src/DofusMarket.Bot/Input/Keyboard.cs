using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using DofusMarket.Bot.Logging;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Input;

internal static class Keyboard
{
    private static readonly ILogger Logger = LoggerProvider.CreateLogger(typeof(Keyboard));
    private static readonly Dictionary<string, VIRTUAL_KEY> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DOWN"] = VIRTUAL_KEY.VK_DOWN,
        ["ENTER"] = VIRTUAL_KEY.VK_RETURN,
        ["ESC"] = VIRTUAL_KEY.VK_ESCAPE,
        ["LEFT"] = VIRTUAL_KEY.VK_LEFT,
        ["RIGHT"] = VIRTUAL_KEY.VK_RIGHT,
        ["SPACE"] = VIRTUAL_KEY.VK_SPACE,
        ["TAB"] = VIRTUAL_KEY.VK_TAB,
        ["UP"] = VIRTUAL_KEY.VK_UP,
    };

    public static void Send(string keys)
    {
        Logger.LogDebug($"{nameof(Keyboard)}.{nameof(Send)}(\"{keys}\")");
        Send(keys, textOnlyMode: false);
    }

    public static void SendText(string text)
    {
        Logger.LogDebug($"{nameof(Keyboard)}.{nameof(SendText)}(REDACTED)");
        Send(text, textOnlyMode: true);
    }

    private static void Send(string keys, bool textOnlyMode)
    {
        var keyList = ParseKeys(keys, textOnlyMode);
        if (keyList.Count == 0)
        {
            return;
        }

        uint sentEvents = PInvoke.SendInput(CollectionsMarshal.AsSpan(keyList), Marshal.SizeOf<INPUT>());
        Win32Helper.ThrowIfZero(sentEvents, nameof(PInvoke.SendInput), true);
    }

    private static List<INPUT> ParseKeys(string keys, bool textOnlyMode)
    {
        var keyboardLayout = PInvoke.GetKeyboardLayout(0);

        List<INPUT> keyList = new();
        for (int i = 0; i < keys.Length; i += 1)
        {
            char c = keys[i];

            if (!textOnlyMode && c == '{')
            {
                int closingBraceIdx = keys.IndexOf('}', i);
                if (closingBraceIdx == -1)
                {
                    throw new ArgumentException($"Unclosed curly brace at position {i}", nameof(keys));
                }

                string bracesContent = keys.Substring(i + 1, closingBraceIdx - i - 1);
                var match = Regex.Match(bracesContent, "^(\\w+)(?: (\\d+))?$");
                if (!match.Success)
                {
                    throw new ArgumentException($"Unexpected braces content \"{bracesContent}\" at position {i}");
                }

                string keyword = match.Groups[1].Value;
                if (!Keywords.TryGetValue(keyword, out var virtualKey))
                {
                    throw new ArgumentException($"Unknown keyword '{keyword}'", nameof(keys));
                }

                int repeat = match.Groups[2] is { Success: true } repeatGroup
                    ? int.Parse(repeatGroup.Value)
                    : 1;
                for (int r = 0; r < repeat; r += 1)
                {
                    keyList.Add(CreateKeyboardInput(virtualKey, 0));
                    keyList.Add(CreateKeyboardInput(virtualKey, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP));
                }

                i += bracesContent.Length + 1;
            }
            else
            {
                short keyCode = PInvoke.VkKeyScanEx(c, keyboardLayout);
                var virtualKey = (VIRTUAL_KEY)(keyCode & 0xff);
                var shiftStateVirtualKeys = ShiftStateToVirtualKeys((ShiftState)(byte)(keyCode >> 8));

                foreach (var shiftStateVirtualKey in shiftStateVirtualKeys)
                {
                    keyList.Add(CreateKeyboardInput(shiftStateVirtualKey, 0));
                }

                keyList.Add(CreateKeyboardInput(virtualKey, 0));
                keyList.Add(CreateKeyboardInput(virtualKey, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP));

                foreach (var shiftStateVirtualKey in shiftStateVirtualKeys)
                {
                    keyList.Add(CreateKeyboardInput(shiftStateVirtualKey, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP));
                }
            }
        }

        return keyList;
    }

    private static INPUT CreateKeyboardInput(VIRTUAL_KEY virtualKey, KEYBD_EVENT_FLAGS flags)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = new KEYBDINPUT
                {
                    wVk = virtualKey,
                    wScan = 0,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };
    }

    private static List<VIRTUAL_KEY> ShiftStateToVirtualKeys(ShiftState shiftState)
    {
        List<VIRTUAL_KEY> virtualKeys = new(3);
        if (shiftState.HasFlag(ShiftState.Shift))
        {
            virtualKeys.Add(VIRTUAL_KEY.VK_SHIFT);
        }
        if (shiftState.HasFlag(ShiftState.Ctrl))
        {
            virtualKeys.Add(VIRTUAL_KEY.VK_CONTROL);
        }
        if (shiftState.HasFlag(ShiftState.Alt))
        {
            virtualKeys.Add(VIRTUAL_KEY.VK_MENU);
        }

        return virtualKeys;
    }

    [Flags]
    private enum ShiftState : byte
    {
        Shift = 0x1,
        Ctrl = 0x2,
        Alt = 0x4,
    }
}