using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InterXLib.Display;
using InterXLib.XGUI;
using InterXLib.Input;

namespace InterXLib
{
    public abstract class BaseCore : Microsoft.Xna.Framework.Game
    {
        System.Drawing.Graphics m_SystemGraphics;
        GraphicsDeviceManager m_Graphics;
        YSpriteBatch m_SpriteBatch;
        GUIManager m_GUI;

        protected bool m_DefaultCoreInitialization = false;

        protected YSpriteBatch SpriteBatch
        {
            get
            {
                return m_SpriteBatch;
            }
        }

        InputState m_Input;
        protected InputState Input
        {
            get { return m_Input; }
        }

        ActionBinder m_Actions;
        protected ActionBinder Actions
        {
            get { return m_Actions; }
        }

        Settings m_Settings;

        protected View.ThirdPersonCamera Camera
        {
            get { return Library.Projections.Camera; }
        }

        protected GUIManager GUIManager
        {
            get { return m_GUI; }
        }

        protected abstract void CoreInitialize();
        protected abstract void CoreUpdate(double totalEngineSeconds, double frameEngineSeconds);
        protected virtual void CoreDraw(double frameTime) { }
        protected virtual void CoreBeforeDraw() { }
        protected virtual void CoreAfterDraw() { }

        public BaseCore()
        {
            m_SystemGraphics = System.Drawing.Graphics.FromHwnd(this.Window.Handle);
            Settings.ScreenDPI = new Vector2(m_SystemGraphics.DpiX / 96f, m_SystemGraphics.DpiY / 96f);

            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.IsFullScreen = false;
            m_Graphics.PreferredBackBufferWidth = 256;
            m_Graphics.PreferredBackBufferHeight = 240;
            m_Graphics.SynchronizeWithVerticalRetrace = false;

            m_Input = new InterXLib.InputState();
            m_Input.Initialize(this.Window.Handle);
            m_Actions = new ActionBinder();

            m_SpriteBatch = new InterXLib.Display.YSpriteBatch(this);
            this.Components.Add(m_SpriteBatch);

            m_GUI = new GUIManager();
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            m_Settings = new InterXLib.Settings();
            InterXLib.Library.Initialize(m_Graphics.GraphicsDevice, m_Input);

            InternalDefaultInitialization(m_DefaultCoreInitialization);
            CoreInitialize();
            base.Initialize();
        }

        protected void InternalDefaultInitialization(bool initialize_gui)
        {
            string font_name = @"CenturyGothic";
            string base_path = @"Content\InterXLibContent\";

            Content.RootDirectory = "";
            SpriteBatch.SpriteBatchShader = Content.Load<Effect>(base_path + "SpriteBatchX");

            if (initialize_gui)
            {
                GUIManager.DefaultSkinName = "XGUI";
                GUIManager.DefaultFontName = font_name;
                GUIManager.AddFont("CenturyGothic", YSpriteFont.LoadFontDF(GraphicsDevice,
                    base_path + @"CenturyGothic.xnb",
                    base_path + @"CenturyGothicXML.xml", 50f));
                GUIManager.AddSkin("XGUI", new InterXLib.XGUI.Rendering.Skin(
                    Content.Load<Texture2D>(base_path + @"GUITexture"),
                    new System.IO.StreamReader(base_path + @"GUIRenderers.txt"),
                    font_name));
            }
        }

        protected override void LoadContent()
        {
            
        }

        protected override void UnloadContent()
        {
            m_SpriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Profiler.InContext("OutOfContext"))
                Profiler.ExitContext("OutOfContext");
            Profiler.EnterContext("Update");

            /*Console.WriteLine("Update: {0} (+{1})",
                gameTime.TotalGameTime.TotalSeconds,
                gameTime.ElapsedGameTime.TotalSeconds);*/

            updateTime(gameTime);

            m_Input.Update(m_TotalTime, m_ElapsedTime);

            // DEBUG: Ctrl-ESC => Exit.
            if (m_Input.HandleKeyboardEvent(
                InterXLib.Input.Windows.KeyboardEventType.Press,
                InterXLib.Input.Windows.WinKeys.Escape, false, false, true))
                this.Exit();

            m_Actions.Update((float)m_ElapsedTime);
            m_Actions.ReceiveKeyboardInput(m_Input.GetKeyboardEvents());

