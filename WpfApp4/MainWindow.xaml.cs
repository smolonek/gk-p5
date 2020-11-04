using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp4;
using Color = System.Drawing.Color;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            LoadImage();
        }

        public Bitmap bm { get; set; }

        private int[] red = null;
        private int[] green = null;
        private int[] blue = null;
        Histogram hist;
        List<KeyValuePair<int, int>> redList = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> greenList = new List<KeyValuePair<int, int>>();
        List<KeyValuePair<int, int>> blueList = new List<KeyValuePair<int, int>>();
        private void LoadImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif;*.tiff;*.jpeg|All files|*.*";
            if (op.ShowDialog() == true)
            {
                bm = new Bitmap(op.FileName);
                displayImage(bm);
                red = new int[256];
                green = new int[256];
                blue = new int[256];
                for (int x = 0; x < bm.Width; x++)
                {
                    for (int y = 0; y < bm.Height; y++)
                    {
                        Color pixel = bm.GetPixel(x, y);
                        red[pixel.R]++;
                        green[pixel.G]++;
                        blue[pixel.B]++;
                    }
                }
                setChartSeries();
            }
            else
            {
                return;
            }

        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        public void setChartSeries()
        {
            redList.Clear();
            greenList.Clear();
            blueList.Clear();

            for (int i = 0; i < 256; i++)
            {
                redList.Add(new KeyValuePair<int, int>(i, red[i]));
                greenList.Add(new KeyValuePair<int, int>(i, green[i]));
                blueList.Add(new KeyValuePair<int, int>(i, blue[i]));
            }
            
        }
        public void displayImage(Bitmap bmp)
        {
            Image.Source = BitmapToImageSource(bmp);
        }

        private void Load_button_Click(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }
        private void Histogram_button_Click(object sender, RoutedEventArgs e)
        {
            hist = new Histogram(redList, greenList, blueList);
            hist.Show();
        }

        
        private int[] calculateLUT(int[] values)
        {
            //poszukaj wartości minimalnej
            int minValue = 0;
            for (int i = 0; i < 256; i++)
            {
                if (values[i] != 0)
                {
                    minValue = i;
                    break;
                }
            }

            //poszukaj wartości maksymalnej
            int maxValue = 255;
            for (int i = 255; i >= 0; i--)
            {
                if (values[i] != 0)
                {
                    maxValue = i;
                    break;
                }
            }

            //przygotuj tablice zgodnie ze wzorem
            int[] result = new int[256];
            double a = 255.0 / (maxValue - minValue);
            for (int i = 0; i < 256; i++)
            {
                result[i] = (int)(a * (i - minValue));
            }

            return result;
        }
        private void Strech_button_Click(object sender, EventArgs e)
        {
            //Sprawdz czy wczytano obraz
            if (red == null)
            {
                return;
            }

            //Tablice LUT dla skladowych
            int[] LUTred = calculateLUT(red);
            int[] LUTgreen = calculateLUT(green);
            int[] LUTblue = calculateLUT(blue);

            //Przetworz obraz i oblicz nowy histogram
            red = new int[256];
            green = new int[256];
            blue = new int[256];
            Bitmap newBitmap = (Bitmap)bm.Clone();
            //Bitmap newBitmap = new Bitmap(oldBitmap.Width, oldBitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color pixel = bm.GetPixel(x, y);
                    Color newPixel = Color.FromArgb(LUTred[pixel.R], LUTgreen[pixel.G], LUTblue[pixel.B]);
                    newBitmap.SetPixel(x, y, newPixel);
                    red[newPixel.R]++;
                    green[newPixel.G]++;
                    blue[newPixel.B]++;
                }
            }
            Image.Source = BitmapToImageSource(newBitmap);
            setChartSeries();
        }

        private void Equalize_button_Click(object sender, RoutedEventArgs e)
        {
            var histR = new int[256];
            var histG = new int[256];
            var histB = new int[256];
            var sR = new double[256];
            var sG = new double[256];
            var sB = new double[256];
            double pR = 0;
            double pG = 0;
            double pB = 0;
            int posR = -1;
            int posG = -1;
            int posB = -1;
            int r;
            int g;
            int b;
            int n = bm.Height * bm.Width;
            var lutR = new int[256];
            var lutG = new int[256];
            var lutB = new int[256];
            var tmp = new System.Drawing.Color();
            for (int i = 0; i < 256; i++)
            {
                histB[i] = 0;
                histG[i] = 0;
                histR[i] = 0;
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    histR[tmp.R]++;
                    histG[tmp.G]++;
                    histB[tmp.B]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                sR[i] = 0;
                sG[i] = 0;
                sB[i] = 0;
                pR += (double)histR[i] / n;
                pG += (double)histG[i] / n;
                pB += (double)histB[i] / n;

                for (int j = 0; j < i; j++)
                {
                    sR[i] = pR;
                    sG[i] = pG;
                    sB[i] = pB;
                }
                if (sR[i] != 0 && posR == -1)
                    posR = i;
                if (sG[i] != 0 && posG == -1)
                    posG = i;
                if (sB[i] != 0 && posB == -1)
                    posB = i;
            }

            for (int i = 0; i < 256; i++)
            {
                lutR[i] = Math.Abs((int)(((sR[i] - sR[posR]) / (1 - sR[posR])) * 255));
                lutG[i] = Math.Abs((int)(((sG[i] - sG[posG]) / (1 - sG[posG])) * 255));
                lutB[i] = Math.Abs((int)(((sB[i] - sB[posB]) / (1 - sB[posB])) * 255));
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);

                    r = lutR[tmp.R];
                    g = lutG[tmp.G];
                    b = lutB[tmp.B];

                    bm.SetPixel(i, j, System.Drawing.Color.FromArgb(tmp.A, r, g, b));

                }
            }
            displayImage(bm);
            setChartSeries();
        }

        private void binarization(int value)
        {
            //var tmpbm = new Bitmap(bm);
            var color = new System.Drawing.Color();

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    if (color.G < value)
                    {
                        bm.SetPixel(i, j, System.Drawing.Color.Black);
                    }
                    else
                    {
                        bm.SetPixel(i, j, System.Drawing.Color.White);
                    }
                }
            }

            //bm = new Bitmap(tmpbm);
            displayImage(bm);
        }

        private void Manual_binarization_button_Click(object sender, RoutedEventArgs e)
        {
            
            binarization(int.Parse(threshold.Value.ToString()));
        }

        private void Percent_binarization_button_Click(object sender, RoutedEventArgs e)
        {
            var percentage = double.Parse(threshold2.Value.ToString());
            if (percentage > 100 || percentage < 0)
                return;

            int nextSum = 0;
            var allPixels = bm.Width * bm.Height;
            var limes = (percentage / 100) * allPixels;
            var hist = new int[256];
            var tmp = new System.Drawing.Color();
            int result = 0;
            for (int i = 0; i < 256; i++)
            {
                hist[i] = 0;

            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    hist[tmp.G]++;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                nextSum += hist[i];
                if (nextSum > limes)
                {
                    result = i;
                    break;
                }
            }
            binarization(result);
        }

        private void Otsu_binarization_button_Click(object sender, RoutedEventArgs e)
        {
            var hist = new int[256];
            var tmp = new System.Drawing.Color();
            for (int i = 0; i < 256; i++)
            {
                hist[i] = 0;

            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    hist[tmp.G]++;
                }
            }

            int total = bm.Width * bm.Height;
            double wB = 0;
            double wF = 0;
            int sum = 0;
            double sumB = 0;
            double varMax = 0;
            int t = 0;
            double uB;
            double uF;

            for (int i = 0; i < 256; i++)
            {
                sum += i * hist[i];
            }
            for (int i = 0; i < 256; i++)
            {
                wB += hist[i];
                if (wB == 0) continue;

                wF = total - wB;
                if (wF == 0) break;

                sumB += (i * hist[i]);

                uB = sumB / wB;
                uF = (sum - sumB) / wF;

                double varBetween = wB * wF * Math.Pow((uF - uB), 2);

                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    t = i;
                }
            }
            binarization(t);

        }
    }
}
