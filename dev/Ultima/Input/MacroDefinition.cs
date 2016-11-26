namespace UltimaXNA.Ultima.Input
{
    public class MacroDefinition
    {
        public readonly string Name;
        public readonly MacroType Type;
        public readonly int Index;

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
