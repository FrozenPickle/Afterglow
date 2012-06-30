using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Afterglow.Core.Plugins;
using System.Runtime.Remoting.Channels.Ipc;
using EasyHook;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using Afterglow.Core;
using Afterglow.DirectX.ScreenshotInterface;
using Afterglow.Core.Configuration;
using Afterglow.Core.Storage;

namespace Afterglow.DirectX.Plugin
{
    public class Direct3DCapture : BasePlugin, ICapturePlugin
    {
        public Direct3DCapture()
        {

        }

        public Direct3DCapture(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        public override string Name
        {
            get { return "Direct3D Capture"; }
        }

        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        public override string Description
        {
            get { return "An Afterglow capture plugin for Direct3D 9/10/11 applications"; }
        }

        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        [ConfigString(DisplayName = "Target application executable name", IsHidden = false)]
        public string Target
        {
            get { return Get(() => Target); }
            set { Set(() => Target, value); }
        }

        private String _channelName = null;
        private IpcServerChannel _screenshotServer;

        public string TargetProcess { get; set;  }
        private Task _screenshotPump = null;
        private volatile bool _stopped = false;
        private ScreenshotResponse _currentResponse;
        private DateTime _startTime;
        private int _captures;
        public override void Start()
        {
            _screenshotServer = RemoteHooking.IpcCreateServer<ScreenshotInterface.ScreenshotInterface>(
                ref _channelName,
                WellKnownObjectMode.Singleton);

            // Inject to process
            this.TargetProcess = this.Target;

            if (String.IsNullOrEmpty(this.TargetProcess))
            {
                Logger.Warn("Configuration: No executable name has been provided to capture");
                return;
            }

            ScreenshotManager.OnScreenshotDebugMessage += (pid, message) => Debug.WriteLine("{0}: {1}", pid, message);

            Inject();
            _stopped = false;
            _screenshotPump = new Task(() =>
                                           {
                                               _startTime = DateTime.Now;
                                               while(!_stopped)
                                               {
                                                   var response = ScreenshotManager.GetScreenshotSynchronous(_processId,
                                                                                              new ScreenshotRequest(
                                                                                                  Rectangle.Empty));
                                                   if (response != null)
                                                       Interlocked.Increment(ref _captures);
                                                   Interlocked.Exchange(ref _currentResponse, response);
                                                   Thread.Sleep(5);
                                               }
                                           }, TaskCreationOptions.LongRunning);
            _screenshotPump.Start();
        }

        #region Injection methods
        int _processId = 0;
        Process _process;
        private void Inject()
        {
            bool newInstanceFound = false;

            while (!newInstanceFound)
            {
                if (_stopped) break;
                Process[] processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(this.TargetProcess));
                foreach (Process process in processes)
                {
                    // Simply attach to the first one found.

                    // If the process doesn't have a mainwindowhandle yet, skip it (we need to be able to get the hwnd to set foreground etc)
                    if (process.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }

                    // Keep track of hooked processes in case more than one need to be hooked
                    HookManager.AddHookedProcess(process.Id);

                    _processId = process.Id;
                    _process = process;

                    // Ensure the target process is in the foreground,
                    // this prevents an issue where the target app appears to be in 
                    // the foreground but does not receive any user inputs.
                    // Note: the first Alt+Tab out of the target application after injection
                    //       may still be an issue - switching between windowed and 
                    //       fullscreen fixes the issue however (see ScreenshotInjection.cs for another option)
                    BringProcessWindowToFront(process);
                    
                    // Inject DLL into target process
                    RemoteHooking.Inject(
                        process.Id,
                        InjectionOptions.Default,
                        typeof(ScreenshotInjection).Assembly.Location, // 32-bit version (the same because AnyCPU) could use different assembly that links to 32-bit C++ helper dll
                        typeof(ScreenshotInjection).Assembly.Location, // 64-bit version (the same because AnyCPU) could use different assembly that links to 64-bit C++ helper dll
                        // the optional parameter list...
                        _channelName // The name of the IPC channel for the injected assembly to connect to
                        , "AutoDetect"
                        , false
                        );

                    newInstanceFound = true;
                    break;
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Bring the target window to the front and wait for it to be visible
        /// </summary>
        /// <remarks>If the window does not come to the front within approx. 30 seconds an exception is raised</remarks>
        private void BringProcessWindowToFront(Process process)
        {
            if (process == null)
                return;
            IntPtr handle = process.MainWindowHandle;
            int i = 0;

            while (!NativeMethods.IsWindowInForeground(handle))
            {
                if (i == 0)
                {
                    // Initial sleep if target window is not in foreground - just to let things settle
                    Thread.Sleep(250);
                }

                if (NativeMethods.IsIconic(handle))
                {
                    // Minimized so send restore
                    NativeMethods.ShowWindow(handle, NativeMethods.WindowShowStyle.Restore);
                }
                else
                {
                    // Already Maximized or Restored so just bring to front
                    NativeMethods.SetForegroundWindow(handle);
                }
                Thread.Sleep(250);

                // Check if the target process main window is now in the foreground
                if (NativeMethods.IsWindowInForeground(handle))
                {
                    // Leave enough time for screen to redraw
                    Thread.Sleep(1000);
                    return;
                }

                // Prevent an infinite loop
                if (i > 120) // about 30secs
                {
                    throw new Exception("Could not set process window to the foreground");
                }
                i++;
            }
        }
        #endregion

        public override void Stop()
        {
            _stopped = true;
            if (_screenshotServer != null)
            {
                _screenshotServer.StopListening(null);
                _screenshotServer = null;
            }
            _channelName = null;
        }

        private FastBitmap _fastBitmap;
        private Bitmap _capturedImage;
        public IDictionary<Core.Light, Core.PixelReader> Capture(ILightSetupPlugin lightSetup)
        {
            IDictionary<Core.Light, Core.PixelReader> dictionary = new Dictionary<Core.Light, Core.PixelReader>();

            if (_currentResponse != null)
            {                
                _capturedImage = _currentResponse.CapturedBitmapAsImage;
                _fastBitmap = new FastBitmap(_capturedImage);

                foreach (var light in lightSetup.GetLightsForBounds(_capturedImage.Width, _capturedImage.Height, 0, 0))
                {
                    dictionary[light] = new PixelReader(_fastBitmap, light.Region);
                }
            }
            if (_captures > 0 && _captures % 10 == 0)
                Console.WriteLine("Time per capture: {0}", new TimeSpan(0, 0, 0, 0, (int)(DateTime.Now - _startTime).TotalMilliseconds / _captures));

            return dictionary;
        }

        public void ReleaseCapture()
        {
            if (_fastBitmap != null)
            {
                _fastBitmap.Dispose();
                _fastBitmap = null;
            }

            if (_capturedImage != null)
            {
                _capturedImage.Dispose();
                _capturedImage = null;
            }
        }
    }

    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal static class NativeMethods
    {
        internal static bool IsWindowInForeground(IntPtr hWnd)
        {
            return hWnd == GetForegroundWindow();
        }

        #region user32

        #region ShowWindow
        /// <summary>Shows a Window</summary>
        /// <remarks>
        /// <para>To perform certain special effects when showing or hiding a
        /// window, use AnimateWindow.</para>
        ///<para>The first time an application calls ShowWindow, it should use
        ///the WinMain function's nCmdShow parameter as its nCmdShow parameter.
        ///Subsequent calls to ShowWindow must use one of the values in the
        ///given list, instead of the one specified by the WinMain function's
        ///nCmdShow parameter.</para>
        ///<para>As noted in the discussion of the nCmdShow parameter, the
        ///nCmdShow value is ignored in the first call to ShowWindow if the
        ///program that launched the application specifies startup information
        ///in the structure. In this case, ShowWindow uses the information
        ///specified in the STARTUPINFO structure to show the window. On
        ///subsequent calls, the application must call ShowWindow with nCmdShow
        ///set to SW_SHOWDEFAULT to use the startup information provided by the
        ///program that launched the application. This behavior is designed for
        ///the following situations: </para>
        ///<list type="">
        ///    <item>Applications create their main window by calling CreateWindow
        ///    with the WS_VISIBLE flag set. </item>
        ///    <item>Applications create their main window by calling CreateWindow
        ///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the
        ///    SW_SHOW flag set to make it visible.</item>
        ///</list></remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">Specifies how the window is to be shown.
        /// This parameter is ignored the first time an application calls
        /// ShowWindow, if the program that launched the application provides a
        /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
        /// the value should be the value obtained by the WinMain function in its
        /// nCmdShow parameter. In subsequent calls, this parameter can be one of
        /// the WindowShowStyle members.</param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero.
        /// If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        /// <summary>Enumeration of the different ways of showing a window using
        /// ShowWindow</summary>
        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag when restoring
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }
        #endregion

        /// <summary>
        /// The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsIconic(IntPtr hWnd);

        #endregion
    }
}
