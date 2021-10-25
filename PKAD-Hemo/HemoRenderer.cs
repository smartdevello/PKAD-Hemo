using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PKAD_Hemo
{
    public class HemoRenderer
    {
        private int width = 0, height = 0;
        private double totHeight = 1000;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private List<HemoData> data = null;
        private Dictionary<string, int> printers = null;
        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        Image redFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "red_finger.png"));
        Image yellowFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "yellow_finger.png"));
        public HemoRenderer(int width, int height)
        {
            this.width = width;
            this.height = height;

        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public Dictionary<string, int> getPrinters()
        {
            return this.printers;
        }
        public int getPrintersCount()
        {
            if (this.printers == null) return 0;
            return this.printers.Count();
        }
        public List<HemoData> getData()
        {
            return this.data;
        }

        public void setChatData(List<HemoData> data, Dictionary<string, int> pp)
        {
            this.data = data;
            this.printers = pp;
        }


        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)((a.X + 20) * px);
            res.Y = (int)((1000 - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)((p.X + 20) * px);
            res.Y = (int)((1000 - p.Y) * px);
            return res;
        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }
        
        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

                // Create a StringFormat object with the each line of text, and the block
                // of text centered on the page.
                double px = height / totHeight;
                rect.Location = convertCoord(rect.Location);
                rect.Width = (int)(px * rect.Width);
                rect.Height = (int)(px * rect.Height);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                // Draw the text and the surrounding rectangle.
                gfx.DrawString(content, font, brush, rect, stringFormat);
                //gfx.DrawRectangle(Pens.Black, rect);

        }
        private void fillPolygon(Brush brush, PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = convertCoord(points[i]);
            }
            gfx.FillPolygon(brush, points);
        }

        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawString(Point o, string content, int font = 15)
        {

            double px = height / totHeight;
            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            double px = height / totHeight;
            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);    

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void draw(int currentChartIndex)
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }
            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }

            drawImg(logoImg, new Point(20, 60), new Size(150, 50));

            if (data == null) return;


            Font numberFont = new Font("Arial", 30, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
            Font textFont = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);
            Font titleFont = new Font("Arial", 21, FontStyle.Bold, GraphicsUnit.Point);
            Font scaleFont = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);
            Font percentFont = new Font("Arial", 21, FontStyle.Bold, GraphicsUnit.Point);
            int pot_got = 0, pot = 0, mot = 0, got = 0, ied = 0;
            double gop = 0, gonz = 0;
            foreach (var item in data)
            {
                pot_got += item.pot - item.got;
                pot += item.pot;
                got += item.got;
                mot += item.mot;
                ied += item.ied;
                gop += item.gop;
                gonz += item.gonz;

            }
            var sorted_printers = printers.OrderByDescending(o => o.Value);
            if (currentChartIndex == 0)
            {


                drawCenteredString(pot_got.ToString(), new Rectangle(-20, 900, 200, 70), Brushes.Black, numberFont);
                drawCenteredString("Potential Ovals\nTo Have Issues", new Rectangle(-20, 830, 200, 70), Brushes.Black, textFont);

                drawCenteredString(ied.ToString(), new Rectangle(-20, 700, 200, 70), Brushes.Black, numberFont);
                drawCenteredString("OVER VOTE\nPOTENTIAL", new Rectangle(-20, 630, 200, 70), Brushes.Black, textFont);



                numberFont.Dispose();
                numberFont = new Font("Arial", 35, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);



                drawCenteredString(pot.ToString(), new Rectangle(800, 850, 150, 70), Brushes.Black, numberFont);
                drawCenteredString("Potential\nOval Totals", new Rectangle(800, 780, 150, 70), Brushes.Black, textFont);

                drawCenteredString(mot.ToString(), new Rectangle(950, 850, 150, 70), Brushes.Black, numberFont);
                drawCenteredString("Voted\nOval Totals", new Rectangle(950, 780, 150, 70), Brushes.Black, textFont);

                drawCenteredString(data.Count.ToString(), new Rectangle(1100, 850, 150, 70), Brushes.Black, numberFont);
                drawCenteredString("Files\nExamined", new Rectangle(1100, 780, 150, 70), Brushes.Black, textFont);



                textFont.Dispose();
                numberFont.Dispose();

                //Draw Big Chart
                textFont = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
                numberFont = new Font("Arial", 25, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);



                for (int x = 200; x <= 600; x += 80)
                {
                    drawLine(new Point(x, 900), new Point(x, 540), Color.Gray);
                }

                int mxEval = Math.Max(mot, got);
                mxEval = ((mxEval / 25) + 1) * 25;

                int step = mxEval / 5, xscale = 0;
                for (int x = 160; x <= 560; x += 80)
                {
                    drawCenteredString(xscale.ToString(), new Rectangle(x, 540, 80, 20), Brushes.Black, scaleFont);
                    xscale += step;
                }

                int rWidth = 0;
                rWidth = (int)(400 * mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(200, 900, rWidth, 80));

                rWidth = (int)(400 * got / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(200, 810, rWidth, 80));
                drawCenteredString(string.Format("{0}%", Math.Round(got * 100 / (double)mot, 2)), new Rectangle(550, 810, 150, 80), Brushes.BlueViolet, percentFont);
                if (got != 0)
                {
                    drawImg(yellowFingerImg, new Point(670, 850), new Size(80, 80));
                }

                drawString(new Point(170, 820), "G/M", 8);

                rWidth = (int)(400 * mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(200, 710, rWidth, 80));
                rWidth = (int)(400 * ied / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(200, 620, rWidth, 80));
                drawCenteredString(string.Format("{0}%", Math.Round(ied * 100 / (double)mot, 2)), new Rectangle(550, 620, 150, 80), Brushes.BlueViolet, percentFont);

                if (ied != 0)
                {
                    drawImg(redFingerImg, new Point(670, 670), new Size(80, 80));
                }

                drawString(new Point(170, 630), "IDE/M", 8);



                ////Worst Instance
                percentFont.Dispose();
                percentFont = new Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Point);

                drawCenteredString("Worst Instance", new Rectangle(-20, 500, 500, 100), Brushes.Black, titleFont);
                drawCenteredString(data[0].got.ToString(), new Rectangle(-20, 400, 100, 50), Brushes.Black, numberFont);
                drawCenteredString("Potential\nOvals To\nHave\nIssues", new Rectangle(-20, 360, 100, 100), Brushes.Black, textFont);
                drawCenteredString(data[0].ied.ToString(), new Rectangle(-20, 260, 100, 50), Brushes.Black, numberFont);
                drawCenteredString("OVER\nVOTE\nPOTENTIAL", new Rectangle(-20, 230, 100, 100), Brushes.Black, textFont);

                mxEval = Math.Max(data[0].got, data[0].mot);
                mxEval = ((mxEval / 25) + 1) * 25;
                for (int x = 100; x <= 350; x += 50)
                {
                    drawLine(new Point(x, 400), new Point(x, 150), Color.Gray);
                }

                xscale = 0;
                step = mxEval / 5;

                for (int x = 75; x <= 325; x += 50)
                {
                    drawCenteredString(xscale.ToString(), new Rectangle(x, 150, 50, 20), Brushes.Black, scaleFont);
                    xscale += step;
                }
                rWidth = (int)(250 * data[0].mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(100, 400, rWidth, 50));

                rWidth = (int)(250 * data[0].got / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(100, 340, rWidth, 50));
                drawCenteredString(string.Format("{0}%", Math.Round(data[0].got * 100 / (double)data[0].mot, 2)), new Rectangle(300, 340, 125, 50), Brushes.BlueViolet, percentFont);

                drawString(new Point(70, 350), "G/M", 8);

                rWidth = (int)(250 * data[0].mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(100, 280, rWidth, 50));

                rWidth = (int)(250 * data[0].ied / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(100, 220, rWidth, 50));
                drawCenteredString(string.Format("{0}%", Math.Round(data[0].ied * 100 / (double)data[0].mot, 2)), new Rectangle(300, 220, 125, 50), Brushes.BlueViolet, percentFont);
                drawString(new Point(70, 230), "IDE/M", 8);

                if (sorted_printers.Count() >=100 || data[0].ied !=0)
                {
                    drawImg(redFingerImg, new Point(350, 150), new Size(100, 100));
                }

                //Average Instance
                double avgot = got / (double)data.Count;
                double avmot = mot / (double)data.Count;
                double avied = ied / (double)data.Count;


                drawCenteredString("Average Instance", new Rectangle(430, 500, 500, 100), Brushes.Black, titleFont);
                drawCenteredString(string.Format("{0}", Math.Round(avgot, 2)), new Rectangle(430, 400, 150, 50), Brushes.Black, numberFont);
                drawCenteredString("Potential\nOvals To\nHave\nIssues", new Rectangle(430, 360, 150, 100), Brushes.Black, textFont);

                drawCenteredString(string.Format("{0}", Math.Round(avied, 1)), new Rectangle(430, 260, 150, 50), Brushes.Black, numberFont);
                drawCenteredString("OVER\nVOTE\nPOTENTIAL", new Rectangle(430, 230, 150, 100), Brushes.Black, textFont);

                mxEval = (int)Math.Max(avgot, avmot);
                mxEval = ((mxEval / 25) + 1) * 25;
                step = mxEval / 5;

                for (int x = 600; x <= 850; x += 50)
                {
                    drawLine(new Point(x, 400), new Point(x, 150), Color.Gray);
                }

                xscale = 0;
                for (int x = 575; x <= 825; x += 50)
                {
                    drawCenteredString(xscale.ToString(), new Rectangle(x, 150, 50, 20), Brushes.Black, scaleFont);
                    xscale += step;
                }

                rWidth = (int)(250 * avmot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(600, 400, rWidth, 50));
                rWidth = (int)(250 * avgot / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(600, 340, rWidth, 50));
                drawCenteredString(string.Format("{0}%", Math.Round(got * 100 / (double)mot, 2)), new Rectangle(800, 340, 125, 50), Brushes.BlueViolet, percentFont);

                drawString(new Point(570, 350), "G/M", 8);

                rWidth = (int)(250 * avmot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(600, 280, rWidth, 50));
                rWidth = (int)(250 * avied / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(600, 220, rWidth, 50));
                drawCenteredString(string.Format("{0}%", Math.Round(ied * 100 / (double)mot, 2)), new Rectangle(800, 220, 125, 50), Brushes.BlueViolet, percentFont);
                drawString(new Point(570, 230), "IDE/M", 8);

                if (sorted_printers.Count() >= 100|| ied != 0)
                {
                    drawImg(redFingerImg, new Point(800, 150), new Size(100, 100));
                }


                //Least Instance
                int leastInd = data.Count - 1;
                drawCenteredString("Least Instance", new Rectangle(900, 500, 500, 100), Brushes.Black, titleFont);
                drawCenteredString(data[leastInd].got.ToString(), new Rectangle(900, 400, 150, 50), Brushes.Black, numberFont);
                drawCenteredString("Potential\nOvals To\nHave\nIssues", new Rectangle(900, 360, 150, 100), Brushes.Black, textFont);

                drawCenteredString(data[leastInd].ied.ToString(), new Rectangle(900, 260, 150, 50), Brushes.Black, numberFont);
                drawCenteredString("OVER\nVOTE\nPOTENTIAL", new Rectangle(900, 230, 150, 100), Brushes.Black, textFont);

                mxEval = Math.Max(data[leastInd].got, data[leastInd].mot);
                mxEval = ((mxEval / 25) + 1) * 25;
                for (int x = 1050; x <= 1300; x += 50)
                {
                    drawLine(new Point(x, 400), new Point(x, 150), Color.Gray);
                }


                xscale = 0;
                step = mxEval / 5;
                for (int x = 1025; x <= 1275; x += 50)
                {
                    drawCenteredString(xscale.ToString(), new Rectangle(x, 150, 50, 20), Brushes.Black, scaleFont);
                    xscale += step;
                }

                rWidth = (int)(250 * data[leastInd].mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(1050, 400, rWidth, 50));

                rWidth = (int)(250 * data[leastInd].got / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(1050, 340, rWidth, 50));

                double percent = 0;
                if (data[leastInd].mot != 0) percent = data[leastInd].got * 100 / (double)data[leastInd].mot;
                else percent = 0;

                drawCenteredString(string.Format("{0}%", Math.Round(percent, 2)), new Rectangle(1230, 340, 125, 50), Brushes.BlueViolet, percentFont);
                drawString(new Point(1020, 350), "G/M", 8);

                rWidth = (int)(250 * data[leastInd].mot / (double)mxEval);
                fillRectangle(Color.Black, new Rectangle(1050, 280, rWidth, 50));

                rWidth = (int)(250 * data[leastInd].ied / (double)mxEval);
                fillRectangle(Color.Red, new Rectangle(1050, 220, rWidth, 50));

                if (data[leastInd].mot != 0) percent = data[leastInd].ied * 100 / (double)data[leastInd].mot;
                else percent = 0;

                drawCenteredString(string.Format("{0}%", Math.Round(percent, 2)), new Rectangle(1230, 220, 125, 50), Brushes.BlueViolet, percentFont);
                drawString(new Point(1020, 230), "IDE/M", 8);

                if (sorted_printers.Count() >= 100 || percent != 0)
                {
                    drawImg(redFingerImg, new Point(1200, 150), new Size(100, 100));
                }




                //Draw Printers

                int height_gap = 35;
                int width_gap = 270;
                int gapCount = printers.Count() / 20;
                if (printers.Count % 20 != 0) gapCount++;

                if (gapCount > 2) gapCount = 2;

                titleFont = new Font("Arial", 40, FontStyle.Bold, GraphicsUnit.Point);
                if (sorted_printers.Count() <= 40)
                {
                    drawCenteredString(sorted_printers.Count().ToString(), new Rectangle(1350, 850, 270 * gapCount, 100), Brushes.Red, titleFont);
                } else
                {
                    drawCenteredString(string.Format("40 of {0}", sorted_printers.Count()), new Rectangle(1350, 850, 270 * gapCount, 100), Brushes.Red, titleFont);
                }

                float percentage;
                for (int i = 0; i < Math.Min(40, sorted_printers.Count()); i++)
                {
                    drawString(new Point(1350 + width_gap * (i / 20), 750 - height_gap * (i % 20)), sorted_printers.ElementAt(i).Key, 8);
                    percentage = sorted_printers.ElementAt(i).Value * 100 / (float)data.Count;
                    drawPercentageLine(percentage, 1500 + width_gap * (i / 20), 750 - height_gap * (i % 20));
                }
            } else
            {
                int height_gap = 35;
                int width_gap = 270;
                int gapCount = 0;

                titleFont = new Font("Arial", 40, FontStyle.Bold, GraphicsUnit.Point);
                int st_tmpindex = 40 + 140 * (currentChartIndex - 1);
                if (sorted_printers.Count() - st_tmpindex >= 140)
                {
                    gapCount = 7;
                    drawCenteredString(string.Format("140 of {0}", sorted_printers.Count().ToString()), new Rectangle(0, 850, 270 * gapCount, 100), Brushes.Red, titleFont);
                }
                else
                {
                    int currentPrinters = sorted_printers.Count() - st_tmpindex;
                    gapCount = currentPrinters / 20;
                    if (currentPrinters % 20 != 0) gapCount++;
                    drawCenteredString(string.Format("{0} of {1}", currentPrinters, sorted_printers.Count().ToString()), new Rectangle(0, 850, 270 * gapCount, 100), Brushes.Red, titleFont);
                }

                titleFont = new Font("Arial", 40, FontStyle.Bold, GraphicsUnit.Point);

                float percentage;
                for (int i = 40 + 140 * (currentChartIndex - 1); i < Math.Min(sorted_printers.Count(), 40 + 140 * currentChartIndex); i++)
                {
                    int j = i - 40 - 140 * (currentChartIndex - 1);

                    drawString(new Point( width_gap * (j / 20), 750 - height_gap * (j % 20)), sorted_printers.ElementAt(i).Key, 8);
                    percentage = sorted_printers.ElementAt(i).Value * 100 / (float)data.Count;
                    drawPercentageLine(percentage, 150 + width_gap * (j / 20), 750 - height_gap * (j % 20));
                }
            }

            titleFont.Dispose();
            textFont.Dispose();
            numberFont.Dispose();
            scaleFont.Dispose();
            percentFont.Dispose();

        }
        private void drawPercentageLine(float percent, int X, int Y)
        {
            fillRectangle(Color.Black, new Rectangle(X, Y, 20, 20));
            fillRectangle(Color.Black, new Rectangle(X + 80, Y, 20, 20));
            drawLine(new Point(X, Y - 10), new Point(X + 80, Y - 10), Color.Black, 4);
            if (percent != 0.0F)
            {
                drawString(new Point(X + 30, Y + 10), string.Format("{0:F}%", percent), 8);
            }

        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }
        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }
        public void drawNumberwithUnderLine_Content(string number, string content, Color color, Point o, int fontSize, int marginX = 0, int marginY = 0, int contentWidth = 200, int contentHeight = 70, int contentFontsize = 13)
        {


            // Create font and brush.
            //Font drawFont = new Font("Arial", fontSize, FontStyle.Bold | FontStyle.Underline);
            //SolidBrush drawBrush = new SolidBrush(color);
            //drawCenteredString(content, new Rectangle(o.X - marginX, o.Y - marginY, contentWidth, contentHeight), drawBrush, contentFontsize);

            //double px = height / totHeight;
            //o = convertCoord(o);            
            //gfx.DrawString(number, drawFont, drawBrush, o.X, o.Y);            

            //drawFont.Dispose();
            //drawBrush.Dispose();
        }
    }
}
