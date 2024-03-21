using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace changeWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    [DataContract]
    public class fetchedBuild // The system's fetched build.
    {
        public fetchedBuild(string buildNum, int installDate, string buildBranch, string buildTag)
        {
            this.buildNum = buildNum;
            this.installDate = installDate;
            this.buildBranch = buildBranch;
            this.buildTag = buildTag;
        }

        [DataMember]
        public string buildNum; // The build #

        [DataMember]
        public int installDate; // The install date (converted from UNIX)

        [DataMember]
        public string buildBranch; // The insider channel (dev; canary)

        [DataMember]
        public string buildTag; // The build tag (23020.ni_prerelease.240118-1056)

        public string getBuildNum()
        {
            return buildNum;
        }

        public int getInstallDate()
        {
            return installDate; // ?
        }

        public string getBuildBranch()
        {
            return buildBranch;
        }

        public string getBuildTag()
        {
            return buildTag;
        }

        public override string ToString() // This function is what prints out all the build details.
        {
            string returnString = "Build: " + buildNum + " UNIX: " + installDate + " (Branch: " + buildBranch + ")" + " (" + buildTag + ")";
            return returnString;
        }
    }

    public sealed partial class homePage : Page
    {

        /* 
         * TODO:
         * - Check whether the file exists (sanity)
	       - **NO**, load normally and capture the current build
	       - **YES**, override the normal procedure and simply use current values.
           - Compare the current build with the captured one.
	       - **IF** build is higher, save it.
	       - **IF** build is equal, do nothing.
	       - Handle loading (?)
	       - Clone build elements and fill details.
         */
        static void SaveViaDataContractSerialization<T>(T serializableObject, string filepath)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
            };
            var writer = XmlWriter.Create(filepath, settings);
            serializer.WriteObject(writer, serializableObject);
            writer.Close();

        }

        // This function handles serializing the XML. (Read/Write?)
        static T LoadViaDataContractSerialization<T>(string filepath)
        {
            var fileStream = new FileStream(filepath, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
            var serializer = new DataContractSerializer(typeof(T));
            T serializableObject = (T)serializer.ReadObject(reader, true);
            reader.Close();
            fileStream.Close();
            return serializableObject;
        }

        public homePage()
        {
            // Local Folder

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\";

            this.InitializeComponent();

            // App Version
            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            appVer.Text = ver;

            // Check the log.
            // Debug.WriteLine(File.Exists(baseDirectory + "buildLog.xml") ? "File exists." : "File doesn't exist.");
            if (File.Exists(baseDirectory + "buildLog.xml"))
            {
                Debug.WriteLine("Log exists. Skipping normal loading.");
                var loadedBuild = LoadViaDataContractSerialization<List<fetchedBuild>>(baseDirectory + "buildLog.xml");
                var run = 0;

                //
                // UPDATE CHECK
                //
                foreach (var a in loadedBuild)
                {
                    var buildNumber = a.getBuildNum();
                    var installDate = a.getInstallDate();
                    var buildBranch = a.getBuildBranch();
                    var buildTag = a.getBuildTag();

                    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    var systemBuildNumber = registryKey.GetValue("CurrentBuild").ToString();
                    var systemBuildNumberC = int.Parse(systemBuildNumber);
                    var buildNumberC = int.Parse(buildNumber);

                    Debug.WriteLine("The system's build number is: " + systemBuildNumber + " and the fetched one was: " + buildNumber);

                    if (systemBuildNumberC > buildNumberC) // If the build number is higher (update)
                    {
                        
                        RegistryKey registryDev = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");

                        var sysBuild = registryKey.GetValue("CurrentBuild").ToString();
                        var sysBuildLab = registryKey.GetValue("BuildLab").ToString();
                        var sysBuildBranch = registryDev.GetValue("BranchName").ToString();
                        var sysInstallDate = registryKey.GetValue("InstallDate");

                        List<fetchedBuild> currentWorkingBuilds = new List<fetchedBuild>
                        {
                            new fetchedBuild(sysBuild, (int)sysInstallDate, sysBuildBranch, sysBuildLab)
                        };

                        XmlDocument buildLog = new XmlDocument();
                        buildLog.Load(baseDirectory + "buildLog.xml");
                        XmlElement systemBuild = buildLog.CreateElement("fetchedBuild");

                        XmlElement root = buildLog.DocumentElement;

                        root.PrependChild(systemBuild);

                        XmlElement sysBuildBranchX = buildLog.CreateElement("buildBranch");
                        sysBuildBranchX.InnerText = sysBuildBranch;
                        systemBuild.AppendChild(sysBuildBranchX);

                        XmlElement sysBuildX = buildLog.CreateElement("buildNum");
                        sysBuildX.InnerText = sysBuild;
                        systemBuild.AppendChild(sysBuildX);

                        XmlElement sysBuildLabX = buildLog.CreateElement("buildTag");
                        sysBuildLabX.InnerText = sysBuildLab;
                        systemBuild.AppendChild(sysBuildLabX);

                        XmlElement sysInstallDateX = buildLog.CreateElement("installDate");
                        sysInstallDateX.InnerText = sysInstallDate.ToString();
                        systemBuild.AppendChild(sysInstallDateX);

                        loadedBuild.Insert(0, new fetchedBuild(sysBuild, (int)sysInstallDate, sysBuildBranch, sysBuildLab));

                        SaveViaDataContractSerialization(loadedBuild, baseDirectory + "buildLog.xml"); // Save (pls no override)
                    }
                    break;
                }

                foreach (var a in loadedBuild)
                {

                    // The first run.
                    run += 1;
                    var buildNumber = a.getBuildNum();
                    var installDate = a.getInstallDate();
                    var buildBranch = a.getBuildBranch();
                    var buildTag = a.getBuildTag();

                    Run installDateElem = this.FindName("buildInstallDate") as Run;
                    Run buildBranchElem = this.FindName("buildBranchC") as Run;
                    Run buildTagElem = this.FindName("buildTag") as Run;

                    // The other runs, checks all the other builds. (If applicable)
                    if (run >= 2 && run <= 4)
                    {
                        SettingsCard pastWorkingBuild = this.FindName("pastBuild" + run) as SettingsCard;

                        // Rename elements

                        installDateElem = this.FindName("pastBuild" + run + "Install") as Run;
                        buildBranchElem = this.FindName("pastBuild" + run + "Branch") as Run;
                        buildTagElem = this.FindName("pastBuild" + run + "Tag") as Run;

                        pastWorkingBuild.Header = "Build " + buildNumber;
                        pastWorkingBuild.Visibility = Visibility.Visible;
                        installDateElem.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(installDate).ToString() + " //";
                        if (buildBranch == null)
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.Green);
                            buildBranchElem.Text = "GAC";
                        }
                        else if (buildBranch == "CanaryChannel") // Override so it looks less weird.
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.Yellow);
                            buildBranchElem.Text = "Canary";
                        }
                        else
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.OrangeRed);
                            buildBranchElem.Text = buildBranch;
                        }

                        buildTagElem.Text = "- " + buildTag;

                    } 
                    //
                    // Current Build
                    //
                    else
                    {
                        currentBuild.Header = "Build " + buildNumber;
                        installDateElem.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(installDate).ToString() + " //";
                        if (buildBranch == null)
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.Green);
                            buildBranchElem.Text = "GAC";
                        }
                        else if (buildBranch == "CanaryChannel") // Override so it looks less weird.
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.Yellow);
                            buildBranchElem.Text = "Canary";
                        }
                        else
                        {
                            buildBranchElem.Foreground = new SolidColorBrush(Colors.OrangeRed);
                            buildBranchElem.Text = buildBranch;
                        }

                        buildTagElem.Text = "- " + buildTag;
                    }
                }
                    

            } else
            {
                Debug.WriteLine("Log doesn't exist. Loading...");

                // Inherited from WinUISys.
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                RegistryKey registryDev = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\WindowsSelfHost\Applicability");

                var build = registryKey.GetValue("CurrentBuild").ToString();
                var buildLab = registryKey.GetValue("BuildLab").ToString();
                var buildBranch = registryDev.GetValue("BranchName").ToString();
                var installDate = registryKey.GetValue("InstallDate");


                // 

                // This is what fills in information.
                // build: #
                // buildLab: The full build tag.
                // buildBranch: Self
                // commonBuildName: Ex. "22H2"
                // editionID: "Enterprise" or etc.
                currentBuild.Header = "Build " + build;

                // Gets the install date.
                int uInstall = (int)installDate;
                buildInstallDate.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(uInstall).ToString() + " //";

                // Gets the branch (and sets the color)
                if (buildBranch != null)
                {
                    buildBranchC.Text = buildBranch;
                }
                else // Most likely, it's a GAC build without a branch.
                {
                    buildBranchC.Text = "GAC";
                }

                // Gets the build tag.
                buildTag.Text = "- " + buildLab;


                //
                // Serialize data. (TEST)
                //

                // Maybe the class will set?

                List<fetchedBuild> currentWorkingBuilds = new List<fetchedBuild>
                {
                    new fetchedBuild(build, (int)installDate, buildBranch, buildLab)
                };



                SaveViaDataContractSerialization(currentWorkingBuilds, baseDirectory + "buildLog.xml"); // Save
                currentWorkingBuilds = null; // This "deletes" or rather sets everything to null to the object.
                currentWorkingBuilds = LoadViaDataContractSerialization<List<fetchedBuild>>(baseDirectory + "buildLog.xml");
                foreach (var a in currentWorkingBuilds) // Go thru index.
                    Debug.WriteLine(a.ToString());

                // Debug.WriteLine(baseDirectory);
            }

        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(settingsPage));
        }
    }
}
