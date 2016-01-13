namespace UltimaXNA.Configuration.Macros
{
    public class MacroDefinition
    {
        public string Name;
        public MacroType Type;
        public int Index;

        /// <summary>
        /// This should be replaced by the typed ctor.
        /// </summary>
        public MacroDefinition()
        {
            Name = string.Empty;
            Type = MacroType.None;
            Index = 0;
        }

        public MacroDefinition(string name, MacroType type)
        {
            Name = name;
            Type = type;
            Index = 0;
        }

        public MacroDefinition(string name, MacroType type, int index)
        {
            Name = name;
            Type = type;
            Index = index;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
