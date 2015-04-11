#region Usings
using Newtonsoft.Json.Linq;
#endregion

namespace UltimaXNA.Configuration
{
    internal sealed class SettingsToken
    {
        public string Comments;
        public object Value;

        public SettingsToken(object value, string comments = null)
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