using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace WindowsFormsApplication4
{
    class BrickTemplate//磚塊模組
    {
        private string brickCode;//磚塊資訊
        private Color brickColor;//磚塊顏色
        public BrickTemplate(string bc,Color c)
        {
            brickCode = bc;
            brickColor = c;
        }
        public string BrickCode
        {
            get { return brickCode; }
        }
        public Color BrickColor
        {
            get { return brickColor; }
        }
    }
}
