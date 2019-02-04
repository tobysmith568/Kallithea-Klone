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

    public static class RunTypeMethods
    {
        public static IState GetState(this RunTypes type, string runLocation)
        {
            switch (type)
            {
                case RunTypes.Clone:
                    return new CloneState(runLocation);
                case RunTypes.LocalRevert:
                    return new LocalRevertState(runLocation);
                case RunTypes.Reclone:
                    return new ReCloneState(runLocation);
                case RunTypes.Update:
                    return new UpdateState(runLocation);
                case RunTypes.Settings:
                case RunTypes.Setup:
                case RunTypes.Uninstall:
                default:
                    throw new NotImplementedException($"The runType {type.ToString()} has not been implemented.");
            }
        }
    }
}
