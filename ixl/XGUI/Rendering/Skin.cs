using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.IO;

namespace InterXLib.XGUI.Rendering
{
    public class Skin
    {
        internal Texture2D Texture { get; private set; }
        internal Dictionary<string, ARenderer> Renderers { get; private set; }

        public Skin(Texture2D texture, StreamReader image_map, string font)
        {
            Renderers = new Dictionary<string, ARenderer>();

            Texture = texture;
            string map = image_map.ReadToEnd();
            map = map.Replace("\r", String.Empty);

            foreach (string line in map.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] equalsSplit = line.Split('=');
                string[] optionSplit = equalsSplit[1].Split(',');

                Type rendererType = typeof(ARenderer);
                List<Type> types = new List<Type>();
                foreach (var assemply in AppDomain.CurrentDomain.GetAssemblies())
                {
                    types.AddRange(assemply.GetTypes().Where(t => t != rendererType && rendererType.IsAssignableFrom(t)));
                }

                foreach (var renderer in types)
                {
                    if (renderer.Name != (optionSplit[1]).Trim())
                        continue;

                    string[] rectangleSplit = (optionSplit[0]).Trim().Split(' ');
                    Rectangle area = new Rectangle(
                        Int32.Parse(rectangleSplit[0]),
                        Int32.Parse(rectangleSplit[1]),
                        Int32.Parse(rectangleSplit[2]),
                        Int32.Parse(rectangleSplit[3]));

                    List<Type> argTypes = new List<Type> { typeof(Texture2D), typeof(Rectangle) };
                    for (int x = 2; x < optionSplit.Length; x++)
                        argTypes.Add(typeof(int));

                    List<object> args = new List<object> { texture, area };
                    for (int x = 2; x < optionSplit.Length; x++)
                        args.Add(int.Parse(optionSplit[x]));

                    ConstructorInfo ctor = renderer.GetConstructor(argTypes.ToArray());
                    if (ctor != null)
                        Renderers.Add((equalsSplit[0]).Trim(), (ARenderer)ctor.Invoke(args.ToArray()));
                }
            }
        }
    }
}
