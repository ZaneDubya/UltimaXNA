#region File Description & Usings
//-----------------------------------------------------------------------------
// GUI.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    public interface IGUI
    {
        bool IsMouseOverGUI(Vector2 nPosition);
        void AddChatText(string nText);
        void LoadInWorldGUI();
    }

    public class EngineGUI : DrawableGameComponent, IGUI
    {
        SpriteFont fontArial14;
        SpriteBatch spriteBatch;
        private FormCollection formCollection;
        private bool mDrawForms = false;
        public string DebugMessage;
        private Texture2D surfaceCursors;

        Window m_ChatWindow;
        Window m_LoginWindow;
        Window m_LoginWindowBG;

        public EngineGUI(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGUI), this);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Events.Initialize(Game.Services);
            GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            formCollection = new FormCollection(Game.Window, Game.Services, ref graphics, @"..\..\res\");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            InitializeForms(graphics);

            surfaceCursors = Texture2D.FromFile(graphics.GraphicsDevice, @"..\..\res\GUI\cursors.png");

            fontArial14 = Game.Content.Load<SpriteFont>(@"fonts\ArialNarrow10");
        }

        protected override void UnloadContent()
        {
            if (mDrawForms)
            {
                //Dispose of the form collection
                formCollection.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            FormCollection.Cursor.Texture = surfaceCursors;
            FormCollection.Cursor.SourceRect = new Rectangle(0, 32, 32, 32);
            FormCollection.Cursor.HasShadow = true;

            if (mDrawForms)
            {
                //Update the form collection
                formCollection.Update(gameTime);
                //Render the form collection (required before drawing)
                formCollection.Render();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the form collection
            if (mDrawForms)
                formCollection.Draw();

            // Draw debug message
            spriteBatch.Begin();
            if (DebugMessage != null)
                spriteBatch.DrawString(fontArial14, DebugMessage,
                    new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsMouseOverGUI(Vector2 nPosition)
        {
            foreach (Form f in FormCollection.Forms)
            {
                if (!f.IsDisposed)
                    if (f.IsMouseOver())
                        return true;
            }
            return false;
        }

        public void AddChatText(string nText)
        {
            ((Window_Chat)m_ChatWindow).AddText(nText);
        }

        public void LoadInWorldGUI()
        {
            m_LoginWindow.Unload();
            m_LoginWindowBG.Unload();
        }

        public void InitializeForms(GraphicsDeviceManager nManager)
        {

            m_ChatWindow = new Window_Chat(formCollection);

            m_LoginWindowBG = new Window_LoginBG(formCollection);
            m_LoginWindow = new Window_Login(formCollection);
            

            #region MessageBox tests
            /*
            MessageBox msgbox0 = new MessageBox(new Vector2(300, 100), new Vector2(470, 40), "MessageBox OK", "hello world!", MessageBox.Type.MB_OK);
            msgbox0.OnOk = msgbox0_OnOk;
            MessageBox msgbox1 = new MessageBox(new Vector2(300, 100), new Vector2(30, 470), "MessageBox OKCancel", "hello world!", MessageBox.Type.MB_OKCANCEL);
            msgbox1.OnOk = msgbox1_OnOk;
            msgbox1.OnCancel = msgbox1_OnCancel;
            MessageBox msgbox2 = new MessageBox(new Vector2(300, 100), new Vector2(280, 420), "MessageBox YesNo", "hello world?", MessageBox.Type.MB_YESNO);
            msgbox2.OnYes = msgbox2_OnYes;
            msgbox2.OnNo = msgbox2_OnNo;
            MessageBox msgbox3 = new MessageBox(new Vector2(300, 100), new Vector2(300, 460), "MessageBox YesNoCancel", "hello world?", MessageBox.Type.MB_YESNOCANCEL);
            msgbox3.OnYes = msgbox3_OnYes;
            msgbox3.OnNo = msgbox3_OnNo;
            msgbox3.OnCancel = msgbox3_OnCancel;
            MessageBox msgbox4 = new MessageBox(new Vector2(300, 100), new Vector2(350, 400), "MessageBox InputOK", "Enter hello to world", MessageBox.Type.MB_INPUTOK);
            msgbox4.OnOk = msgbox4_OnOk;
            MessageBox msgbox5 = new MessageBox(new Vector2(300, 100), new Vector2(380, 440), "MessageBox InputOKCancel", "Enter hello to world", MessageBox.Type.MB_INPUTOKCANCEL);
            msgbox5.OnOk = msgbox5_OnOk;
            msgbox5.OnCancel = msgbox5_OnCancel;
            */
            #endregion
            #region Form #1
            /*
            //Create a new form
            formCollection.Add(new Form("form1", "Form #1", new Vector2(600, 400), new Vector2(10, 10), Form.BorderStyle.Sizable));
            //formCollection["form1"].Style = Form.BorderStyle.None;
            //formCollection["form1"].DrawTitleBar = false;
            //formCollection["form1"].BorderName = null;

            //formCollection["form1"].BackColor = new Color(0, 0, 0, .3f);
            //formCollection["form1"].DrawShadow = false;
            //formCollection["form1"].FontName = "pericles9";
            //formCollection["form1"].FontName = "kootenay9";

            //Add a Button
            formCollection["form1"].Controls.Add(new Button("button1", new Vector2(15, 50), 130, "Button1", Color.White, Color.Black));
            formCollection["form1"]["button1"].OnPress += Button1_OnPress;
            formCollection["form1"]["button1"].OnRelease = Button1_OnRelease;

            formCollection["form1"].Controls.Add(new Button("btRemove", new Vector2(15, 320), 60, "Remove Listbox Item", Color.White, Color.Black));
            formCollection["form1"]["btRemove"].OnPress += Remove_OnPress;

            //Add a Button Row
            formCollection["form1"].Controls.Add(new ButtonRow("buttonRow1", new Vector2(15, 80), 250, new string[] { "RowButton1", "RowButton2", "RowButton3" }, Color.White, Color.Black));
            formCollection["form1"]["buttonRow1"].OnRelease = ButtonRow1_OnRelease;

            //Add a Checkbox
            formCollection["form1"].Controls.Add(new Checkbox("checkbox1", new Vector2(15, 110), "Checkbox", true));
            formCollection["form1"]["checkbox1"].OnRelease = Checkbox1_OnRelease;

            //Add a RadioButton
            formCollection["form1"].Controls.Add(new RadioButton("radiobutton1", new Vector2(170, 55), "RadioButton", true));
            formCollection["form1"]["radiobutton1"].OnRelease = Radiobutton1_OnRelease;

            //Add a Label
            formCollection["form1"].Controls.Add(new Label("label1", new Vector2(15, 135), "Label #1", Color.White, Color.Black, 70, Label.Align.Left));
            formCollection["form1"]["label1"].OnMouseOver = Label1_MouseOver;
            formCollection["form1"]["label1"].OnMouseOut = Label1_MouseOut;
            formCollection["form1"]["label1"].OnPress = Label1_OnPress;
            formCollection["form1"]["label1"].OnRelease = Label1_OnRelease;

            //Add a PictureBox
            formCollection["form1"].Controls.Add(new PictureBox("picturebox1", new Vector2(15, 160), @"xwinforms\textures\xna_logo.png", 1));
            formCollection["form1"]["picturebox1"].OnMouseOver = PictureBox1_MouseOver;
            formCollection["form1"]["picturebox1"].OnMouseOut = PictureBox1_MouseOut;
            formCollection["form1"]["picturebox1"].OnPress = PictureBox1_OnPress;
            formCollection["form1"]["picturebox1"].OnRelease = PictureBox1_OnRelease;

            //Add a CheckboxGroup
            formCollection["form1"].Controls.Add(new CheckboxGroup("checkboxgroup1", new Checkbox[] { 
                new Checkbox("group1check1", new Vector2(165, 130), "Group Check #1", true),
                new Checkbox("group1check2", new Vector2(165, 150), "Group Check #2", false),
                new Checkbox("group1check3", new Vector2(165, 170), "Group Check #3", false),
                new Checkbox("group1check4", new Vector2(165, 190), "Group Check #4", false),
                new Checkbox("group1check5", new Vector2(165, 210), "Group Check #5", false)}));
            ((CheckboxGroup)formCollection["form1"]["checkboxgroup1"]).OnChangeSelection = CheckboxGroup1_ChangeSelection;

            //Add a RadioButtonGroup
            formCollection["form1"].Controls.Add(new RadiuButtonGroup("radiobuttongroup1", new RadioButton[] { 
                new RadioButton("group1check1", new Vector2(310, 280), "RadioButton #1", true),
                new RadioButton("group1check2", new Vector2(310, 300), "RadioButton #2", false),
                new RadioButton("group1check3", new Vector2(310, 320), "RadioButton #3", false),
                new RadioButton("group1check4", new Vector2(310, 340), "RadioButton #4", false),
                new RadioButton("group1check5", new Vector2(310, 360), "RadioButton #5", false)}));
            ((RadiuButtonGroup)formCollection["form1"]["radiobuttongroup1"]).OnChangeSelection = RadioButtonGroup1_ChangeSelection;

            //Add a ButtonGroup
            formCollection["form1"].Controls.Add(new ButtonGroup("buttongroup1", new Button[] { 
                new Button("group2button1", new Vector2(165, 250), "Group Button #1", Color.White, Color.Black),
                new Button("group2button2", new Vector2(165, 275), "Group Button #2", Color.White, Color.Black),
                new Button("group2button3", new Vector2(165, 300), "Group Button #3", Color.White, Color.Black),
                new Button("group2button4", new Vector2(165, 325), "Group Button #4", Color.White, Color.Black),
                new Button("group2button5", new Vector2(165, 350), "Group Button #5", Color.White, Color.Black)}));
            ((ButtonGroup)formCollection["form1"]["buttongroup1"]).OnChangeSelection = ButtonGroup1_ChangeSelection;

            //Add a multiline Textbox
            formCollection["form1"].Controls.Add(new Textbox("textbox1", new Vector2(310, 50), 130, 80, "This is a test"));
            ((Textbox)formCollection["form1"]["textbox1"]).Scrollbar = Textbox.Scrollbars.Both;

            //Add a Listbox
            formCollection["form1"].Controls.Add(new Listbox("listbox1", new Vector2(310, 150), 130, 120,
                new string[] { "This is item #1 from the listbox", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Item8", "Item9", "Item10" }));
            ((Listbox)formCollection["form1"]["listbox1"]).HorizontalScrollbar = true;

            //Add a menu to the form
            //Note: inverted process, first create a submenu then add it when creating the menuItem
            #region Form1 Menu
            
            SubMenu mnuFile = new SubMenu(formCollection["form1"]);
            mnuFile.Add(new MenuItem("mnuFileClose", "&Close", Form1_mnuFileClose), null);
            mnuFile.Add(new MenuItem("", "-", Form1_mnuFileClose), null);
            mnuFile.Add(new MenuItem("mnuFileExit", "E&xit", Form1_mnuFileExit), null);

            SubMenu mnuView = new SubMenu(formCollection["form1"]);
            mnuView.Add(new MenuItem("mnuViewToggleFS", "&Toggle Fullscreen", Form1_mnuViewToggleFS), null);

            SubMenu mnuTestSubMenu0 = new SubMenu(formCollection["form1"]);
            mnuTestSubMenu0.Add(new MenuItem("mnuTestSubItem0", "SubMenuItem0", null), null);
            mnuTestSubMenu0.Add(new MenuItem("mnuTestSubItem1", "SubMenuItem1", null), null);

            SubMenu mnuTestSubMenu1 = new SubMenu(formCollection["form1"]);
            mnuTestSubMenu1.Add(new MenuItem("mnuTestSubItem0", "SubMenuItem0", null), null);
            mnuTestSubMenu1.Add(new MenuItem("mnuTestSubItem1", "SubMenuItem1", null), null);

            SubMenu mnuTest = new SubMenu(formCollection["form1"]);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem0", null), mnuTestSubMenu0);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem1", null), mnuTestSubMenu1);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem2", null), null);

            formCollection["form1"].Menu = new Menu("form1Menu");
            formCollection["form1"].Menu.Add(new MenuItem("mnuFile", "&File", null), mnuFile);
            formCollection["form1"].Menu.Add(new MenuItem("mnuView", "&View", null), mnuView);
            formCollection["form1"].Menu.Add(new MenuItem("mnuView", "&Test", null), mnuTest);
            
            #endregion

            //Add a ProgressBar
            formCollection["form1"].Controls.Add(new Progressbar("progressbar1", new Vector2(15, 295), 125, 15, true));

            //Add a Potentiometer
            formCollection["form1"].Controls.Add(new Potentiometer("potentiometer1", new Vector2(120, 135)));
            ((Potentiometer)formCollection["form1"]["potentiometer1"]).OnChangeValue = Potentiometer_OnChangeValue;

            formCollection["form1"].Controls.Add(new ComboBox("combo1", new Vector2(450, 50), 120, new string[] { "Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Item8" }));
            formCollection["form1"]["combo1"].FontName = "pescadero9b";

            formCollection["form1"].Controls.Add(new Button("btAdd", new Vector2(440, 100), "Add to Listbox", Color.White, Color.Black));
            formCollection["form1"]["btAdd"].OnPress = Button1_OnPress;

            //Show the form
            formCollection["form1"].Show();
            */
            #endregion
            
            mDrawForms = true;
        }
        #region Form Controls Events

        private void Button1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Pressed!");
            //((Listbox)formCollection["form1"]["listbox1"]).Add(formCollection["form1"].Controls["textbox1"].Text);
        }
        private void Button1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Released!");
        }

        private void Remove_OnPress(object obj, EventArgs e)
        {
            Listbox listbox1 = (Listbox)formCollection["form1"]["listbox1"];
            listbox1.RemoveAt(listbox1.SelectedIndex);
        }

        private void ButtonRow1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Selected Index : " + (formCollection["form1"]["buttonRow1"] as ButtonRow).SelectedIndex);
            System.Diagnostics.Debug.WriteLine("Selected Item  : " + formCollection["form1"]["buttonRow1"].Text);
        }

        private void Checkbox1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Checkbox Value: " + ((Checkbox)obj).Value);
        }

        private void Radiobutton1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RadioButton Value: " + ((RadioButton)obj).Value);
        }

        private void Label1_MouseOver(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnMouseOver");
        }
        private void Label1_MouseOut(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnMouseOut");
        }
        private void Label1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnPress");
        }
        private void Label1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnRelease");
        }

        private void PictureBox1_MouseOver(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnMouseOver");
        }
        private void PictureBox1_MouseOut(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnMouseOut");
        }
        private void PictureBox1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnPress");
        }
        private void PictureBox1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnRelease");
        }

        private void Potentiometer_OnChangeValue(object obj, EventArgs e)
        {
            ((Progressbar)formCollection["form1"].Controls["progressbar1"]).Value = (int)obj;
        }

        private void Form1_mnuFileClose(object obj, EventArgs e)
        {
            formCollection["form1"].Close();
        }
        private void Form1_mnuFileExit(object obj, EventArgs e)
        {
            GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            if (graphics.IsFullScreen)
                graphics.ToggleFullScreen();

            Game.Exit();
        }
        private void Form1_mnuViewToggleFS(object obj, EventArgs e)
        {
            GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            graphics.ToggleFullScreen();
        }

        private void CheckboxGroup1_ChangeSelection(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CheckboxGroup1.OnChangeSelection : " + (string)obj);
        }

        private void RadioButtonGroup1_ChangeSelection(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RadioButtonGroup1.OnChangeSelection : " + (string)obj);
        }

        private void ButtonGroup1_ChangeSelection(object obj, EventArgs e)
        {
            string selection = obj as string;
            System.Diagnostics.Debug.WriteLine("ButtonGroup1.OnChangeSelection : " + selection);
        }

        private void Listview_ColumnHeader0_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListView ColumnHeader0 Pressed");
        }
        private void Listview_ColumnHeader0_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListView ColumnHeader0 Released");
        }

        private void msgbox0_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox0 OK");
            System.Diagnostics.Debug.WriteLine("MessageBox0 OK");
        }
        private void msgbox1_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox1 OK");
            System.Diagnostics.Debug.WriteLine("MessageBox1 OK");
        }
        private void msgbox1_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox1 Cancel");
            System.Diagnostics.Debug.WriteLine("MessageBox1 Cancel");
        }
        private void msgbox2_OnYes(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox2 Yes");
            System.Diagnostics.Debug.WriteLine("MessageBox2 Yes");
        }
        private void msgbox2_OnNo(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox2 No");
            System.Diagnostics.Debug.WriteLine("MessageBox2 No");
        }
        private void msgbox3_OnYes(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 Yes");
            System.Diagnostics.Debug.WriteLine("MessageBox3 Yes");
        }
        private void msgbox3_OnNo(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 No");
            System.Diagnostics.Debug.WriteLine("MessageBox3 No");
        }
        private void msgbox3_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 Cancel");
            System.Diagnostics.Debug.WriteLine("MessageBox3 Cancel");
        }
        private void msgbox4_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox4 OK");
            //frmDebug.WriteLine("MessageBox4 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox4 OK, Output : " + (string)obj);
        }
        private void msgbox5_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox5 OK");
            //frmDebug.WriteLine("MessageBox5 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox5 OK, Output : " + (string)obj);
        }
        private void msgbox5_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox5 Cancel");
            //frmDebug.WriteLine("MessageBox5 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox5 Cancel, Output: " + (string)obj);
        }

        #endregion
    }
}