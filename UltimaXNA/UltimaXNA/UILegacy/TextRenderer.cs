using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy
{
    class TextRenderer
    {
        private Texture2D _texture;
        public Texture2D Texture
        {
            get { return _texture; }
        }

        private HREFRegions _href;
        public HREFRegions HREFRegions
        {
            get { return _href; }
        }

        private bool _lastText_AsHTML = false;
        private int _lastText_Width = -1, _lastText_Height = -1;
        private string _lastText_String = string.Empty;

        public void RenderText(string text)
        {
            RenderText(text, false, 0, 0);
        }

        public void RenderText(string text, int width, int height)
        {
            RenderText(text, false, width, height);
        }

        public void RenderText(string text, bool asHTML)
        {
            RenderText(text, asHTML, 0, 0);
        }

        public void RenderText(string text, bool asHTML, int width, int height)
        {
            if ((text == _lastText_String) && (asHTML == _lastText_AsHTML) &&
                (width == _lastText_Width) && (height == _lastText_Height) &&
                _texture != null)
            {
                // we are trying to render the same text again.
                return;
            }

            _lastText_String = text;
            _lastText_AsHTML = asHTML;
            _lastText_Width = width;
            _lastText_Height = height;

            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
            if (_href != null)
            {
                _href = null;
            }

            if (asHTML)
                _texture = Data.UniText.GetTextureHTML(text, width, height, ref _href);
            else
                _texture = Data.UniText.GetTexture(text, width, height);
        }
    }
}
