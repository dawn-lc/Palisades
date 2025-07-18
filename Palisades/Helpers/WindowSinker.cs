﻿using Palisades.Helpers.Native;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Palisades.Helpers
{
    internal partial class WindowSinker : IDisposable
    {
        #region Windows API

        private const int WM_WINDOWPOSCHANGING = 0x0046;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        private static readonly IntPtr HWND_BOTTOM = new(1);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            uint uFlags);

        #endregion

        #region Fields

        private readonly Window window;
        private bool disposed;

        #endregion

        internal WindowSinker(Window window)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));

            if (window.IsLoaded)
            {
                OnWindowLoaded(window, null);
            }
            else
            {
                window.Loaded += OnWindowLoaded;
            }

            window.Closing += OnWindowClosing;
        }

        #region Dispose Pattern

        ~WindowSinker()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // 安全解绑 UI 事件
                if (window.Dispatcher.CheckAccess())
                {
                    window.Loaded -= OnWindowLoaded;
                    window.Closing -= OnWindowClosing;
                }
                else
                {
                    window.Dispatcher.Invoke(() =>
                    {
                        window.Loaded -= OnWindowLoaded;
                        window.Closing -= OnWindowClosing;
                    });
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Event Handlers

        private void OnWindowLoaded(object? sender, RoutedEventArgs? e)
        {
            SetWindowPos(new WindowInteropHelper(window).Handle, HWND_BOTTOM, 0, 0, 0, 0,
                SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

            var source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            source?.AddHook(WndProc);
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            source?.RemoveHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_WINDOWPOSCHANGING)
            {
                var windowPos = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                windowPos.flags |= SWP_NOZORDER;
                Marshal.StructureToPtr(windowPos, lParam, false);
            }

            return IntPtr.Zero;
        }

        #endregion

        #region Attached Properties

        private static readonly DependencyProperty SinkerProperty = DependencyProperty.RegisterAttached(
            "Sinker",
            typeof(WindowSinker),
            typeof(WindowSinker),
            null);

        public static readonly DependencyProperty AlwaysOnBottomProperty = DependencyProperty.RegisterAttached(
            "AlwaysOnBottom",
            typeof(bool),
            typeof(WindowSinker),
            new UIPropertyMetadata(false, OnAlwaysOnBottomChanged));

        public static WindowSinker? GetSinker(DependencyObject d)
        {
            return (WindowSinker?)d.GetValue(SinkerProperty);
        }

        private static void SetSinker(DependencyObject d, WindowSinker? value)
        {
            d.SetValue(SinkerProperty, value);
        }

        public static bool GetAlwaysOnBottom(DependencyObject d)
        {
            return (bool)d.GetValue(AlwaysOnBottomProperty);
        }

        public static void SetAlwaysOnBottom(DependencyObject d, bool value)
        {
            d.SetValue(AlwaysOnBottomProperty, value);
        }

        private static void OnAlwaysOnBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Window window)
            {
                if ((bool)e.NewValue)
                {
                    SetSinker(window, new WindowSinker(window));
                }
                else
                {
                    GetSinker(window)?.Dispose();
                    SetSinker(window, null);
                }
            }
        }

        #endregion
    }

    namespace Native
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
    }
}
