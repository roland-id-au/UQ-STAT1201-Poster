using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;

namespace ScatterPlotFilter
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			StringBuilder sb = new StringBuilder();

			Bitmap src = (Bitmap)Bitmap.FromFile(@"/Users/Blake/Uni/UQ-STAT1201-Poster/Face.bmp");
			var dst = new Bitmap(src.Width, src.Height);
			var canvas = Graphics.FromImage(dst);
			canvas.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, dst.Width, dst.Height));

			if(src.PixelFormat != PixelFormat.Format24bppRgb) {
				Console.WriteLine("Input must be 24bppRGB Bitmap");
				return;
			}

			int kernelSize = 15;
			int inverseThreshold = 750;

			for(int i = 0; i < src.Height; i += kernelSize) {
				for(int j = 0; j < src.Width; j+= kernelSize){

					int r = 0;
					int g = 0;
					int b = 0;

					int sampleCount = 0;

					for(int y = 0; y < kernelSize; y++) {
						for(int x = 0; x < kernelSize; x++) {
							var colour = src.GetPixel(x + j, y + i);

							if(colour.R + colour.G + colour.B > 750)
								continue;

							r += colour.R;
							g += colour.G;
							b += colour.B;

							sampleCount++;
						}
					}

					if(sampleCount == 0)
						continue;

					r /= sampleCount;
					g /= sampleCount;
					b /= sampleCount;

					if(r + g + b > 700)
						continue;

					Color c = Color.FromArgb(r, g, b);

					Random random = new Random();

					int r1 = random.Next(0, kernelSize);
					int r2 = random.Next(0, kernelSize);

					//canvas.DrawEllipse(new Pen(new SolidBrush(c)), j,i, 6, 6);
					canvas.DrawString("+", new Font(FontFamily.GenericSansSerif, 35), new SolidBrush(c), j + r1, i + r2);
					sb.Append(String.Format("(({0}, {1}), ({2}, {3}, {4})), ", j + r1, i + r2, r, g, b));
				}
			}

			src.Dispose();
			canvas.Save();

			dst.Save(@"/Users/Blake/Uni/UQ-STAT1201-Poster/Plot.bmp");
			dst.Dispose();

			File.WriteAllText(@"/Users/Blake/Uni/UQ-STAT1201-Poster/Plot.txt", sb.ToString());
		}
	}
}
