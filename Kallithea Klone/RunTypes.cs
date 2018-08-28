using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone
{
    public enum RunTypes
    {
        Clone,
        LocalRevert,

        Settings,

        Setup,
        Uninstall,
    }

    static class RunTypeMethods
    {
        public static IState GetState(this RunTypes type)
        {
            switch (type)
            {
                case RunTypes.Clone:
                    return new CloneState();
                case RunTypes.LocalRevert:
                case RunTypes.Settings:
                case RunTypes.Setup:
                case RunTypes.Uninstall:
                default:
                    throw new Exception("Unhandled Run Type");
            }
        }
    }
}
