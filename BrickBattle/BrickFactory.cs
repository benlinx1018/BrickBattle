using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace WindowsFormsApplication4
{
    class BrickFactory
    {
        private TemplateArray brickArrray;//磚塊模組列表
        private Color backGroundColor;//磚塊背景色
        private int rectPix;//單位矩陣像素寬度
        //初始化磚塊產生工廠基本設定
        public BrickFactory(TemplateArray info, Color bgColor, int pix)
        {
            brickArrray = info;
            rectPix = pix;
            backGroundColor = bgColor;
        }
        public Brick CreateBrick()
        {
            Random rd = new Random();
            int index = rd.Next(brickArrray.Count);
            string bcode = brickArrray[index].BrickCode;
            List<Point> pointList = new List<Point>();
            for (int i = 0; i < bcode.Length; i++)
            {
                if (bcode[i] == '1')
                {
                    Point p = new Point(i % 5, i / 5);
                    p.offSet(-2,- 2);
                    pointList.Add(p);
                }
            }
            Brick brick = new Brick(pointList.ToArray(), brickArrray[index].BrickColor, backGroundColor, rectPix);
            return brick;
        }
    }
}
