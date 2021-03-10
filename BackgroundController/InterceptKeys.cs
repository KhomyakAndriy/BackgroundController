using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace BackgroundController
{
    public enum KeyState
    {
        Up = 0,
        Down = 1
    }
    public class InterceptKeys
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        public static readonly int WH_KEYBOARD_LL = 13;
        public static readonly int WM_KEYDOWN = 0x100;
        public static readonly int WM_KEYUP = 0x101;
        public static readonly int WM_SYSKEYDOWN = 0x104;
        public static readonly int WM_SYSKEYUP = 0x105;
        private static IntPtr _hookID = IntPtr.Zero;

        private static CancellationTokenSource _cancellationTokenSource = null;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static LowLevelKeyboardProc _proc = HookCallback;

        private static Dictionary<Keys,KeyState> _keyStates = new Dictionary<Keys, KeyState>();
        private static List<Keys> _keys = new List<Keys>();

        private static readonly Background _controller = new Background();

        public InterceptKeys()
        {
            var keys = Enum.GetValues(typeof(Keys)).Cast<Keys>();
            foreach (var key in keys)
            {
                if (!_keyStates.ContainsKey(key))
                {
                    _keyStates.Add(key, KeyState.Up);
                    _keys.Add(key);
                }
            }
        }

        public void SetHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                Keys vkCode = (Keys)Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (_keyStates.ContainsKey(vkCode))
                {
                    _keyStates[vkCode] = KeyState.Down;
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                Keys vkCode = (Keys)Marshal.ReadInt32(lParam);
                if (_keyStates.ContainsKey(vkCode))
                {
                    _keyStates[vkCode] = KeyState.Up;
                }
            }

            if (
                _keyStates[Keys.Return] == KeyState.Down &&
                _keyStates[Keys.Add] == KeyState.Down 
            )
            {
                _controller.SetNext();
            }

            if (
                _keyStates[Keys.Return] == KeyState.Down &&
                _keyStates[Keys.Subtract] == KeyState.Down
            )
            {
                _controller.RestoreOriginal();
                _keys.ForEach(f => _keyStates[f] = KeyState.Up);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}