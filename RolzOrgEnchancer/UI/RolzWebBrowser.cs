﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using RolzOrgEnchancer.Interfaces;

namespace RolzOrgEnchancer.UI
{
    internal class RolzWebBrowser : WebBrowser, IRolzOrg
    {

        //
        // These static metods are required to configure ie settings
        //
        #region IE settings
        private static uint GetBrowserEmulationMode()
        {
            var browserVersion = 7;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                if (null != ieKey)
                {
                    var version = ieKey.GetValue("svcVersion");
                    if (null == version)
                    {
                        version = ieKey.GetValue("Version");
                        if (null == version)
                            throw new ApplicationException("Microsoft Internet Explorer is required!");
                    }
                    int.TryParse(version.ToString().Split('.')[0], out browserVersion);
                }
            }

            uint mode;
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 mode. Default value for Internet Explorer 10.
                    break;
                default:
                    // use IE11 mode by default
                    mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode. Default value for Internet Explorer 11.
                    break;
            }

            return mode;
        }

        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                string.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key != null) key.SetValue(appName, value, RegistryValueKind.DWord);
            }
        }

        public static void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (string.Compare(fileName, "devenv.exe", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(fileName, "XDesProc.exe", StringComparison.OrdinalIgnoreCase) == 0)
                return;

            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode()); // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
            SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1);
            SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0);
            SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1);
        }
        #endregion

        private const string INFRAME_PREFIX = "https://rolz.org/wiki/inframe";
        private const string ROOM_PREFIX = "https://rolz.org/dr?room=";
        private readonly Uri _inframeEntered = new Uri("https://rolz.org/wiki/inframe?w=help&n=index", UriKind.Absolute);
        private Uri _room;
        private volatile bool _roomEntered;

        private static void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // Ignore the error and suppress the error dialog box
            e.Handled = true;
        }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //should be never
            if (sender != this)
            {
                Program.Log("ERROR: Browser: Invalid sender");
                return;
            }
            if (null == Document) return;
            if (null == Document.Window) return;

            var completedUrl = e.Url;

            //almost not interested in child documents
            if (Document.Window.Frames != null && Document.Window.Frames.Count != 0)
            {
                if (completedUrl.Equals(_inframeEntered))
                {
                    Program.Log(string.Format("Browser: on DocumentCompleted: Room Entered - InFrame ({0})", completedUrl));
                    _roomEntered = true;
                }
                return;
            }

            //should be never
            if (Url != _room)
            {
                Program.Log(string.Format("ERROR: Browser: Invalid base_url ({0})", Url));
            }

            //set our error handler that will silence all java script errors
            Document.Window.Error += Window_Error;
            Program.Log(string.Format("Browser: on DocumentCompleted: Entering Room ({0})", Url));
        }

        private void OnNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //navigation inside iframe is not interesting
            if (e.TargetFrameName != "") return;

            var url = e.Url.ToString();
            if (url.StartsWith(INFRAME_PREFIX))
            {
                Program.Log(string.Format("Browser: on prefix Navigating: url= {0}", url));
                return;
            }
            if (url == "about:blank")
            {
                Program.Log(string.Format("Browser: on blank Navigating: url= {0}", url));
                return;
            }
            if (url == _room.ToString())
            {
                Program.Log(string.Format("Browser: on room Navigating: url= {0}", url));
                return;
            }
            Program.Log(string.Format("Browser: CANCEL on Navigating: url= {0}", url));
            e.Cancel = true;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // RolzWebBrowser
            //
            Dock = DockStyle.Fill;
            IsWebBrowserContextMenuEnabled = false;
            TabStop = false;
            DocumentCompleted += OnDocumentCompleted;
            Navigating += OnNavigating;
            ResumeLayout(false);
        }

        public RolzWebBrowser()
        {
            InitializeComponent();
        }

        #region IRolzOrg interface
        public void JoinRoom(string roomName)
        {
            _room = new Uri(ROOM_PREFIX + roomName, UriKind.Absolute);
            _roomEntered = false;
            Url = _room;
        }

        public bool RoomEntered()
        {
            return _roomEntered;
        }

        public void SendMessage(string message)
        {
            if (Document != null) Document.InvokeScript("eval", new object[] { string.Format("window.sendLine('{0}');", message) });
        }
        #endregion

    }
}
