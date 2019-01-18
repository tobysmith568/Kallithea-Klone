using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Web;
using System.Security.Cryptography;
using System.Deployment.Application;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using Kallithea_Klone.States;

namespace Kallithea_Klone.States
{
    class UpdateState : TemplateState
    {
        //  Variables
        //  =========

        private int updatingCount;
        private int updatedCount = 0;
        private List<string> errorCodes = new List<string>();

        //  Constructors
        //  ============

        public UpdateState() : base()
        {

        }

        //  Events
        //  ======

        private void Process_Exited(object sender, EventArgs e)
        {
            if (((Process)sender).ExitCode != 0)
                errorCodes.Add(((Process)sender).ExitCode.ToString());
            updatedCount++;

            if (updatedCount == updatingCount)
            {
                if (errorCodes.Count > 0)
                    MessageBox.Show("Finshed, but with the following mercurial exit codes:\n" + string.Join("\n", errorCodes), "Errors", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Environment.Exit(0);
            }
        }

        //  State Pattern
        //  =============

        public override void OnLoad()
        {
            LoadRepositories();
        }

        public override void OnLoaded()
        {
            mainWindow.BtnClone.Content = "Update";
            mainWindow.LblTitle.Content = "Kallithea Update";
            mainWindow.BtnReload.Visibility = Visibility.Hidden;
        }

        public override void OnMainAction()
        {
            if (!ValidSettings())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        mainWindow.OpenSettings();
                        break;
                    default:
                        break;
                }
                return;
            }

            mainWindow.DisableAll();

            updatingCount = MainWindow.CheckedURLs.Count;
            foreach (string url in MainWindow.CheckedURLs)
            {
                string remotePath = GetDefaultPath(url);

                if (remotePath == null)
                    continue;

                Uri uri = new Uri(remotePath);
                string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(MainWindow.Username)}:{HttpUtility.UrlEncode(MainWindow.Password)}@{uri.Host}{uri.PathAndQuery}";

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = CmdExe,
                    Arguments = $"/C cd /d {url}" +
                                 $"&hg --config \"extensions.shelve = \" shelve --name {DateTime.Now.ToString("ddMMyyHHmmss")}" +
                                 $"&hg pull {fullURL}" +
                                 $"&hg update -m" +
                                 $"&hg --config \"extensions.shelve = \" unshelve --name {DateTime.Now.ToString("ddMMyyHHmmss")} --tool :other"
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();
            }
        }

        public override void OnReload()
        {
            throw new Exception("Invalid Button Press!");
        }

        public override void OnSearch()
        {
            if (mainWindow.TbxSearch.Text.Length != 0)
            {
                LoadRepositories(mainWindow.TbxSearch.Text.Split(' '));
            }
        }

        public override void OnSearchTermChanged()
        {
            if (mainWindow.TbxSearch.Text.Length == 0)
            {
                LoadRepositories();
            }
        }

        //  Other Methods
        //  =============

        private void LoadRepositories(string[] searchTerms = null)
        {
            mainWindow.MainTree.Items.Clear();

            string name = mainWindow.runFrom.Split('\\').Last().ToLower();

            if (IsRepo(mainWindow.runFrom) && (searchTerms == null || searchTerms.Where(t => name.Contains(t.ToLower())).Count() > 0))
            {
                mainWindow.MainTree.Items.Add(CreateRepo(mainWindow.runFrom));
            }
            else foreach (string folder in Directory.GetDirectories(mainWindow.runFrom))
            {
                name = folder.Split('\\').Last().ToLower();

                if (IsRepo(folder) && (searchTerms == null || searchTerms.Where(t => name.Contains(t.ToLower())).Count() > 0))
                {
                    mainWindow.MainTree.Items.Add(CreateRepo(folder));
                }
            }

            mainWindow.SelectionUpdated();
        }

        private bool IsRepo(string path)
        {
            string[] innerFolders = Directory.GetDirectories(path);
            foreach (string folder in innerFolders)
            {
                if (folder.Split('\\').Last() == ".hg")
                {
                    return true;
                }
            }
            return false;
        }

        private CheckBox CreateRepo(string location)
        {
            CheckBox newItem = new CheckBox
            {
                Content = location.Split('\\').Last(),
                Tag = location,
                VerticalContentAlignment = VerticalAlignment.Center,
                IsChecked = MainWindow.CheckedURLs.Contains(location)
            };
            newItem.Checked += mainWindow.NewItem_Checked;
            newItem.Unchecked += mainWindow.NewItem_Unchecked;

            return newItem;
        }
    }
}
