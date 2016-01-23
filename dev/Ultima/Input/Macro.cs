namespace UltimaXNA.Ultima.Input
{
    /// <summary>
    /// A single macro action.
    /// </summary>
    public class Macro
    {
        public readonly MacroType Type;
        public readonly int Index;

        private bool m_IsInteger = true;
        private int m_ValueInteger = -1;
        private string m_ValueString = null;

        public string Name
        {
            get
            {
                return Macros.GetMacroName(this);
            }
        }

        public Macro(MacroDefinition def)
        {
            Type = def.Type;
            Index = def.Index;
        }

        public Macro(MacroDefinition def, int value)
            : this(def)
        {
            m_ValueInteger = value;
            m_IsInteger = true;
        }

        public Macro(MacroDefinition def, string value)
            : this(def)
        {
            m_ValueString = value;
            m_IsInteger = false;
        }

        public int ValueInteger
        {
            set
            {
                m_IsInteger = true;
                m_ValueInteger = value;
            }
            get
            {
                return m_ValueInteger;
            }
        }

        public string ValueString
        {
            set
            {
                m_IsInteger = false;
                m_ValueString = value;
            }
            get
            {
                return m_ValueString;
            }
        }

        public bool IsInteger
        {
            get
            {
                return m_IsInteger;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, (m_IsInteger ? m_ValueInteger.ToString() : m_ValueString));
        }
    }
}
