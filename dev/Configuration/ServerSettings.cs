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

        protected override string Comments
        {
            get { return @"This section is responsible for all server related settings.  These values can be modified to change the server connection."; }
        }

        public string ServerAddress
        {
            get { return GetValue("127.0.0.1"); }
            set { SetValue(value, "Address of the server you want to connect to."); }
        }

        public int ServerPort
        {
            get { return GetValue(2593); }
            set { SetValue(value, "Port number of the server you want to connect to."); }
        }

        public string UserName
        {
            get { return GetValue(""); }
            set { SetValue(value, "Username last used when connecting to a server."); }
        }
    }
}