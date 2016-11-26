using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI.HTML.Elements;
using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlRenderer
    {
        /// <summary>
        /// Renders all the elements in the root branch. At the same time, also sets areas for regions and href links.
        /// TODO: code for setting areas for regions / hrefs belongs in layout code in HtmlDocument.
        /// </summary>
        public Texture2D Render(BlockElement root, int ascender, HtmlLinkList links)
        {
            SpriteBatchUI sb = Services.Get<SpriteBatchUI>();
            GraphicsDevice graphics = sb.GraphicsDevice;

            if (root == null || root.Width == 0 || root.Height == 0) // empty text string
            {
                return new Texture2D(graphics, 1, 1);
            }
            uint[] pixels = new uint[root.Width * root.Height];

            if (root.Err_Cant_Fit_Children)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = 0xffffff00;
                }
                Tracer.Error("Err: Block can't fit children.");
            }
            else
            {
                unsafe
                {
                    fixed (uint* ptr = pixels)
                    {
                        DoRenderBlock(root, ascender, links, ptr, root.Width, root.Height);
                    }
                }
            }

            Texture2D texture = new Texture2D(graphics, root.Width, root.Height, false, SurfaceFormat.Color);
            texture.SetData(pixels);
            return texture;
        }

        unsafe void DoRenderBlock(BlockElement root, int ascender, HtmlLinkList links, uint* ptr, int width, int height)
        {
            foreach (AElement e in root.Children)
            {
                int x = e.Layout_X;
                int y = e.Layout_Y - ascender; // ascender is always negative.
                StyleState style = e.Style;
                if (e is CharacterElement)
                {
                    IFont font = style.Font;
                    ICharacter character = font.GetCharacter((e as CharacterElement).Character);
                    // HREF links should be colored white, because we will hue them at runtime.
                    uint color = style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(style.Color);
                    character.WriteToBuffer(ptr, x, y, width, height, font.Baseline, style.IsBold, style.IsItalic, style.IsUnderlined, style.DrawOutline, color, 0xFF000008);
                    // offset y by ascender for links...
                    if (character.YOffset < 0)
                    {
                        y += character.YOffset;
                        height -= character.YOffset;
                    }
                }
                else if (e is ImageElement)
                {
                    ImageElement image = (e as ImageElement);
                    image.AssociatedImage.Area = new Rectangle(x, y, image.Width, image.Height);
                    if (style.IsHREF)
                    {
                        links.AddLink(style, new Rectangle(x, y, e.Width, e.Height));
                        image.AssociatedImage.LinkIndex = links.Count;
                    }
                }
                else if (e is BlockElement)
                {
                    DoRenderBlock(e as BlockElement, ascender, links, ptr, width, height);
                }
                // set href link regions
                if (style.IsHREF)
                {
                    links.AddLink(style, new Rectangle(x, y, e.Width, e.Height));
                }
            }
        }
    }
}
