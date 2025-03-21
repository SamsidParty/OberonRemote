#if UNITY_STANDALONE_WIN

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowsTitlebar
{
    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, int[] pvAttribute, int cbAttribute);

    private const string UnityWindowClassName = "UnityWndClass";

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    private static IntPtr windowHandle = IntPtr.Zero;

    public static void SetTitlebarColor(IntPtr hwnd, int color)
    {
        DwmSetWindowAttribute(hwnd, 35, new int[] { color }, 4);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Apply()
    {
        uint threadId = GetCurrentThreadId();
        EnumThreadWindows(threadId, (hWnd, lParam) =>
        {
            var classText = new StringBuilder(UnityWindowClassName.Length + 1);
            GetClassName(hWnd, classText, classText.Capacity);
            if (classText.ToString() == UnityWindowClassName)
            {
                windowHandle = hWnd;
                return false;
            }
            return true;
        }, IntPtr.Zero);

        SetTitlebarColor(windowHandle, 0x211412);
    }
}

#endif