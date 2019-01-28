namespace Kallithea_Klone.Account_Settings
{
    public class AdvancedOptions
    {
        //  Properties
        //  ==========

        public bool Enabled { get; set; }
        public bool Revert { get; set; }
        public bool Reclone { get; set; }
        public bool Update { get; set; }
        public bool Settings { get; set; }

        public int PackedValue
        {
            get
            {
                int result = 0;

                if (Enabled)
                    result += 1;

                if (Revert)
                    result += 2;

                if (Reclone)
                    result += 4;

                if (Update)
                    result += 8;

                if (Settings)
                    result += 16;

                return result;
            }
        }

        //  Constructors
        //  ============

        public AdvancedOptions(bool enabled = false, bool revert = false, bool reclone = false, bool update = false, bool settings = false)
        {
            Enabled = enabled;
            Revert = revert;
            Reclone = reclone;
            Update = update;
            Settings = settings;
        }

        public AdvancedOptions(int value)
        {
            if (value >= 32)
                value = 31;

            if (value >= 16)
            {
                Settings = true;
                value -= 16;
            }

            if (value >= 8)
            {
                Update = true;
                value -= 8;
            }

            if (value >= 4)
            {
                Reclone = true;
                value -= 4;
            }

            if (value >= 2)
            {
                Revert = true;
                value -= 2;
            }

            if (value >= 1)
            {
                Enabled = true;
                value -= 1;
            }
        }
    }
}