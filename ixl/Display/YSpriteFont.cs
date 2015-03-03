using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace InterXLib.Display
{
    public class YSpriteFont
    {
        private List<Char> m_Characters;
        private Dictionary<char, Glyph> m_Glyphs;

        public Texture2D Texture;
        public float LineSpacing;
        public float Spacing;
        public char? DefaultCharacter;

        public int Line0VerticalOffset = 0;

        public static YSpriteFont LoadFont(GraphicsDevice device, string path, float size)
        {
            YSpriteFont ysf = (YSpriteFont)XNBReader.ReadObject(device, path);
            ysf.Size = size;
            return ysf;
        }

        public static YSpriteFont LoadFontDF(GraphicsDevice device, string texture_path, string xml_path, float size)
        {
            Texture2D texture = (Texture2D)XNBReader.ReadObject(device, texture_path);
            XmlTextReader xml = new XmlTextReader(xml_path);
            YSpriteFont ysf = CreateFromFontDF(texture, xml);
            ysf.Size = size;
            return ysf;
        }

        public void CorrectNumeralOneWidth()
        {
            Glyph one = m_Glyphs['1'];
            Glyph zero = m_Glyphs['0'];
            one.LeftSideBearing += (zero.Width - one.Width) / 2;
            one.Width = zero.Width - one.LeftSideBearing;
            m_Glyphs['1'] = one;
        }

        public bool HasCharacter(char ch)
        {
            Glyph glyph;
            if (m_Glyphs.TryGetValue(ch, out glyph))
                return true;
            return false;
        }

        public Glyph GetCharacter(char chr)
        {
            Glyph glyph;
            if (m_Glyphs.TryGetValue(chr, out glyph))
                return glyph;
            return m_Glyphs[' '];
        }

        internal static YSpriteFont CreateFromFontDF(Texture2D texture, XmlTextReader xml)
        {
            List<char> characters = new List<char>();
            List<Rectangle> glyphBounds = new List<Rectangle>();
            List<Rectangle> cropping = new List<Rectangle>();
            List<Vector3> kerning = new List<Vector3>();

            float baseline = 0, linespacing = 0;
            int char_count = 0;

            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xml.Name == "font")
                        {
                            // element 'font' is the main element, has no attributes, can ignore.
                        }
                        else if (xml.Name == "info")
                        {
                            // element 'info' has information about the height, etc, but we can get this elsewhere...
                        }
                        else if (xml.Name == "common")
                        {
                            while (xml.MoveToNextAttribute())
                            {
                                if (xml.Name == "lineHeight")
                                    linespacing = float.Parse(xml.Value);
                                else if (xml.Name == "base")
                                    baseline = float.Parse(xml.Value);
                            }
                        }
                        else if (xml.Name == "pages")
                        {
                            // more useless data ...
                        }
                        else if (xml.Name == "page")
                        {
                            // more useless data ...
                        }
                        else if (xml.Name == "chars")
                        {
                            while (xml.MoveToNextAttribute())
                            {
                                if (xml.Name == "count")
                                    char_count = int.Parse(xml.Value);
                            }
                        }
                        else if (xml.Name == "char")
                        {
                            char ch = (char)0;
                            Vector4 bounds = Vector4.Zero;
                            Vector4 crops = Vector4.Zero;
                            Vector3 kern = Vector3.Zero;
                            while (xml.MoveToNextAttribute())
                            {
                                switch (xml.Name)
                                {
                                    case "id":
                                        ch = (char)int.Parse(xml.Value);
                                        break;
                                    case "x":
                                        bounds.X = float.Parse(xml.Value);
                                        break;
                                    case "y":
                                        bounds.Y = float.Parse(xml.Value);
                                        break;
                                    case "width":
                                        bounds.Z = float.Parse(xml.Value);
                                        break;
                                    case "height":
                                        bounds.W = float.Parse(xml.Value);
                                        break;
                                    case "xoffset":
                                        crops.X = float.Parse(xml.Value);
                                        // bounds.Z = bounds.Z - float.Parse(xml.Value);
                                        break;
                                    case "yoffset":
                                        crops.Y = float.Parse(xml.Value);
                                        // bounds.W = bounds.W - float.Parse(xml.Value);
                                        break;
                                    case "xadvance":
                                        kern = new Vector3(0, bounds.Z, float.Parse(xml.Value) - bounds.Z);
                                        break;
                                    case "page":
                                        break;
                                    case "chnl":
                                        break;
                                }
                            }
                            characters.Add(ch);
                            glyphBounds.Add(new Rectangle((int)(bounds.X * 4), (int)(bounds.Y * 4), (int)(bounds.Z * 4), (int)(bounds.W * 4)));
                            cropping.Add(new Rectangle((int)(crops.X * 4), (int)(crops.Y * 4), (int)(crops.Z * 4), (int)(crops.W * 4)));
                            kerning.Add(kern);
                        }
                        else
                        {

                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    case XmlNodeType.Text:
                        break;
                }
            }

            YSpriteFont ysf = new YSpriteFont(texture, glyphBounds, cropping, characters, (int)(linespacing * 4), 0f, kerning, null);
            ysf.IsDistanceFieldFont = true;
            ysf.DistanceFieldScale = 0.25f;
            return ysf;
        }

        public bool IsDistanceFieldFont = false;
        private float m_DistanceFieldScale = 1f;
        public float DistanceFieldScale
        {
            get { return m_DistanceFieldScale; }
            set
            {
                if (value != m_DistanceFieldScale)
                {
                    char[] keys = m_Glyphs.Keys.ToArray<char>();
                    foreach (char c in keys)
                    {
                        Glyph g = m_Glyphs[c];
                        g.Cropping *= value;
                        g.BoundsInTexture *= value;
                        m_Glyphs[c] = g;
                    }
                    LineSpacing *= value;
                    m_DistanceFieldScale = value;
                }
            }
        }

        public float Size = 0f;

        internal YSpriteFont (
			Texture2D texture, List<Rectangle> glyphBounds, List<Rectangle> cropping, List<char> characters,
			int lineSpacing, float spacing, List<Vector3> kerning, char? defaultCharacter)
		{
			m_Characters = characters;
			Texture = texture;
			LineSpacing = lineSpacing;
			Spacing = spacing;
			DefaultCharacter = defaultCharacter;

			m_Glyphs = new Dictionary<char, Glyph>(characters.Count, CharComparer.Default);

			for (var i = 0; i < characters.Count; i++) 
            {
				var glyph = new Glyph 
                {
					BoundsInTexture = new Vector4(glyphBounds[i].X, glyphBounds[i].Y, glyphBounds[i].Width, glyphBounds[i].Height),
                    Cropping = new Vector4(cropping[i].X, cropping[i].Y, cropping[i].Width, cropping[i].Height),
                    Character = characters[i],

                    LeftSideBearing = kerning[i].X,
                    Width = kerning[i].Y,
                    RightSideBearing = kerning[i].Z,

                    WidthIncludingBearings = kerning[i].X + kerning[i].Y + kerning[i].Z
				};
				m_Glyphs.Add (glyph.Character, glyph);
			}
		}


        public string BreakStringIntoLines(string text, int max_width, float font_size)
        {
            string[] words = text.Replace("\n", "\n ").Split(new char[1] { ' ' });
            string out_text = string.Empty;
            float line_length = 0;
            foreach (string word in words)
            {
                CharacterSource source = new CharacterSource(word);
                Vector2 size;
                float space_width = m_Glyphs[' '].WidthIncludingBearings * (font_size / Size);
                MeasureString(ref source, font_size, out size);
                float word_width = space_width + size.X;
                if (line_length == 0)
                {
                    out_text += word;
                    line_length = size.X;
                }
                else if (line_length + word_width < max_width)
                {
                    out_text += ' ' + word;
                    line_length += word_width;
                }
                else
                {
                    out_text += '\n' + word;
                    line_length = size.X;
                }
                if (out_text[out_text.Length - 1] == '\n')
                {
                    line_length = 0;
                }
            }
            return out_text;
        }

        /// <summary>
        /// Returns the size of a string when rendered in this font.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The size, in pixels, of 'text' when rendered in
        /// this font.</returns>
        public Vector2 MeasureString(string text, float font_size)
        {
            CharacterSource source = new CharacterSource(text);
            Vector2 size;
            MeasureString(ref source, font_size, out size);
            return size;
        }

        private void MeasureString(ref CharacterSource text, float font_size, out Vector2 size)
        {
            float scale = font_size / Size;

            if (text.Length == 0)
            {
                size = Vector2.Zero;
                return;
            }

            // Get the default glyph here once.
            Glyph? defaultGlyph = null;
            if (DefaultCharacter.HasValue)
                defaultGlyph = m_Glyphs[DefaultCharacter.Value];

            var width = 0.0f;
            var finalLineHeight = (float)LineSpacing * scale;
            var fullLineCount = 0;
            var currentGlyph = Glyph.Empty;
            var offset = Vector2.Zero;
            var hasCurrentGlyph = false;
            var firstGlyphOfLine = true;

            for (var i = 0; i < text.Length; ++i)
            {
                var c = text[i];
                if (c == '\r')
                {
                    hasCurrentGlyph = false;
                    continue;
                }

                if (c == '\n')
                {
                    fullLineCount++;
                    finalLineHeight = LineSpacing * scale;

                    offset.X = 0;
                    offset.Y = LineSpacing * fullLineCount * scale;
                    hasCurrentGlyph = false;
                    firstGlyphOfLine = true;
                    continue;
                }

                if (hasCurrentGlyph)
                {
                    offset.X += Spacing * scale;

                    // The first character on a line might have a negative left side bearing.
                    // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                    //  so that text does not hang off the left side of its rectangle.
                    if (firstGlyphOfLine)
                    {
                        offset.X = System.Math.Max(offset.X + Math.Abs(currentGlyph.LeftSideBearing), 0) * scale;
                        firstGlyphOfLine = false;
                    }
                    else
                    {
                        offset.X += currentGlyph.LeftSideBearing * scale;
                    }

                    offset.X += (currentGlyph.Width + currentGlyph.RightSideBearing) * scale;
                }

                if (!m_Glyphs.TryGetValue(c, out currentGlyph))
                {
                    if (!defaultGlyph.HasValue)
                        Logging.Fatal("Error: TextContainsUnresolvableCharacters");

                    currentGlyph = defaultGlyph.Value;
                }
                hasCurrentGlyph = true;

                var proposedWidth = offset.X + (currentGlyph.WidthIncludingBearings + Spacing) * scale;
                if (proposedWidth > width)
                    width = proposedWidth;

                if (currentGlyph.Cropping.W * scale > finalLineHeight)
                    finalLineHeight = currentGlyph.Cropping.W * scale;

                if (i == text.Length - 1)
                {
                    width -= currentGlyph.RightSideBearing * scale;
                }
            }

            size.X = width;
            size.Y = (fullLineCount * LineSpacing) * scale + finalLineHeight;
        }

        internal void DrawInto(YSpriteBatch destination, string s, Vector2 position, Color color,
                                Vector2 area, FontJustification justification, float font_size)
        {
            float scale = font_size / Size;

            CharacterSource text = new CharacterSource(s);

            Vector2 size_entire = MeasureString(text.Text, font_size);
            int[] line_offsets = new int[text.LineCount];
            Vector2 origin = Vector2.Zero;
            
            if (justification.HasFlag(FontJustification.Center))
            {
                int max_line_length = 0;
                for (int i = 0; i < text.LineCount; i++)
                {
                    Vector2 size_line = MeasureString(text.GetLine(i), font_size);
                    if (size_line.X > max_line_length)
                        max_line_length = (int)size_line.X;
                    line_offsets[i] = (int)((size_entire.X - size_line.X) / 2f);
                }

                for (int i = 0; i < text.LineCount; i++)
                    line_offsets[i] += (int)(area.X - max_line_length) / 2;
            }
            else if (justification.HasFlag(FontJustification.Right))
            {
                for (int i = 0; i < text.LineCount; i++)
                {
                    Vector2 size_line = MeasureString(text.GetLine(i), font_size);
                    line_offsets[i] = (int)(area.X - size_line.X);
                }
            }

            if (justification.HasFlag(FontJustification.CenterVertically))
            {
                position.Y += (int)(area.Y - size_entire.Y) / 2;
            }
            else if (justification.HasFlag(FontJustification.Bottom))
            {
                position.Y += (int)(area.Y - size_entire.Y);
            }

            var flipAdjustment = Vector2.Zero;

            // TODO: This looks excessive... i suspect we could do most
            // of this with simple vector math and avoid this much matrix work.
            Matrix transformation, temp;
            Matrix.CreateTranslation(-origin.X, -origin.Y, 0f, out transformation);
            Matrix.CreateScale(1f, 1f, 1f, out temp);
            Matrix.Multiply(ref transformation, ref temp, out transformation);
            Matrix.CreateTranslation(flipAdjustment.X, flipAdjustment.Y, 0, out temp);
            Matrix.Multiply(ref temp, ref transformation, out transformation);
            // Matrix.CreateRotationZ(rotation, out temp);
            Matrix.Multiply(ref transformation, ref temp, out transformation);
            Matrix.CreateTranslation(position.X, position.Y, 0f, out temp);
            Matrix.Multiply(ref transformation, ref temp, out transformation);

            // Get the default glyph here once.
            Glyph? defaultGlyph = null;
            if (DefaultCharacter.HasValue)
                defaultGlyph = m_Glyphs[DefaultCharacter.Value];

            int line = 0;
            var currentGlyph = Glyph.Empty;
            var offset = new Vector2(line_offsets[line++], 0);
            var hasCurrentGlyph = false;
            var firstGlyphOfLine = true;

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                if (c == '\r')
                {
                    hasCurrentGlyph = false;
                    continue;
                }

                if (c == '\n')
                {
                    offset.X = line_offsets[line++];
                    offset.Y += LineSpacing * scale;
                    hasCurrentGlyph = false;
                    firstGlyphOfLine = true;
                    continue;
                }

                if (hasCurrentGlyph)
                {
                    offset.X += ((Spacing + currentGlyph.Width + currentGlyph.RightSideBearing) * scale);
                }

                if (!m_Glyphs.TryGetValue(c, out currentGlyph))
                {
                    if (!defaultGlyph.HasValue)
                        Logging.Fatal("Error: TextContainsUnresolvableCharacters");

                    currentGlyph = defaultGlyph.Value;
                }
                hasCurrentGlyph = true;

                // The first character on a line might have a negative left side bearing.
                // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                //  so that text does not hang off the left side of its rectangle.
                if (firstGlyphOfLine)
                {
                    offset.X += System.Math.Max(currentGlyph.LeftSideBearing, 0) * scale;
                    firstGlyphOfLine = false;
                }
                else
                {
                    offset.X += currentGlyph.LeftSideBearing * scale;
                }

                var p = offset;

                p.X += currentGlyph.Cropping.X * scale;
                p.Y += currentGlyph.Cropping.Y * scale;

                Vector2.Transform(ref p, ref transformation, out p);

                Vector4 destRect = new Vector4(p.X, p.Y,
                                            currentGlyph.BoundsInTexture.Z * scale,
                                            currentGlyph.BoundsInTexture.W * scale);
                if (IsDistanceFieldFont)
                    destination.DrawSpriteGlyphDF(Texture, destRect, currentGlyph.BoundsInTexture, color, scale);
                else
                    destination.DrawSpriteGlyph(Texture, destRect, currentGlyph.BoundsInTexture, color);
            }
        }

        internal struct CharacterSource 
        {
			private readonly string m_string;
			private readonly StringBuilder m_builder;
            private string[] m_lines;

			public CharacterSource(string s)
			{
				m_string = s;
				m_builder = null;
				Length = s.Length;
                m_lines = null;
			}

			public CharacterSource(StringBuilder builder)
			{
				m_builder = builder;
				m_string = null;
				Length = m_builder.Length;
                m_lines = null;
			}

			public readonly int Length;
			public char this [int index] 
            {
				get 
                {
					if (m_string != null)
						return m_string[index];
					return m_builder[index];
				}
			}

            public string Text
            {
                get
                {
                    if (m_string != null)
                        return m_string;
                    return m_builder.ToString();
                }
            }

            public int LineCount
            {
                get
                {
                    if (m_lines == null)
                    {
                        m_lines = Text.Split('\n');
                    }
                    return m_lines.Length;
                }
            }

            public string GetLine(int index)
            {
                if (index < 0 || index >= LineCount)
                    return null;
                return m_lines[index];
            }
		}

        /// <summary>
        /// Struct that defines the spacing, Kerning, and bounds of a character.
        /// </summary>
        /// <remarks>Provides the data necessary to implement custom SpriteFont rendering.</remarks>
		public struct Glyph 
        {
            /// <summary>
            /// The char associated with this glyph.
            /// </summary>
			public char Character;
            /// <summary>
            /// Rectangle in the font texture where this letter exists.
            /// </summary>
			public Vector4 BoundsInTexture;
            /// <summary>
            /// Cropping applied to the BoundsInTexture to calculate the bounds of the actual character.
            /// </summary>
			public Vector4 Cropping;
            /// <summary>
            /// The amount of space between the left side ofthe character and its first pixel in the X dimention.
            /// </summary>
            public float LeftSideBearing;
            /// <summary>
            /// The amount of space between the right side of the character and its last pixel in the X dimention.
            /// </summary>
            public float RightSideBearing;
            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public float Width;
            /// <summary>
            /// Width of the character before kerning is applied. 
            /// </summary>
            public float WidthIncludingBearings;

			public static readonly Glyph Empty = new Glyph();

			public override string ToString ()
			{
                return "CharacterIndex=" + Character + ", Glyph=" + BoundsInTexture + ", Cropping=" + Cropping + ", Kerning=" + LeftSideBearing + "," + Width + "," + RightSideBearing;
			}
		}

        class CharComparer : IEqualityComparer<char>
        {
            public bool Equals(char x, char y)
            {
                return x == y;
            }

            public int GetHashCode(char b)
            {
                return (b | (b << 16));
            }

            static public readonly CharComparer Default = new CharComparer();
        }
	}
}
