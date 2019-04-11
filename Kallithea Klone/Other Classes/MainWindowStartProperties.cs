using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kallithea_Klone.Other_Classes
{
    public class MainWindowStartProperties
    {
        //  Properties
        //  ==========

        public string Title { get; set; }
        public string MainActionContent { get; set; }
        public Visibility ReloadVisibility { get; set; }

        //  Constructors
        //  ============

        public MainWindowStartProperties()
        {

        }

        public MainWindowStartProperties(string title, string mainActionverb, Visibility reloadVisible)
        {
            Title = title;
            MainActionContent = mainActionverb;
            ReloadVisibility = reloadVisible;
        }
    }
}
