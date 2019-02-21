using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication4
{
    public partial class FromMain : Form
    {
        private Game game;
        public FromMain()
        {
            InitializeComponent();
            
            TemplateArray brickTemplate = new TemplateArray();
            brickTemplate.add("0000001000011100000000000", Color.FromArgb(-128));
            brickTemplate.add("0000000000111100000000000", Color.FromArgb(-65536));
            brickTemplate.add("0000000110011000000000000", Color.FromArgb(-16711936));
            brickTemplate.add("0000000100011100000000000", Color.FromArgb(-4144960));
            brickTemplate.add("0000000100011000100000000", Color.FromArgb(-16776961));
            brickTemplate.add("0000000000011100100000000", Color.FromArgb(-65281));
            brickTemplate.add("0000000000011000110000000", Color.FromArgb(-8323073));

            game = new Game(15, 25, brickTemplate, 20, Color.Black,
                mainPalette.CreateGraphics(),
                nextPalette.CreateGraphics(),
                9, Color.Blue, false);

            game.Start();                
        }

        private void mainPalette_Paint(object sender, PaintEventArgs e)
        {
            game.paintPalette(e.Graphics);
        }

        private void nextPalette_Paint(object sender, PaintEventArgs e)
        {
            game.paintNext(e.Graphics);
        }

        private void FromMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                game.moveLeft();
            else if (e.KeyCode == Keys.Right)
                game.moveRight();
            else if (e.KeyCode == Keys.Down)
                game.moveDown();
            else if (e.KeyCode == Keys.Up)
                game.clockRotate();
            else if (e.KeyCode == Keys.Space)
                game.dropDown();
        }


        
        


    }
}
