using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UltimaXNA.WinForms
{
    public partial class ResolutionSet : Form
    {
        public ResolutionSet()
        {
            InitializeComponent();
        }

        public int getWidth()
        {
            int width = Int32.Parse(widthBox.Text);
            return width;
        }
        public int getHeight()
        {
            int height = Int32.Parse(heightBox.Text);
            return height;
        }
    }
}
