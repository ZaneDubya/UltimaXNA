using System;

namespace UltimaXNA.Ultima.Player.Partying {
    public class PartyCommand {
        PartyCommandType m_CommandType;
        string m_Meesage;
        string m_SecondaryCommand;

        public string PlayerMessage { get { return m_Meesage; } set { m_Meesage = value; } }
        public PartyCommandType PrimaryCmd { get { return m_CommandType; } set { m_CommandType = value; } }
        /// <summary> for example: UserName which is adding or deleting </summary>
        public string SecondaryCmd { get { return m_SecondaryCommand; } set { m_SecondaryCommand = value; } }

        public PartyCommand(string _text) {
            string[] cmds = _text.Split(new string[] { " " }, StringSplitOptions.None);
            if (_text.ToLower() == "add") {
                PrimaryCmd = PartyCommandType.Add;
            }
            else if (cmds[0].ToLower() == "rem") {
                PrimaryCmd = PartyCommandType.Remove;
                int number1 = 0;
                bool isInt1 = int.TryParse(cmds[1], out number1);
                if (isInt1)
                    SecondaryCmd = number1.ToString();
                else
                    SecondaryCmd = "unknowncmd";
            }
            else if (_text.ToLower() == "accept") {
                PrimaryCmd = PartyCommandType.Accept;
            }
            else if (_text.ToLower() == "decline") {
                PrimaryCmd = PartyCommandType.Decline;
            }
            else if (_text.ToLower() == "quit") {
                PrimaryCmd = PartyCommandType.Quit;
            }
            else if (cmds[0].ToLower() == "loot") {
                PrimaryCmd = PartyCommandType.Loot;
                if (cmds.Length > 1) {
                    if (cmds[1].ToLower() == "on")
                        SecondaryCmd = "on";
                    else if (cmds[1].ToLower() == "off")
                        SecondaryCmd = "off";
                    return;
                }
                SecondaryCmd = "unknowncmd";
            }
            else if (_text.ToLower() == "list")//list of party members (new)
            {
                PrimaryCmd = PartyCommandType.List;
            }
            else if (_text.ToLower() == "hlp")//about of party commands (new)
            {
                PrimaryCmd = PartyCommandType.HelpMenu;
            }
            else {
                //public party message
                PrimaryCmd = PartyCommandType.Public;
                PlayerMessage = _text;
                return;
            }
        }
    }
}
