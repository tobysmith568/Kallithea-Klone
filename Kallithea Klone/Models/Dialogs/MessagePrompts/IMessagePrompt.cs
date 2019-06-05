using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Dialogs.MessagePrompts
{
    public interface IMessagePrompt
    {
        //  Methods
        //  =======

        void PromptOK(string message, string title = "", MessageType messageType = MessageType.None);
        MessageResult PromptOKCancel(string message, string title = "", MessageType messageType = MessageType.None);
        MessageResult PromptYesNo(string message, string title = "", MessageType messageType = MessageType.None);
        MessageResult PromptYesNoCancel(string message, string title = "", MessageType messageType = MessageType.None);
    }
}
