using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace media
{
    public class fMedia : Form
    {
        const int _header_height = 29;
        const int _img_height = 250;
        TranspCtrl ui_header; 
        PictureBox ui_img_video;

        static fMedia()
        { 
        }

        public fMedia()
        {
            this.Icon = Resources.play;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = app.app_color_background;

            ui_img_video = new PictureBox()
            {
                Location = new Point(0, 0),
                Width = app.app_width,
                Height = _img_height,
                Image = Resources.bg
            };

            ui_header = new TranspCtrl()
            {
                Location = new Point(0, 0),
                Width = app.app_width + 3,
                Height = _header_height, 
                Opacity = 70,
                BackColor = Color.Black,
            };  
            ui_header.Controls.AddRange(new Control[] {
            });
            ui_header.MouseMove += f_form_event_MouseMove;
             
            this.Controls.AddRange(new Control[] {
                ui_header,
                ui_img_video,
            });


            this.Shown += (se, ev) =>
            {
                this.Top = (Screen.PrimaryScreen.WorkingArea.Height - app.app_height) / 2;
                this.Left = (Screen.PrimaryScreen.WorkingArea.Width - app.app_width) / 2;
                this.Width = app.app_width;
                this.Height = app.app_height;
                 
            };
        }



        #region [ FORM MOVE ]

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void f_form_event_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion
    }
}
