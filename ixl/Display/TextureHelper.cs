using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InterXLib.Display
{
    public class TextureHelper
    {
        public static bool LoadTextureFromFile(GraphicsDevice graphics, string filepath, out Texture2D texture)
        {
            texture = null;

            // Open a Stream and decode a PNG image
            if (Path.GetExtension(filepath).ToLower() == ".png")
            {
                try
                {
                    using (FileStream stream = new FileStream(filepath, FileMode.Open))
                    {
                        PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.None);
                        BitmapSource frame = decoder.Frames[0];

                        bool is_rgb = false;
                        bool has_alpha = false;
                        int stride = 0;

                        switch (frame.Format.BitsPerPixel)
                        {
                            case 8:
                                is_rgb = false;
                                has_alpha = false;
                                stride = 1;
                                break;
                            case 24:
                                is_rgb = true;
                                has_alpha = false;
                                stride = 3;
                                break;
                            case 32:
                                is_rgb = true;
                                has_alpha = true;
                                stride = 4;
                                break;
                            default:
                                break;
                        }

                        texture = new Texture2D(graphics, (int)frame.PixelWidth, (int)frame.PixelHeight);
                        uint[] texture_data = new uint[texture.Width * texture.Height];

                        byte[] png_data = new byte[texture.Width * texture.Height * stride];
                        frame.CopyPixels(png_data, texture.Width * stride, 0);

                        int i = 0, j = 0;
                        for (int y = 0; y < texture.Height; y++)
                        {
                            for (int x = 0; x < texture.Width; x++)
                            {
                                if (is_rgb)
                                {
                                    uint b = (uint)png_data[j++];
                                    uint g = (uint)png_data[j++];
                                    uint r = (uint)png_data[j++];
                                    uint a = (has_alpha) ? (uint)png_data[j++] : (uint)0xFF;

                                    float fl_a = (float)a / (float)0xFF;

                                    texture_data[i] |= (uint)((float)r * fl_a);
                                    texture_data[i] |= (uint)((float)g * fl_a) << 8;
                                    texture_data[i] |= (uint)((float)b * fl_a) << 16;
                                    texture_data[i] |= (uint)(a << 24);
                                }
                                else
                                {
                                    int palette_index = png_data[j++];
                                    Color pal_color = frame.Palette.Colors[palette_index];
                                    float fl_a = (float)pal_color.A / (float)0xFF;

                                    texture_data[i] |= (uint)((float)pal_color.R * fl_a);
                                    texture_data[i] |= (uint)((float)pal_color.G * fl_a) << 8;
                                    texture_data[i] |= (uint)((float)pal_color.B * fl_a) << 16;
                                    texture_data[i] |= (uint)(pal_color.A << 24);
                                }
                                i++;
                            }
                        }

                        texture.SetData<uint>(texture_data);
                        return true;
                    }

                }
                catch
                {
                    // try to load using alternate method ...
                    System.Diagnostics.Debug.WriteLine("Attempt to load file using PngBitmapDecoder failed. Falling back to Texture2D.FromStream().");
                }
            }

            // Stream imageStreamSource = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            // BitmapSource bitmapSource = decoder.Frames[0];

            try
            {
                using (FileStream stream = new FileStream(filepath, FileMode.Open))
                {
                    texture = Texture2D.FromStream(graphics, stream);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
