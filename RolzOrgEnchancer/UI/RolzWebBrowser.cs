using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RolzOrgEnchancer
{
    class RolzWebBrowser : WebBrowser, IRolzOrg
    {
        //
        // These static metods are required to configure ie settings
        //
        static private UInt32 GetBrowserEmulationMode()
        {
            int browserVersion = 7;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
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

            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode. Default value for Internet Explorer 11.
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
                    break;
            }

            return mode;
        }
        static private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }
        static public void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
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

        private const string inframe_prefix = "https://rolz.org/wiki/inframe";
        private const string room_prefix = "https://rolz.org/dr?room=";
        private Uri inframe_entered = new Uri("https://rolz.org/wiki/inframe?w=help&n=index", System.UriKind.Absolute);
        private Uri room;
        private volatile bool room_entered = false;

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // Ignore the error and suppress the error dialog box 
            e.Handled = true;
        }

        private void onDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //should be never
            if (sender != this)
            {
                Program.Log("ERROR: Invalid sender");
            }

            Uri base_url = this.Url;
            Uri completed_url = e.Url;
            
            //almost not interested in child documents
            if (this.Document.Window.Frames.Count != 0)
            {
                if (completed_url.Equals(inframe_entered))
                {
                    Program.Log("on DocumentCompleted: Room Entered - InFrame (" + completed_url.ToString() + ")");
                }
                room_entered = true;
                return;
            }

            //should be never
            if (base_url != this.room)
            {
                Program.Log("ERROR: Invalid base_url (" + base_url.ToString() + ")");
            }

            //set our error handler that will silence all java script errors
            ((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);

            Program.Log("on DocumentCompleted: Entering Room (" + base_url.ToString() + ")");
        }

        private void onNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //navigation inside iframe is not interesting
            if (e.TargetFrameName == "") 
            {
                string url = e.Url.ToString();
                if (url.StartsWith(inframe_prefix))
                {
                    Program.Log("on prefix Navigating: url= " + url);
                    return;
                }
                if (url == "about:blank")
                {
                    Program.Log("on blank Navigating: url= " + url);
                    return;
                }
                if (url == room.ToString())
                {
                    Program.Log("on room Navigating: url= " + url);
                    return;
                }
                Program.Log("CANCEL on Navigating: url= " + url);
                e.Cancel = true;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RolzWebBrowser
            // 
            this.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.onDocumentCompleted);
            this.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.onNavigating);
            this.ResumeLayout(false);
        }

        public RolzWebBrowser() : base()
        {
            InitializeComponent();
        }

        public void JoinRoom(string room_name)
        {
            this.room = new System.Uri("https://rolz.org/dr?room=" + room_name, System.UriKind.Absolute);
            this.inframe_entered = new System.Uri("https://rolz.org/wiki/inframe?w=help&n=index", System.UriKind.Absolute);
            this.room_entered = false;
            this.Url = room;
        }

        public bool RoomEntered()
        {
            return room_entered;
        }

        public void SendMessage(string message)
        {
            this.Document.InvokeScript("eval", new object[] { "window.sendLine('" + message + "');" });
        }

        public void SendSystemMessage(string message)
        {
            this.Document.InvokeScript("eval", new object[] { "window.sendLine('red:" + message + "');" });
        }

    }
}
