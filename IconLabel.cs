using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace media
{
    [Designer(typeof(ParentControlDesigner))]
    public class IconLabel : Label
    {
        private string IconChar { get; set; }
        private Font IconFont { get; set; }
        private Brush IconBrush = new SolidBrush(Color.Orange);

        static IconLabel()
        {
            InitialiseFont();
        }

        public IconLabel()
        {
            IconChar = char.ConvertFromUtf32((int)0xf0f3);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;

            // Set best quality
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // is the font ready to go?
            if (IconFont == null)
            {
                SetFontSize(graphics);
            }

            // Measure string so that we can center the icon.
            SizeF stringSize = graphics.MeasureString(IconChar, IconFont, Width);
            float w = stringSize.Width;
            float h = stringSize.Height;

            // center icon
            float left = (Width - w) / 2;
            float top = (Height - h) / 2;

            //graphics.Clear(Color.Transparent);
            //graphics.Flush();

            //////// Draw string to screen. 
            //////graphics.DrawString(IconChar, IconFont, IconBrush, new PointF(left, top));

            //////TextRenderer.DrawText(e.Graphics,
            //////              "231313131",
            //////              this.Font,
            //////              new Point(10, 10),
            //////              Color.Red);
            //////ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, (int)w, (int)h), Color.Transparent, Color.Transparent);            
            //////base.OnPaint(e);
            ////////graphics.Clear(Color.Transparent);
            ////////graphics.Flush();
            ////////if (Focused)
            ////////{
            ////////    var rc = this.ClientRectangle;
            ////////    rc.Inflate(-2, -2);
            ////////    ControlPaint.DrawFocusRectangle(e.Graphics, rc);
            ////////}

            Bitmap bit = TextToBitmap(IconChar, IconFont, Color.Orange);
            graphics.DrawImage(bit, 55, 55);

            //graphics.Dispose();

            base.OnPaint(e);
        }


        static Bitmap TextToBitmap(string text, Font font, Color foregroundColor)
        {
            SizeF textSize;

            using (var g = Graphics.FromHwndInternal(IntPtr.Zero))
                textSize = g.MeasureString(text, font);

            var image = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
            var brush = new SolidBrush(foregroundColor);

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Color.Magenta);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(text, font, brush, 0, 0);
                g.Flush();
            }

            image.MakeTransparent(Color.Magenta);

            // The image now has a transparent background, but around each letter are antialiasing artifacts still keyed to magenta.  We need to remove those.
            RemoveChroma(image, foregroundColor, Color.Magenta);
            return image;
        }

        static unsafe void RemoveChroma(Bitmap image, Color foregroundColor, Color chroma)
        {
            if (image == null) throw new ArgumentNullException("image");
            BitmapData data = null;

            try
            {
                data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                for (int y = data.Height - 1; y >= 0; --y)
                {
                    int* row = (int*)((int)data.Scan0 + (y * data.Stride));
                    for (int x = data.Width - 1; x >= 0; --x)
                    {
                        if (row[x] == 0) continue;
                        Color pixel = Color.FromArgb(row[x]);

                        if ((pixel != foregroundColor) &&
                             ((pixel.B >= foregroundColor.B) && (pixel.B <= chroma.B)) &&
                             ((pixel.G >= foregroundColor.G) && (pixel.G <= chroma.G)) &&
                             ((pixel.R >= foregroundColor.R) && (pixel.R <= chroma.R)))
                        {
                            row[x] = Color.FromArgb(
                              255 - ((int)
                                ((Math.Abs(pixel.B - foregroundColor.B) +
                                  Math.Abs(pixel.G - foregroundColor.G) +
                                  Math.Abs(pixel.R - foregroundColor.R)) / 3)),
                              foregroundColor).ToArgb();
                        }
                    }
                }
            }
            finally
            {
                if (data != null) image.UnlockBits(data);
            }
        }

        private void SetFontSize(Graphics g)
        {
            IconFont = GetAdjustedFont(g, IconChar, Width, Height, 4, true);
        }

        private Font GetIconFont(float size)
        {
            return new Font(Fonts.Families[0], size, GraphicsUnit.Point);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        /// <summary>
        /// Store the icon font in a static variable to reuse between icons
        /// </summary>
        private static readonly PrivateFontCollection Fonts = new PrivateFontCollection();
        private Font GetAdjustedFont(Graphics g, string graphicString, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            for (double adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize = adjustedSize - 0.5)
            {
                Font testFont = GetIconFont((float)adjustedSize);
                // Test the string with the new size
                SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont);
                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    // Fits! return it
                    return testFont;
                }
            }

            // Could not find a font size
            // return min or max or maxFontSize?
            return GetIconFont(smallestOnFail ? minFontSize : maxFontSize);
        }

        /// <summary>
        /// Loads the icon font from the resources.
        /// </summary>
        private static void InitialiseFont()
        {
            try
            {
                unsafe
                {
                    fixed (byte* pFontData = Resources.fontawesome_webfont)
                    {
                        uint dummy = 0;
                        Fonts.AddMemoryFont((IntPtr)pFontData, Resources.fontawesome_webfont.Length);
                        AddFontMemResourceEx((IntPtr)pFontData, (uint)Resources.fontawesome_webfont.Length, IntPtr.Zero, ref dummy);
                    }
                }
            }
            catch
            {
                // log?
            }
        }

    }
}
