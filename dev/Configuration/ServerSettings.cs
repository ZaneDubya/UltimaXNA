namespace UltimaXNA.Data
{
    public sealed class ServerSettings : SettingsBase
    {
        private const string Server = "server";

        internal ServerSettings(SettingsFile file)
            : base(file)
        {
        }

        protected override string Name
        {
            get { return Server; }
        }

        public string ServerAddress
        {
            get { return GetValue("127.0.0.1"); }
            set { SetValue(value); }
        }

        public int ServerPort
        {
            get { return GetValue(2593); }
            set { SetValue(value); }
        }

        public string UserName
        {
            get { return GetValue(""); }
            set { SetValue(value); }
        }
    }
}