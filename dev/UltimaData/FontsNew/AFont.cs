#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.UltimaData.FontsNew
{
    abstract class AFont
    {
        protected ACharacter[] m_Characters;

        public int Height
        {
            get;
            set;
        }

        public int Baseline
        {
            get
            {
                return GetCharacter('M').Height + GetCharacter('M').YOffset;
            }
        }

        public int Lineheight
        {
            get
            {
                return Baseline + 4;
            }
        }

        // public Texture2D Texture;

        /*protected void InitializeTexture(GraphicsDevice device, bool bigTexture = false)
        {
            int textureSize = bigTexture ? 1024 : 512;

            CygonRectanglePacker packer = new CygonRectanglePacker(textureSize, textureSize);
            uint[] textureData = new uint[textureSize * textureSize];

            // get the sprite data and get a place for each character in the texture, then write the character data to an array.
            foreach (ACharacter ch in m_Characters)
            {
                Point uv;
                if (packer.TryPack(ch.Width + 4, ch.Height + 4, out uv)) // allow a two-pixel buffer on each side of the character
                {
                    ch.TextureBounds = new Rectangle(uv.X + 2, uv.Y + 2, ch.Width, ch.Height);
                    ch.WriteTextureData(textureData);
                }
                else
                {
                    Logger.Fatal("Could not pack font.mul texture.");
                }
            }

            // write the completed array of character data to the texture.
            Texture = new Texture2D(device, textureSize, textureSize);
            Texture.SetData<uint>(textureData);
        }*/

        public abstract ACharacter GetCharacter(char character);

        public void GetTextDimensions(ref string text, ref int width, ref int height, int wrapwidth)
        {
            width = 0;
            height = Height;
            int biggestwidth = 0;
            List<char> word = new List<char>();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (((int)c) > 32)
                {
                    word.Add(c);
                }

                if (c == ' ' || i == text.Length - 1 || c == '\n')
                {
                    // Size the word, character by character.
                    int wordwidth = 0;

                    if (word.Count > 0)
                    {
                        for (int j = 0; j < word.Count; j++)
                        {
                            int charwidth = GetCharacter(word[j]).Width;
                            wordwidth += charwidth;
                        }
                    }

                    // Now make sure this line can fit the word.
                    if (width + wordwidth <= wrapwidth)
                    {
                        width += wordwidth;
                        word.Clear();
                        // if this word is followed by a space, does it fit? If not, drop it entirely and insert \n after the word.
                        if (c == ' ')
                        {
                            int charwidth = GetCharacter(c).Width;
                            if (width + charwidth <= wrapwidth)
                            {
                                // we can fit an extra space here.
                                width += charwidth;
                            }
                            else
                            {
                                // can't fit an extra space on the end of the line. replace the space with a \n.
                                text = text.Substring(0, i) + '\n' + text.Substring(i + 1, text.Length - i - 1);
                                i--;
                            }
                        }
                    }
                    else
                    {
                        // The word is too big for the line. SOME words are longer than the entire line, so we have to break them up manually.
                        if (wordwidth > wrapwidth)
                        {
                            int splitwidth = 0;
                            for (int j = 0; j < word.Count; j++)
                            {
                                splitwidth += GetCharacter(word[j]).Width + 1;
                                if (splitwidth > wrapwidth)
                                {
                                    text = text.Insert(i - word.Count + j - 1, "\n");
                                    word.Insert(j - 1, '\n');
                                    j--;
                                    splitwidth = 0;
                                }
                            }
                            i = i - word.Count - 1;
                            word.Clear();
                        }
                        else
                        {
                            // this word is too big, so we insert a \n character before the word... and try again.
                            text = text.Insert(i - word.Count, "\n");
                            i = i - word.Count - 1;
                            word.Clear();
                        }
                    }
                }

                if (c == '\n')
                {
                    if (width > biggestwidth)
                        biggestwidth = width;
                    height += Height;
                    width = 0;
                }
            }

            if (biggestwidth > width)
                width = biggestwidth;
        }
    }
}
