#region Usings

using System.Collections.Generic;

#endregion

namespace UltimaXNA.Data
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