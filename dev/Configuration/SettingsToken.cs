using Newtonsoft.Json.Linq;

namespace UltimaXNA.Data
{
    internal sealed class SettingsToken
    {
        public JToken Token;
        public string Comments;

        public SettingsToken(JToken token, string comments = null)
        {
            Token = token;
            Comments = comments;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != typeof(SettingsToken))
            {
                return false;
            }

            return Token == ((SettingsToken)obj).Token;
        }

        public override int GetHashCode()
        {
            return Token == null ? 0 : Token.GetHashCode();
        }
    }
}