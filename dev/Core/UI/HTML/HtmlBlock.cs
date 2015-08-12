using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.UI.HTML.Atoms;
using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML
{
    /// <summary>
    /// Blocks fit their content. They can be assigned width, height, and alignment.
    /// </summary>
    class HtmlBlock : AAtom
    {
        public List<AAtom> Contents = new List<AAtom>();

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

        public HtmlBlock(StyleState style)
            : base(style)
        {

        }

        public void AddAtom(AAtom atom)
        {
            Contents.Add(atom);
        }
    }
}
