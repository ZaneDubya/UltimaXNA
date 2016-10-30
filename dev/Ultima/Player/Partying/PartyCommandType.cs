using System.ComponentModel;

namespace UltimaXNA.Ultima.Player.Partying {
    public enum PartyCommandType {
        Add,

        [DefaultValue("rem")]
        Remove,

        Accept,
        Decline,
        Quit,
        Loot, // (on-off) control is secondary command
        List,
        Public,

        [DefaultValue("hlp")]
        HelpMenu,//list all party member with index number for private message

        Unknown,
    }
}
