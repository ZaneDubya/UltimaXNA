using Newtonsoft.Json.Linq;

namespace UltimaXNA.Data
{
    internal sealed class SettingsToken
    {
        public JToken Value;
        public string Comments;

        public SettingsToken(JToken value, string comments = null)
        {
            Value = value;
            Comments = comments;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != typeof(SettingsToken))
            {
                return false;
            }

            return Value == ((SettingsToken)obj).Value;
        }

        public override int GetHashCode()
        {
            return Value == null ? 0 : Value.GetHashCode();
        }
    }
}