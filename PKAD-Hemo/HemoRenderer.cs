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
        private Dictionary<string, int> left_info_dict = null;
        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        Image redFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "red_finger.png"));
        Image yellowFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "yellow_finger.png"));
        Image img14 = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "14.png"));
        Image img_percent_100 = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "percent-100.png"));
        Image img_percent_70 = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "percent-70.png"));
        Image img_percent_40 = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "percent-40.png"));

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

        public void setChatData(List<HemoData> data, Dictionary<string, int> pp, Dictionary<string, int> qq)
        {
            this.data = data;
            this.printers = pp;
            this.left_info_dict = qq;
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
        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle, string content = "")
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);

            o.X += size.Width / 2;
            o.Y -= size.Height / 3;
            float radius = size.Width * 0.3f;
            o.X += (int)(radius * Math.Cos(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            o.Y -= (int)(radius * Math.Sin(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            if (!string.IsNullOrEmpty(content))
            {
                int percent = (int)(sweepAngle * 100.0f / 360.0f);
                content = percent.ToString() + "%";
                Font numberFont = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Point);
                drawCenteredString(content, new Rectangle(o.X, o.Y, size.Width / 2 + 50, 50), Brushes.White, numberFont);
                numberFont.Dispose();
            }

        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
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

            Pen borderPen = new Pen(new SolidBrush(borderColor), 15);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
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
            int images_count = 0;
            
            foreach (var item in data)
            {
                pot_got += item.pot - item.got;
                pot += item.pot;
                got += item.got;
                mot += item.mot;
                ied += item.ied;
                gop += item.gop;
                gonz += item.gonz;

                if (item.got != 0 || item.ied != 0) images_count++;

            }

            var sorted_printers = printers.OrderByDescending(o => o.Value);            

            if (currentChartIndex == 0)
            {
                //drawCenteredString(pot_got.ToString(), new Rectangle(-20, 900, 200, 70), Brushes.Black, numberFont);
                //drawCenteredString("Potential Ovals\nTo Have Issues", new Rectangle(-20, 830, 200, 70), Brushes.Black, textFont);

                //drawCenteredString(ied.ToString(), new Rectangle(-20, 700, 200, 70), Brushes.Black, numberFont);
                //drawCenteredString("OVER VOTE\nPOTENTIAL", new Rectangle(-20, 630, 200, 70), Brushes.Black, textFont);



                numberFont.Dispose();
                numberFont = new Font("Arial", 30, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);



                drawCenteredString(pot.ToString(), new Rectangle(750, 900, 180, 70), Brushes.Black, numberFont);
                drawCenteredString("Potential\nOval Totals", new Rectangle(750, 830, 180, 70), Brushes.Black, textFont);

                drawCenteredString(mot.ToString(), new Rectangle(930, 900, 170, 70), Brushes.Black, numberFont);
                drawCenteredString("Voted\nOval Totals", new Rectangle(930, 830, 170, 70), Brushes.Black, textFont);

                drawCenteredString(data.Count.ToString(), new Rectangle(1100, 900, 150, 70), Brushes.Black, numberFont);
                drawCenteredString("Files\nExamined", new Rectangle(1100, 830, 150, 70), Brushes.Black, textFont);



                //////////////////Draw Pie/////////////////////////////////////
                ///

                int tot_left_info = 0;
                for (int i = 0; i< left_info_dict.Count(); i++)
                {
                    tot_left_info += left_info_dict.ElementAt(i).Value;
                }
                var sorted_left_info_dict = left_info_dict.OrderByDescending(o => o.Value);
                if (tot_left_info !=0)
                {
                    float startAngle = 270.0f, sweepAngle = 0;
                    Color[] colors = new Color[]
                    {
                        Color.BlueViolet,
                        Color.DarkSlateBlue,
                        Color.SlateBlue,
                        Color.MediumSlateBlue,
                        Color.MediumPurple
                    };
                    int colorexplainerY = 600, colorexplainerX = 1000;
                    for (int i = 0; i< Math.Min(sorted_left_info_dict.Count(), 5); i++)
                    {
                        sweepAngle = 360 * sorted_left_info_dict.ElementAt(i).Value / (float)tot_left_info;
                        if (i == 0)
                            drawPie(colors[i], new Point(780, 750), new Size(200, 200), startAngle, sweepAngle, "draw percent");
                        else drawPie(colors[i], new Point(780, 750), new Size(200, 200), startAngle, sweepAngle, "");
                        startAngle = startAngle + sweepAngle;

                        fillRectangle(colors[i], new Rectangle(colorexplainerX, colorexplainerY + i * 20, 15, 15));
                        int explainer_percent = (int)( sorted_left_info_dict.ElementAt(i).Value * 100 / tot_left_info);
                        string explainer_content = sorted_left_info_dict.ElementAt(i).Key + " " + explainer_percent.ToString() + "%";
                        drawString(new Point(colorexplainerX + 30, colorexplainerY + i * 20), explainer_content, 10);
                    }
                }

                /////////////////////////////Draw 14 Icon part////////////////////////////
                ///
                //new Rectangle(1100, 830, 150, 70)
                drawImg(img14, new Point(1120, 770), new Size(100, 100));
                Font newNumberfont  = new Font("Arial", 35, FontStyle.Bold, GraphicsUnit.Point);
                drawCenteredString_withBorder(images_count.ToString(), new Rectangle(1120, 660, 100, 100), Brushes.Black, newNumberfont, Color.Red);
                if (images_count != 0)
                    drawImg(redFingerImg, new Point(1220, 650), new Size(100, 100));
                newNumberfont.Dispose();

                /////////////////////////////////////////////////////////////

                textFont.Dispose();
                numberFont.Dispose();

                numberFont =  new Font("Arial", 35, FontStyle.Bold , GraphicsUnit.Point);
                textFont = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Point);
                drawCenteredString(mot.ToString(), new Rectangle(0, 900, 300, 100), Brushes.Black, numberFont);
                drawCenteredString("Total Number of\nHand Cast\nOvals", new Rectangle(0, 800, 300, 100), Brushes.Black, textFont);

                double percent = 0;
                percent = Math.Round(got * 100 / (double)mot, 3);
                
                drawCenteredString(percent.ToString() + "%", new Rectangle(300, 900, 300, 100), Brushes.Red, numberFont);
                if (percent != 0)
                    drawImg(yellowFingerImg, new Point(600, 900), new Size(100, 100));

                drawCenteredString("Of Total Votes\nCompromised", new Rectangle(300, 800, 300, 70), Brushes.Black, textFont);


                drawCenteredString(got.ToString(), new Rectangle(0, 700, 300, 100), Brushes.Red, numberFont);
                if (got != 0)
                    drawImg(yellowFingerImg, new Point(200, 750), new Size(100, 100));

                drawCenteredString("Number of\nBleed Through\nVotes", new Rectangle(0, 600, 300, 100), Brushes.Black, textFont);

                percent = Math.Round(ied * 100 / (double)data.Count, 2);
                drawCenteredString(percent.ToString() + "%", new Rectangle(300, 700, 300, 100), Brushes.Red, numberFont);
                drawCenteredString("Forced\nAdjudication\nRate", new Rectangle(300, 600, 300, 100), Brushes.Black, textFont);
                if (percent != 0)
                    drawImg(redFingerImg, new Point(580, 600), new Size(100, 100));

                drawCenteredString("Ballot Type Ratio", new Rectangle(750, 540, 300, 50), Brushes.Black, textFont);

                fillRectangle(Color.Red, new Rectangle(0, 480, 1300, 50));
                drawCenteredString("ANALYSIS OF BALLOTS DISPLAYING BLEED THROUGH VOTES", new Rectangle(0, 480, 1300, 50), Brushes.White, textFont);

                int max_got = 0, max_got_index = 0;
                for (int i = 0; i< data.Count; i++)
                {
                    if (data[i].got > max_got)
                    {
                        max_got = data[i].got;
                        max_got_index = i;

                    }
                }

                drawCenteredString("Ballot Most\nCompromised", new Rectangle(0, 400, 300, 80), Brushes.Black, textFont);
                drawCenteredString(data[max_got_index].gop.ToString() + "%", new Rectangle(0, 330, 300, 100), Brushes.Red, numberFont);
                percent = Math.Round(data[max_got_index].gop / 100, 2); 
                drawCenteredString(percent.ToString() + " Bleeds Per\nHand Cast Vote", new Rectangle(0, 230, 300, 80), Brushes.Black, textFont);
                drawCenteredString(data[max_got_index].mot.ToString() + "%", new Rectangle(0, 170, 300, 100), Brushes.Red, numberFont);
                drawCenteredString("Over Vote Conflict", new Rectangle(0, 110, 300, 80), Brushes.Black, textFont);


                drawCenteredString("Over Vote Ratio\nCreated By Bleed\nThrough Votets", new Rectangle(300, 400, 300, 100), Brushes.Black, textFont);
                percent = Math.Round(ied * 100 / (double)got, 2);
                drawCenteredString(percent.ToString() + "%", new Rectangle(300, 300, 300, 100), Brushes.Red, numberFont);

                if (percent !=0)
                {
                    drawImg(redFingerImg, new Point(400, 200), new Size(120, 120));
                }

                drawCenteredString("Worst\nOver Vote Ratio\nCreated By Bleed\nThrough Votes", new Rectangle(700, 400, 300, 130), Brushes.Black, textFont);
                drawCenteredString(data[max_got_index].mot.ToString() + "%", new Rectangle(700, 270, 300, 100), Brushes.Red, numberFont);

                if (data[max_got_index].mot != 0)
                {
                    drawImg(redFingerImg, new Point(800, 200), new Size(120, 120));
                }
                double max_gop = 0;
                foreach(var item in data)
                {
                    if (item.gop > max_gop) max_gop = item.gop;
                }
                drawCenteredString("Worst\nBleed Through\nRatio", new Rectangle(1000, 400, 300, 100), Brushes.Black, textFont);
                drawCenteredString(max_gop.ToString() + "%", new Rectangle(980, 300, 300, 80), Brushes.Red, numberFont);

                percent = max_gop / 100;
                
                if (max_gop % 100 == 0)
                    drawCenteredString(Math.Round(percent).ToString() + " Bleeds\nThrough Votes\nPer Hand Cast\nVote", new Rectangle(1000, 200, 300, 150), Brushes.Black, textFont);
                else 
                    drawCenteredString(Math.Round(percent, 2).ToString() + " Bleeds\nThrough Votes\nPer Hand Cast\nVote", new Rectangle(1000, 200, 300, 150), Brushes.Black, textFont);


                if (percent >= 0.9)
                {
                    drawImg(img_percent_100, new Point(1220, 250), new Size(100, 70));
                } else if (percent >= 0.5)
                {
                    drawImg(img_percent_70, new Point(1220, 250), new Size(100, 70));
                } else
                {
                    drawImg(img_percent_40, new Point(1220, 250), new Size(100, 70));
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
