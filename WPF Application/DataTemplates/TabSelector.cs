﻿using KallitheaKlone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KallitheaKlone.WPF.DataTemplates
{
    public class TabSelector : DataTemplateSelector
    {
        public DataTemplate RepositoryTemplate { get; set; }
        public DataTemplate NewTabTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
#warning //TODO cast check
            TabViewModel tabViewModel = item as TabViewModel;

            if (tabViewModel is RepositoryViewModel)
            {
                return RepositoryTemplate ?? throw new NotImplementedException($"No {nameof(RepositoryTemplate)} is bound in the TabSelector");
            }

            if (tabViewModel is NewTabViewModel)
            {
                return NewTabTemplate ?? throw new NotImplementedException($"No {nameof(NewTabTemplate)} is bound in the TabSelector");
            }

#warning //TODO
            throw new NotImplementedException();
        }
    }
}
