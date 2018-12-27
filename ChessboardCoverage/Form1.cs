﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessboardCoverage
{
    public partial class Form1 : Form
    {
        private bool calcFlag = false;//棋盘颜色是否已经计算过了
        private int i = 1;//棋盘上色的时候用于计数
        private int size = 4;//棋盘的边长 默认是4
        private int ptx = 1;//特殊方块的x坐标
        private int pty = 1;//特殊方块的y坐标
        private float unit = 100;//每一个小格子的边长
        private int LCode = 0;
        private PointF[] Board;//从Board[1]开始 每三个一组 是同一个L ([1][2][3]存储着同一个L的坐标)
        private Graphics graphics;
        //Brushes笔刷，存放L型骨牌颜色
        private Brush[] brushes = { Brushes.Crimson, Brushes.ForestGreen, Brushes.Chartreuse, Brushes.IndianRed,
            Brushes.DeepSkyBlue, Brushes.Blue, Brushes.Green, Brushes.Purple, Brushes.BurlyWood, Brushes.Goldenrod,
            Brushes.DarkOrchid,Brushes.Brown,Brushes.Chartreuse,Brushes.Cyan,Brushes.Gainsboro,Brushes.Indigo,Brushes.Lavender,Brushes.MediumPurple,Brushes.OliveDrab,Brushes.PaleGoldenrod};
        private const float CHESSBOARD_SIZE = 512;

        public Form1()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();//图像类，用于绘图
            //textBoxSize.Tag = true;
            domainUpDown1.Tag = true;
            textBoxX.Tag = true;
            textBoxY.Tag = true;
            Board = new PointF[size * size];
            Board[0] = new PointF(1, 1);//先初始化第0个，后续从1开始计数，每三个数算一个L骨牌
            timer1.Enabled = false;
            this.buttonPrev.Enabled = false;
            this.buttonNext.Enabled = false;

        }

        private void Form1_Shown(object sender, EventArgs e)//初始化form视图
        {
            drawFrame();
        }

        private void textBox_TextChanged(object sender, EventArgs e)//判断x y 是否比 size大 
        {
            this.buttonPrev.Enabled = false;
            this.buttonNext.Enabled = false;
            calcFlag = false;
            TextBox tb = (TextBox)sender;
            if (tb.Text == "")
            {
                tb.Tag = false;
            }
            //else if (this.textBoxSize.Text != "error data" && Convert.ToInt32(tb.Text) > Convert.ToInt32(this.textBoxSize.Text))
            else if (Convert.ToInt32(tb.Text) > Convert.ToInt32(this.domainUpDown1.Text)||Convert.ToInt32(tb.Text)<=0)
            {
                tb.Tag = false;
                MessageBox.Show("初始块坐标不合法");
                this.buttonStart.Enabled = false;
            }
            else
            {
                if (tb == this.textBoxX)
                {
                    ptx = Convert.ToInt32(tb.Text);
                }
                if (tb == this.textBoxY)
                {
                    pty = Convert.ToInt32(tb.Text);
                }
                tb.Tag = true;
            }
            validataStart();
        }


        private void validataStart()
        {
            this.buttonEnd.Enabled = this.buttonResult.Enabled = this.buttonStart.Enabled
                = (bool)domainUpDown1.Tag && (bool)textBoxX.Tag && (bool)textBoxY.Tag;
            //= (bool)textBoxSize.Tag && (bool)textBoxX.Tag && (bool)textBoxY.Tag;
            if (this.buttonStart.Enabled == true)
                drawFrame();
        }




        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (this.buttonStart.Text == "开始")
            {
                drawFrame();
                i = 1;
                LCode = 0;
                this.buttonStart.Enabled = false;
                if (calcFlag == false)
                {
                    Board = new PointF[size * size];
                    fillChessboard(1, 1, ptx, pty, size);
                    calcFlag = true;
                }
                this.buttonStart.Text = "暂停";
                this.buttonStart.Enabled = true;
                timer1.Enabled = true;
                //this.textBoxSize.Enabled = false;
                this.domainUpDown1.Enabled = false;
                this.textBoxX.Enabled = false;
                this.textBoxY.Enabled = false;
                this.buttonResult.Enabled = true;
                this.buttonPrev.Enabled = true;
                this.buttonNext.Enabled = true;
            }
            else if (this.buttonStart.Text == "暂停")
            {
                this.buttonStart.Text = "继续";
                timer1.Enabled = false;

            }
            else if (this.buttonStart.Text == "继续")
            {
                this.buttonStart.Text = "暂停";
                timer1.Enabled = true;

            }


        }





        private void fillChessboard(int tx, int ty, int dx, int dy, int size)//得到Board
        {
            if (size == 1)
                return;
            ++LCode; 
            int code = LCode; // 骨牌号
            int i = 0;
            size = size / 2;
            //覆盖左上角子棋盘
            if (dx < tx + size && dy < ty + size)
            {//特殊方格在此棋盘中
                fillChessboard(tx, ty, dx, dy, size);
            }
            else
            {
                //右下角方块坐标放入棋盘表中，表中每三个方块绘制同一种颜色代表同一块骨牌
                Board[(code - 1) * 3 + (++i)] = new PointF(11 + (tx + size - 2) * unit, 11 + (ty + size - 2) * unit);
                //覆盖其余方格
                fillChessboard(tx, ty, tx + size - 1, ty + size - 1, size);
            }

            //覆盖右上角子棋盘
            if (dx >= tx + size && dy < ty + size)
            {
                //特殊方格在此棋盘中
                fillChessboard(tx + size, ty, dx, dy, size);
            }
            else
            {
                Board[(code - 1) * 3 + (++i)] = new PointF(11 + (tx + size - 1) * unit, 11 + (ty + size - 2) * unit);
                fillChessboard(tx + size, ty, tx + size, ty + size - 1, size);
            }

            //覆盖左下角子棋盘
            if (dx < tx + size && dy >= ty + size)
            {
                //特殊方格在此棋盘中
                fillChessboard(tx, ty + size, dx, dy, size);
            }
            else
            {
                Board[(code - 1) * 3 + (++i)] = new PointF(11 + (tx + size - 2) * unit, 11 + (ty + size - 1) * unit);
                fillChessboard(tx, ty + size, tx + size - 1, ty + size, size);
            }

            //覆盖右下角子棋盘
            if (dx >= tx + size && dy >= ty + size)
            {
                
                fillChessboard(tx + size, ty + size, dx, dy, size);
            }
            else
            {
                Board[(code - 1) * 3 + (++i)] = new PointF(11 + (tx + size - 1) * unit, 11 + (ty + size - 1) * unit);
                Console.WriteLine("i = " + i.ToString());
                fillChessboard(tx + size, ty + size, tx + size, ty + size, size);
            }

        }





        private void drawFrame()                
        {
            //size = Convert.ToInt32(this.textBoxSize.Text);
            size = Convert.ToInt32(this.domainUpDown1.Text);
            unit = CHESSBOARD_SIZE / size;
            graphics.FillRectangle(Brushes.LightGray, new RectangleF(10, 10, CHESSBOARD_SIZE, CHESSBOARD_SIZE));
            Pen pen = new Pen(Color.Black, 1);
            for (int i = 0; i <= size; ++i)
            {
                graphics.DrawLine(pen, 10 + i * unit, 10, 10 + i * unit, CHESSBOARD_SIZE + 10);
                graphics.DrawLine(pen, 10, 10 + i * unit, CHESSBOARD_SIZE + 10, 10 + i * unit);
            }
            graphics.FillRectangle(Brushes.LightSeaGreen, new RectangleF(11 + (ptx - 1) * unit, 11 + (pty - 1) * unit, unit - 1, unit - 1));


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i < size * size)
            {
                graphics.FillRectangle(brushes[i % 10], Board[i].X, Board[i].Y, unit - 1f, unit - 1f);
                graphics.FillRectangle(brushes[i % 10], Board[i + 1].X, Board[i + 1].Y, unit - 1f, unit - 1f);
                graphics.FillRectangle(brushes[i % 10], Board[i + 2].X, Board[i + 2].Y, unit - 1f, unit - 1f);
                i = i + 3;
            }   
            else
            {

                i = 1;
                this.buttonStart.Text = "开始";
                timer1.Enabled = false;
                //this.textBoxSize.Enabled = true;
                this.domainUpDown1.Enabled = true;
                this.textBoxX.Enabled = true;
                this.textBoxY.Enabled = true;
            }

        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (i >= 3)
            {
                i -= 3;
                graphics.FillRectangle(Brushes.LightGray, Board[i].X, Board[i].Y, unit - 1, unit - 1);
                graphics.FillRectangle(Brushes.LightGray, Board[i + 1].X, Board[i + 1].Y, unit - 1, unit - 1);
                graphics.FillRectangle(Brushes.LightGray, Board[i + 2].X, Board[i + 2].Y, unit - 1, unit - 1);
            }

        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (i <= size * size - 3)
            {
                graphics.FillRectangle(brushes[i % 20], Board[i].X, Board[i].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 1].X, Board[i + 1].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 2].X, Board[i + 2].Y, unit - 1, unit - 1);
                i += 3;
            }
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            drawFrame();
            this.buttonStart.Text = "开始";
            i = 1;
            LCode = 0;
            //this.textBoxSize.Enabled = true;
            this.domainUpDown1.Enabled = true;
            this.textBoxX.Enabled = true;
            this.textBoxY.Enabled = true;


        }


        private void buttonResult_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            i = 1;
            LCode = 0;
            if (calcFlag == false)
            {
                Board = new PointF[size * size];
                fillChessboard(1, 1, ptx, pty, size);
                calcFlag = true;
            }
            while (i <= size * size - 3)
            {
                graphics.FillRectangle(brushes[i % 20], Board[i].X, Board[i].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 1].X, Board[i + 1].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 2].X, Board[i + 2].Y, unit - 1, unit - 1);
                i += 3;
            }
            this.buttonPrev.Enabled = true;
            this.buttonNext.Enabled = true;
            //this.textBoxSize.Enabled = true;
            this.domainUpDown1.Enabled = true;
            this.textBoxX.Enabled = true;
            this.textBoxY.Enabled = true;
            this.buttonStart.Text = "开始";

        }
        private void resultEvent()    
        {
            timer1.Enabled = false;
            i = 1;
            LCode = 0;
            if (calcFlag == false)
            {
                Board = new PointF[size * size];
                fillChessboard(1, 1, ptx, pty, size);
                calcFlag = true;
            }
            while (i <= size * size - 3)
            {
                graphics.FillRectangle(brushes[i % 20], Board[i].X, Board[i].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 1].X, Board[i + 1].Y, unit - 1, unit - 1);
                graphics.FillRectangle(brushes[i % 20], Board[i + 2].X, Board[i + 2].Y, unit - 1, unit - 1);
                i += 3;
            }
            this.buttonPrev.Enabled = true;
            this.buttonNext.Enabled = true;
            //this.textBoxSize.Enabled = true;
            this.domainUpDown1.Enabled = true;
            this.textBoxX.Enabled = true;
            this.textBoxY.Enabled = true;
            this.buttonStart.Text = "开始";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.domainUpDown1.SelectedItem = this.domainUpDown1.Items[5];

            validataStart();
            this.domainUpDown1.SelectedItem = this.domainUpDown1.Items[5];
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            this.buttonPrev.Enabled = false;
            this.buttonNext.Enabled = false;
            calcFlag = false;
            DomainUpDown tb = (DomainUpDown)sender;
            if (tb.Text != "" )//如果不是空
            {
                int num = Convert.ToInt32(tb.Text);
                if ((num != 0 && (num & (num - 1)) == 0)
                    && num >= Convert.ToInt16(this.textBoxX.Text) && num >= Convert.ToInt16(this.textBoxY.Text))  //num必须是2的幂
                {
                    //如果size x y 都有效 tag 为 true 并绘图
                    tb.Tag = true;
                    this.textBoxX.Tag = true;
                    this.textBoxY.Tag = true;
                    tb.BackColor = System.Drawing.SystemColors.Window;
                    validataStart();
                    return;
                }
            }

            tb.Tag = false;
            validataStart();
        }

        private void domainUpDown1_TextChanged(object sender, EventArgs e)
        {
            DomainUpDown dd = (DomainUpDown)sender;
            String t = dd.Text;
            bool f = false;
            foreach(String item in dd.Items)
            {
                if (String.Equals(dd.Text, item))
                {
                    f = true;
                    dd.SelectedItem = item;
                }
            }
            if (!f)
            {
                
                MessageBox.Show("切片数应为大于0的2的幂次方");
                
            }
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            drawFrame();
            this.buttonStart.Text = "开始";
            i = 1;
            LCode = 0;
            //this.textBoxSize.Enabled = true;
            this.domainUpDown1.Enabled = true;
            this.textBoxX.Enabled = true;
            this.textBoxY.Enabled = true;
        }
    }
}
