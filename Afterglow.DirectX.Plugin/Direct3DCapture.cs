﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Afterglow.Core.Plugins;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using Afterglow.Core;
using Afterglow.Core.Configuration;
using System.ComponentModel.DataAnnotations;
using Capture;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel.Composition;

namespace Afterglow.DirectX.Plugin
{
    [DataContract]
    [Export(typeof(ICapturePlugin))]
    public class Direct3DCapture : BasePlugin, ICapturePlugin
    {
        [DataMember]
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        public override string Name
        {
            get { return "Direct3D Capture"; }
        }

        [DataMember]
        /// <summary>
        /// The author of this plugin
        /// </summary>
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }
        
        [DataMember]
        /// <summary>
        /// A description of this plugin
        /// </summary>
        public override string Description
        {
            get { return "An Afterglow capture plugin for Direct3D 9/10/11 applications"; }
        }

        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Target application executable name")]
        public string Target
        {
            get { return Get(() => Target); }
            set { Set(() => Target, value); }
        }

        private String _channelName = null;
        private Capture.Interface.CaptureInterface _captureInterface;
        private CaptureProcess _capturedProcess;
        [XmlIgnore]
        public string TargetProcess { get; set;  }
        private volatile bool _stopped = false;
        private global::Capture.Interface.Screenshot _currentResponse;
        private DateTime _startTime;
        private long _captures;
        
        public override void Start()
        {
            _captureInterface = new Capture.Interface.CaptureInterface();
            _captureInterface.RemoteMessage += (message) => Debug.WriteLine(message.ToString());

            // Inject to process
            this.TargetProcess = this.Target;

            if (String.IsNullOrEmpty(this.TargetProcess))
            {
                //Logger.Warn("Configuration: No executable name has been provided to capture");
                return;
            }

            Inject();
            _stopped = false;

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

                    _processId = process.Id;
                    _process = process;

                    _capturedProcess = new CaptureProcess(process, new Capture.Interface.CaptureConfig{
                        ShowOverlay = false, Direct3DVersion = global::Capture.Interface.Direct3DVersion.AutoDetect
                    }, _captureInterface);

                    newInstanceFound = true;
                    break;
                }
                Thread.Sleep(10);
            }
        }
        
        #endregion

        public override void Stop()
        {
            _stopped = true;
            if (_capturedProcess != null)
            {
                _capturedProcess.CaptureInterface.Disconnect();
                _capturedProcess.Dispose();
            }
        }

        private FastBitmap _fastBitmap;
        private Bitmap _capturedImage;
        public IDictionary<Core.Light, Core.PixelReader> Capture(ILightSetupPlugin lightSetup)
        {
            IDictionary<Core.Light, Core.PixelReader> dictionary = new Dictionary<Core.Light, Core.PixelReader>();

            Capture.Interface.Screenshot response = null;

            if (_capturedProcess != null && _capturedProcess.CaptureInterface != null)
            {
                _capturedProcess.CaptureInterface.GetScreenshot();
            }

            if (response != null)
            {
                Interlocked.Increment(ref _captures);
            }

            Interlocked.Exchange(ref _currentResponse, response);
            
            if (_currentResponse != null)
            {    
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(_currentResponse.CapturedBitmap))
                    _capturedImage = new Bitmap(ms);
                _fastBitmap = new FastBitmap(_capturedImage);

                foreach (var light in lightSetup.GetLightsForBounds(_capturedImage.Width, _capturedImage.Height, 0, 0))
                {
                    dictionary[light] = new PixelReader(_fastBitmap, light.Region);
                }
            }

            if (_captures > 0 && _captures % 10 == 0)
            {
                AfterglowRuntime.Logger.Debug("Time per capture: {0}", new TimeSpan(0, 0, 0, 0, (int)((DateTime.Now - _startTime).TotalMilliseconds / _captures)));
            }

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
