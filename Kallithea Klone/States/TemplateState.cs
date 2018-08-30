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

namespace Kallithea_Klone
{
    public abstract class TemplateState : IState
    {
        protected MainWindow mainWindow;

        public TemplateState()
        {
            mainWindow = MainWindow.singleton;
        }

        //  Abstract State Methods
        //  ======================

        public abstract void OnLoad();

        public abstract void OnMainAction();

        public abstract void OnReload();

        public abstract void OnSearch();

        public abstract void OnSearchTermChanged();

        //  Implemented State Methods
        //  =========================

        public virtual void OnLoaded()
        {

        }

        public virtual void OnLoseFocus()
        {
            if (!mainWindow.settingsOpen)
                Environment.Exit(0);
        }

        public virtual void OnSettings()
        {
            mainWindow.OpenSettings();
        }

        //  Other Methods
        //  =============

        protected bool ValidSettings()
        {
            return MainWindow.Host != ""
                && MainWindow.APIKey != ""
                && MainWindow.Username != ""
                && MainWindow.Password != "";
        }
    }
}
