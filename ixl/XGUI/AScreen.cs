using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using InterXLib.Patterns.MVC;

namespace InterXLib.XGUI
{
    public abstract class AScreen : AElement
    {
        public AScreen()
            : base(null, 0)
        {
            LocalArea = new Rectangle(0, 0, Settings.Resolution.X, Settings.Resolution.Y);
        }

        protected override AView CreateView()
        {
            return new AScreenView(this, Manager);
        }
    }
}
