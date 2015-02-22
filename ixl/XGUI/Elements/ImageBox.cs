using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Elements
{
    public class ImageBox : AElement
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.ImageBoxView(this, Manager);
        }

        public ImageBox(AElement parent, int page)
            : base(parent, page)
        {

        }

        public Texture2D Texture
        {
            get;
            set;
        }

        public bool Centered = false;
        public bool StretchImage = false;
    }
}
