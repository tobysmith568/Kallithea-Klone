using KallitheaKlone.Enums;
using KallitheaKlone.Models.Dialogs.FolderPicker;
using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.WFP.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KallitheaKlone.WPF.Models.Dialogs.RepositoryPicker
{
    public class RepositoryPicker : IRepositoryPicker
    {
        //  Variables
        //  =========

        private readonly IFolderPicker folderPicker;
        private readonly IMessagePrompt messagePrompt;
        private readonly IRepositoryFactory repositoryFactory;

        private static readonly RepositoryType[] repositoryTypes;

        //  Constructor
        //  ===========

        /// <exception cref="ReflectionTypeLoadException">Ignore.</exception>
        static RepositoryPicker()
        {
            IEnumerable<RepositoryType> types = Enumeration.GetAll<WPFRepositoryType>();
            repositoryTypes = types.ToArray();
        }

        public RepositoryPicker(IFolderPicker folderPicker, IMessagePrompt messagePrompt, IRepositoryFactory repositoryFactory)
        {
            this.folderPicker = folderPicker ?? throw new ArgumentNullException(nameof(folderPicker));
            this.messagePrompt = messagePrompt ?? throw new ArgumentNullException(nameof(messagePrompt));
            this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        //  Methods
        //  =======

        public IRepository Select()
        {
            string selectedFolder = folderPicker.Show();

            if (selectedFolder == null || selectedFolder == string.Empty)
            {
                return null;
            }

            if (!Directory.Exists(selectedFolder))
            {
                messagePrompt.PromptOK("The selected location does not exist!", "Error", MessageType.Error);
                return null;
            }

            try
            {
                foreach (RepositoryType repositoryType in repositoryTypes)
                {
                    if (Directory.Exists(Path.Combine(selectedFolder, repositoryType.DataFolder)))
                    {
                        return repositoryFactory.Create(repositoryType, selectedFolder);
                    }
                }
            }
            catch
            {
#warning //TODO
                return null;
            }
            return null;
        }
    }
}
