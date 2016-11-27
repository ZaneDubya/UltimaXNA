using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Threading.Tasks;
using UltimaXNA;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Core.Resources;

namespace ExamplePlugin
{
    class DebugOutputGumpsModule : IModule {
        public string Name => "DebugOutputGumpsModule";
        Task m_WriteGumps;
        CancellationTokenSource m_WriteGumpsCancel;

        public void Load() {
            // LoginGump.AddButton("Debug:Gumps", OnClickDebugGump);
        }

        public void Unload() {
            // LoginGump.RemoveButton(OnClickDebugGump);
        }

        void OnClickDebugGump() {
            if (m_WriteGumps == null) {
                Tracer.Info("Writing all gump textures to /Gumps folder. Progress may be slow. Click Debug:Gumps button again to cancel.");
                // This is ugly! I have no idea how to do parallelism. Someone help me?
                m_WriteGumpsCancel = new CancellationTokenSource();
                m_WriteGumps = new Task(
                    () => {
                        System.IO.Directory.CreateDirectory("Gumps");
                        IResourceProvider resources = Service.Get<IResourceProvider>();
                        for (int i = 0; i < 0x10000; i++) {
                            Texture2D texture = resources.GetUITexture(i);
                            if (texture != null) {
                                texture.SaveAsJpeg(new System.IO.FileStream($"Gumps/{i:D6}.jpg", System.IO.FileMode.Create), texture.Width, texture.Height);
                            }
                            if (m_WriteGumpsCancel.Token.IsCancellationRequested) {
                                break;
                            }
                        }
                        m_WriteGumpsCancel = null;
                        m_WriteGumps = null;
                    }, m_WriteGumpsCancel.Token);
                m_WriteGumps.Start();
            }
            else {
                Tracer.Info("Canceled writing gump textures.");
                m_WriteGumpsCancel.Cancel();
            }
        }
    }
}
