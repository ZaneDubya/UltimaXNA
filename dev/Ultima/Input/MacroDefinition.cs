namespace UltimaXNA.Ultima.Input
{
    public class MacroDefinition
    {
        public string Name;
        public MacroType Type;
        public int Index;

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
