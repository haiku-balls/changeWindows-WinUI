using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace changeWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class settingsPage : Page
    {
        public settingsPage()
        {
            this.InitializeComponent();
        }

        // Clear build log button.
        private async void clearBuildLog_Click(object sender, RoutedEventArgs e)
        {
            clearBuildLogProgression.IsActive = true;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\";
            File.Delete(baseDirectory + "buildLog.xml");
            await Task.Delay(1000);
            clearBuildLogProgression.IsActive = false;
            Environment.Exit(0);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(homePage));
        }
    }
}
