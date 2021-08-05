using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareDemo
{
    public partial class CatalogueForm : Form
    {
        public CatalogueForm()
        {
            InitializeComponent();
        }

        private void CatalogueForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ControlTransparent().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ControlExStyle().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new ControlDirectUI().Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new FormTransparentKey().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new FormLayered().Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new FormLayeredDirectUI().Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FormAnimationControl().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new FormAnimationDraw().Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new FormAnimationTween().Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new WindowHwndHost().Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            new WindowAllowTranspartent().Show();
        }
    }
}
