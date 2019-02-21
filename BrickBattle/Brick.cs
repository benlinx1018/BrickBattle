using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace WindowsFormsApplication4
{
    class Brick
    {
        private Point[] m_Point;
        private int centerX;//中心位置X軸座標
        private int centerY;//中心位置Y軸座標
        private Color brickColor;//磚塊顏色
        private Color backGroundColor;//背景色
        private int blockSize;//單位格的像素
        private SolidBrush brush;//繪圖筆
        public Color Color
        {
            get { return brickColor; }
        }
        public Point[] Points
        {
            get { return m_Point; }
        }
        public int X
        {
            get { return centerX; }
            set { centerX = value; }
        }
        public int Y
        {
            get { return centerY; }
            set {centerY = value;}
        }
        public Brick(Point[] sa,Color color,Color bgColor,int size)//方塊建立
        {
            brickColor = color;
            backGroundColor = bgColor;
            blockSize = size;
            m_Point = sa;
            brush = new SolidBrush(brickColor);
            centerX = 2;//
            centerY = 2;//預設(2,2)可以直接顯示完整方塊
        }

        //轉換單點成單位矩形
        private Rectangle PointToRect(Point p)
        {
            return new Rectangle((centerX + p.X) * blockSize + 1,
                                 (centerY + p.Y) * blockSize + 1,
                                 blockSize - 2,
                                 blockSize - 2);
        }
        //順時針旋轉
        public void ClockRotate()
        {
            int temp;
            for (int i = 0; i < m_Point.Length; i++)
            {
                temp = m_Point[i].X;
                m_Point[i].X = -m_Point[i].Y;
                m_Point[i].Y = temp;
            }
        }
        //逆時針旋轉
        public void AntiClockWise()
        {
            int temp;
            for (int i = 0; i < m_Point.Length; i++)
            {
                temp = m_Point[i].X;
                m_Point[i].X = m_Point[i].Y;
                m_Point[i].Y = -temp;
            }
        }
        //繪製磚塊
        public void Paint(Graphics gp)
        {
            foreach (Point p in m_Point)
            {
                lock (gp)
                {
                    try
                    {
                        gp.FillRectangle(brush, PointToRect(p));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
        //清除磚塊  背景色填上
        public void Erase(Graphics gp)
        {
            using (SolidBrush sb = new SolidBrush(backGroundColor))
            {
                foreach (Point p in m_Point)
                {
                    try
                    {
                        gp.FillRectangle(sb, PointToRect(p));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }
            }
        }
    
    
    
    }

}
