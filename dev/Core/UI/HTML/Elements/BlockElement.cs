using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    /// <summary>
    /// Blocks fit their content. They can be assigned width, height, and alignment.
    /// </summary>
    class BlockElement : AElement
    {
        public List<AElement> Children = new List<AElement>();
        public BlockElement Parent;

        public string Tag;

        private Rectangle m_Area;

        public Rectangle Area
        {
            get { return m_Area; }
            set { m_Area = value; }
        }

        public override int Width
        {
            get
            {
                return m_Area.Width;
            }
            set
            {
                m_Area.Width = value;
            }
        }

        public override int Height
        {
            get
            {
                return m_Area.Height;
            }
            set
            {
                m_Area.Height = value;
            }
        }

        public Alignments Alignment = Alignments.Default;

        public int Layout_MinWidth = 0;
        public int Layout_MaxWidth = 0;

        public BlockElement(string tag, StyleState style)
            : base(style)
        {
            Tag = tag;
        }

        public void AddAtom(AElement atom)
        {
            Children.Add(atom);
            if (atom is BlockElement)
                (atom as BlockElement).Parent = this;
        }

        public override string ToString()
        {
            return Tag;
        }

        public bool Err_Cant_Fit_Children = false;
    }
}
