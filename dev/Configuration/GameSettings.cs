namespace UltimaXNA.Data
{
    public sealed class GameSettings : SettingsBase
    {
        private const string Game = "game";

        internal GameSettings(SettingsFile file)
            : base(file)
        {
        }

        protected override string Name
        {
            get { return Game; }
        }

        public bool AlwaysRun
        {
            get { return GetValue(false); }
            set { SetValue(value); }
        }
    }
}