            if (m_Settings.HasUpdates)
                handleUpdates();

            // GUIManager blocks input for other systems.
            GUIManager.ReceiveKeyboardInput(Input.GetKeyboardEvents());
            GUIManager.ReceiveMouseInput(Input.MousePosition, Input.GetMouseEvents());

            base.Update(gameTime);

            CoreUpdate(m_TotalTime, m_ElapsedTime);

            // Update GUI last as it may be updated by other systems.
            GUIManager.Update(m_TotalTime, m_ElapsedTime);

            Profiler.ExitContext("Update");
            Profiler.EnterContext("OutOfContext");
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.EndFrame();
            Profiler.BeginFrame();
            if (Profiler.InContext("OutOfContext"))
                Profiler.ExitContext("OutOfContext");
            Profiler.EnterContext("RenderFrame");

            /*Console.WriteLine("Draw: {0} (+{1})",
                    gameTime.TotalGameTime.TotalSeconds,
                    gameTime.ElapsedGameTime.TotalSeconds);*/

            double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds / 1000D;
            CoreBeforeDraw();
            CoreDraw(frameTime);
            m_GUI.Draw(SpriteBatch, frameTime);
            CoreAfterDraw();
            base.Draw(gameTime);

            Profiler.ExitContext("RenderFrame");
            Profiler.EnterContext("OutOfContext");

            double frame_time_drawing = Profiler.GetContext("RenderFrame").TimeSpent * 1000d;
            double frame_time_updating = Profiler.GetContext("Update").TimeSpent * 1000d;
            double frame_time = Profiler.TotalTimeMS;
            double last_draw_ms = Profiler.GetContext("RenderFrame").AverageOfLast60Times * 1000d;
            double other_time = Profiler.GetContext("OutOfContext").TimeSpent * 1000d;

            double total_time_check = other_time + frame_time_drawing + frame_time_updating;

            this.Window.Title = string.Format("DrawTime:{0:0.00}% UpdateTime:{1:0.00}% AvgDraw:{2:0.00}ms {3}",
                100d * (frame_time_drawing / frame_time),
                100d * (frame_time_updating / frame_time),
                last_draw_ms,
                gameTime.IsRunningSlowly ? "IsRunningSlowly" : string.Empty);
        }

        private double m_TotalTime, m_ElapsedTime;
        private void updateTime(GameTime gameTime)
        {
            m_TotalTime = gameTime.TotalGameTime.TotalMilliseconds / 1000D;
            m_ElapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds / 1000D;
        }

        private void handleUpdates()
        {
            InterXLib.Settings.Setting s;
            while ((s = m_Settings.GetNextUpdate()) != InterXLib.Settings.Setting.None)
            {
                switch (s)
                {
                    case InterXLib.Settings.Setting.Resolution:
                        m_Graphics.PreferredBackBufferWidth = (int)(Settings.Resolution.X * Settings.ScreenDPI.X);
                        m_Graphics.PreferredBackBufferHeight = (int)(Settings.Resolution.Y * Settings.ScreenDPI.X);
                        m_Graphics.ApplyChanges();
                        // if (m_GUI != null) { m_GUI.OnResize(); }
                        break;
                    default:
                        Logging.Fatal("Setting not handled.");
                        break;
                }
            }
        }

        protected void EnableDragAndDrop(Action<string> drag_over, Action<string> drag_drop)
        {
            System.Windows.Forms.Form gameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            gameForm.AllowDrop = true;
            gameForm.DragEnter += DragDrop_Over;
            gameForm.DragDrop += DragDrop_Drop;
            m_DragOver += drag_over;
            m_DragDrop += drag_drop;
        }

        private Action<string> m_DragOver, m_DragDrop;

        private void DragDrop_Over(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            InternalDragEvent(e, m_DragOver);
        }

        private void DragDrop_Drop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            InternalDragEvent(e, m_DragDrop);
        }

        private void InternalDragEvent(System.Windows.Forms.DragEventArgs e, Action<string> action)
        {
            string[] formats = e.Data.GetFormats();
            for (int i = 0; i < formats.Length; i++)
            {
                if (formats[i] == "FileNameW")
                {
                    object data = e.Data.GetData(formats[i]);
                    string file_name = ((string[])data)[0];
                    action.Invoke(file_name);
                    break;
                }
            }
        }
    }
}
