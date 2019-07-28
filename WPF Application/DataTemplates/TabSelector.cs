using KallitheaKlone.ViewModels;
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

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;

#warning //TODO cast check
            TabViewModel tabViewModel = item as TabViewModel;
            if (tabViewModel is RepositoryViewModel)
            {
                return RepositoryTemplate ?? throw new NotImplementedException("No RepositoryTemplate is bound in the TabSelector");
            }

#warning //TODO
            throw new NotImplementedException();
        }
    }
}
