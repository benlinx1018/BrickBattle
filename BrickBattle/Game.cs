using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApplication4
{
    class Game
    {
        #region 常數
        private readonly Color[] COLORS = new Color[] { Color.White, Color.Tomato, Color.Thistle, Color.Turquoise };
        private readonly int[] TIME_SPANS = new int[] { 700, 600, 550, 500, 450, 400, 350, 300, 250, 200 };
        private readonly int[] SCORE_SPANS = new int[] { 100, 300, 500, 1000, 1500 };
        #endregion

        #region 屬性
        private BrickFactory brickFactory;//方塊生產工廠
        private int Width = 15;
        private int Height = 25;
        private Color[,] CoorArray;//遊戲方格
        private Color backGroundColor;//背景色設定
        private Color gridColor;//網格顏色
        private int pixSize;//單位方格像素
        private int Level=0;
        private int Score=0;
        private bool endOfGame = false;
        private bool showGrid;
        private bool pause = false;
        private bool ready = true;

        private Graphics mainPalette;//主畫布
        private Graphics nextPalette;//緩衝畫布
        private Brick runBrick;//當前方塊
        private Brick nextBrick;//下一個方塊
        private System.Timers.Timer gameTimer;
        #endregion
        public Game(int w, int h, TemplateArray info, int pix, Color bgColor, Graphics palette, Graphics next, int lv, Color gridC, bool show)
        {
            Width = w;//初始化寬度
            Height = h;//初始化高度
            CoorArray  = new Color[Width,Height];//初始化所有遊戲方格
            backGroundColor = bgColor;//背景顏色
            mainPalette = palette;//主畫布
            nextPalette = next;
            pixSize = pix;
            Level = lv;
            gridColor = gridC;
            showGrid = show;
            brickFactory = new BrickFactory(info,backGroundColor,pixSize);
        }
        //向下
        public bool moveDown()
        {
            if(runBrick==null)return true;
            int xPos = runBrick.X;//X座標不變
            int yPos = runBrick.Y+1;//Y座標+1
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (yPos + runBrick.Points[i].Y >= Height)
                    return false;
                if (!CoorArray[xPos + runBrick.Points[i].X, yPos + runBrick.Points[i].Y].IsEmpty)
                    return false;
            }
            runBrick.Erase(mainPalette);
            runBrick.Y++;
            runBrick.Paint(mainPalette);
            return true;
        }
        //向左
        public bool moveLeft()
        {
            if (runBrick == null) return true;
            int xPos = runBrick.X -1;//X座標-1
            int yPos = runBrick.Y ;//Y座標
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (xPos + runBrick.Points[i].X <0)
                    return false;
                if (!CoorArray[xPos + runBrick.Points[i].X, yPos + runBrick.Points[i].Y].IsEmpty)
                    return false;
            }
            runBrick.Erase(mainPalette);
            runBrick.X--;
            runBrick.Paint(mainPalette);
            return true;
        }
        //向右
        public bool moveRight()
        {
            if (runBrick == null) return true;
            int xPos = runBrick.X + 1;//X座標+1
            int yPos = runBrick.Y;//Y座標
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (xPos + runBrick.Points[i].X >= Width)
                    return false;
                if (!CoorArray[xPos + runBrick.Points[i].X, yPos + runBrick.Points[i].Y].IsEmpty)
                    return false;
            }
            runBrick.Erase(mainPalette);
            runBrick.X++;
            runBrick.Paint(mainPalette);
            return true;
        }
        //持續向下
        public void dropDown()
        {
            gameTimer.Stop();
            while (moveDown()) ;
            gameTimer.Start();
        }
        //順時針
        public bool clockRotate()
        {
            if(runBrick==null)return true;
            for(int i=0;i<runBrick.Points.Length;i++)
            {
                int x = runBrick.X - runBrick.Points[i].Y;
                int y = runBrick.Y + runBrick.Points[i].X;
                if (x < 0 || x >= Width || y < 0 || y >= Height || !CoorArray[x, y].IsEmpty)
                    return false;

            }
            runBrick.Erase(mainPalette);
            runBrick.ClockRotate();
            runBrick.Paint(mainPalette);
            return true;
        }
        public void paintPalette(Graphics g)
        {
            lock (g)
            {
                g.Clear(backGroundColor);//用背景色清除畫布
            }
            if (showGrid)
                PaintGridLine(g);
            PaintBricks(g);
            if (runBrick != null)
                runBrick.Paint(g);

        }
        public void paintNext(Graphics g)
        {
            lock (g)
            {
                g.Clear(backGroundColor);
            }
            if (nextBrick != null)
                nextBrick.Paint(g);
        }
        //開始遊戲
        public void Start()
        {
            runBrick = brickFactory.CreateBrick();
            runBrick.X = Width / 2;
            int y = 0;
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (runBrick.Points[i].Y < y)
                    y = runBrick.Points[i].Y;
            }
            runBrick.Y = -y;
            paintPalette(mainPalette);
            Thread.Sleep(20);
            nextBrick = brickFactory.CreateBrick();
            paintNext(nextPalette);
            gameTimer = new System.Timers.Timer(TIME_SPANS[Level]);
            gameTimer.Elapsed += new System.Timers.ElapsedEventHandler(onBrickTimedEvent);
            gameTimer.AutoReset = true;
            gameTimer.Start();
        }
        //關閉遊戲
        public void Close()
        {
            if (gameTimer != null)
            {
                gameTimer.Close();
                gameTimer.Dispose();
            }
            nextPalette.Dispose();
            mainPalette.Dispose();
        }
        //更新方塊訊息
        private void onBrickTimedEvent(object sourse, ElapsedEventArgs e)
        {
            if (pause || endOfGame)
            {
                if (endOfGame) paintGameOver();
                return;
            }
            if (ready)
            {
                if (!moveDown()) checkAndOverBrick();
            }
        }
        //畫上已有的磚塊
        public void PaintBricks(Graphics g)
        {
            lock (g)
            {
                for (int row = 0; row < Height; row++)
                {
                    for (int column = 0; column < Width; column++)
                    {
                        try
                        {
                            Color c = CoorArray[column, row];
                            if (c.IsEmpty) c = backGroundColor;
                            using (SolidBrush sb = new SolidBrush(c))
                            {
                                g.FillRectangle(sb, column * pixSize + 1, row * pixSize + 1, pixSize - 2, pixSize - 2);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
        }
        //畫上格線
        private void PaintGridLine(Graphics g)
        {
            try
            {
                lock (g)
                {
                    using (Pen p = new Pen(gridColor,1))
                    {
                        //網格橫線
                        for (int column = 1; column < Width; column++)
                            g.DrawLine(p, column * pixSize - 1, 0, column * pixSize - 1, Height * pixSize);
                        //網格縱線
                        for (int row = 1 ; row < Height; row++)
                            g.DrawLine(p, 0, row * pixSize, Width * pixSize, row * pixSize);

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //檢查磚塊和遊戲狀態
        private void checkAndOverBrick()
        {
            //設定當前磚塊到畫面上
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                CoorArray[runBrick.X + runBrick.Points[i].X, runBrick.Y + runBrick.Points[i].Y] = runBrick.Color;
            }
            //檢查消除
            CheckAndDelFollRow();
            runBrick = nextBrick;
            runBrick.X = Width / 2;
            int y=0;
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (runBrick.Points[i].Y < y)
                    y = runBrick.Points[i].Y;
            }
            runBrick.Y = -y;
            for (int i = 0; i < runBrick.Points.Length; i++)
            {
                if (!CoorArray[runBrick.X + runBrick.Points[i].X, runBrick.Y + runBrick.Points[i].Y].IsEmpty)
                {
                    runBrick = null;
                    gameTimer.Stop();
                    for (int row = Height-1; row >=0; row--)
                    {

                        for (int column = 0; column < Width;column++ )
                        {
                            CoorArray[column, row] = backGroundColor;
                        }
                        PaintBricks(mainPalette);
                        System.Threading.Thread.Sleep(50);
                    }
                    endOfGame = true;
                    ready = false;
                    gameTimer.Start();
                    return;
                }
            }
            runBrick.Paint(mainPalette);

            nextBrick = brickFactory.CreateBrick();
            paintNext(nextPalette);

        }
        //檢查並消行磚塊
        private void CheckAndDelFollRow()
        {
            int fullRowCount = 0;
            int upRow = runBrick.Y - 2;
            int downRow = runBrick.Y + 2;
            if (upRow < 0) upRow = 0;
            if (downRow >= Height) downRow = Height - 1;
            for (int row = upRow; row <= downRow; row++)
            {
                bool isFull = true;
                for (int column = 0; column < Width; column++)
                {
                    if (CoorArray[column, row].IsEmpty)
                    {
                        isFull = false;
                        break;
                    }
                    
                }
                if (isFull)
                {
                    fullRowCount++;
                    gameTimer.Stop();
                    for (int n = 0; n < COLORS.Length; n++)
                    {
                        for (int column = 0; column < Width; column++)
                            CoorArray[column, row] = COLORS[n];
                    }
                    for (int rowIndex = row; rowIndex > 0; rowIndex--)
                    {
                        for (int column = 0; column < Width; column++)
                        {
                            CoorArray[column, rowIndex] = CoorArray[column, rowIndex - 1];
                        }
                    }
                    PaintBricks(mainPalette);
                    gameTimer.Start();
                }
            }
        }
        //結束遊戲的畫面
        private void paintGameOver()
        {
            StringFormat drawFormat = new StringFormat();
            Font font = new Font("Arial Black", 30f);
            drawFormat.Alignment = StringAlignment.Center;
            for (int j = 0; j < COLORS.Length; j++)
            {
                lock (mainPalette)
                {
                    try
                    {
                        mainPalette.DrawString("Game Over!",
                            font,
                            new SolidBrush(COLORS[j]),
                            new RectangleF(0, Height * pixSize / 2 - 100, Width * pixSize, 100),
                            drawFormat);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Thread.Sleep(100);
                }
            }
            
        }
    }
}
