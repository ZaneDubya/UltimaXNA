#region Usings
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Core.Configuration
{
    internal sealed class SettingsContainer : Dictionary<string, SettingsToken>
    {
        public string Comments;

        public SettingsContainer(string comments = null)
        {
            Comments = comments;
        }
    }
}