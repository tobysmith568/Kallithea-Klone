using System;

namespace Kallithea_Klone.States
{
    public enum RunTypes
    {
        Clone,
        LocalRevert,
        Reclone,
        Update,

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
                    return new LocalRevertState();
                case RunTypes.Reclone:
                    return new ReCloneState();
                case RunTypes.Update:
                    return new UpdateState();
                case RunTypes.Settings:
                case RunTypes.Setup:
                case RunTypes.Uninstall:
                default:
                    throw new Exception("Unhandled Run Type");
            }
        }
    }
}
