using Microsoft.UI;           // Needed for WindowId
using Microsoft.UI.Windowing; // Needed for AppWindow
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace changeWindows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private OverlappedPresenter _presenter;

        private AppWindow m_AppWindow;

        // SYSTEM FUNCTION, GRABS THE CURRENT WINDOW. SEE MAINWINDOW.
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        public MainWindow()
        {

            this.InitializeComponent();

            // Inherited from winUISys.
            m_AppWindow = GetAppWindowForCurrentWindow();
            var titleBar = m_AppWindow.TitleBar;
            // Hide default title bar
            Title = "Windows SysInfo";

            // Allows XAML to "clip" into.
            titleBar.ExtendsContentIntoTitleBar = true;

            // This disables the ability to resize and maximize the window.
            // This is a temporary fix to preventing users to resizing the window causing most pages to scale terribly.
            _presenter = m_AppWindow.Presenter as OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;

            // Headless navigate
            contentFrame.Navigate(typeof(homePage));
        }
    }
}
