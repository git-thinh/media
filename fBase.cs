using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace media
{
    public class fBase : Form
    {
        const int _header_height = 19;
        Panel ui_header;
        IconButton ui_btn_exit;
        IconButton ui_btn_mini;

        public fBase()
        {
            this.Icon = Resources.play;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = app.app_color_background;

            ui_header = new Panel() {
                Dock = DockStyle.Top,
                Height = _header_height,
                BackColor = app.app_color_background_header, 
            };
            ui_header.MouseMove += f_form_event_MouseMove; 
            this.Controls.AddRange(new Control[] { ui_header });

            ui_btn_exit = new IconButton(19) {
                IconType = IconType.ios_close_empty,
                Dock = DockStyle.Right,
                InActiveColor = app.app_color_button,
            };
            ui_btn_mini = new IconButton(19) {
                IconType = IconType.ios_minus_empty,
                Dock = DockStyle.Right,
                InActiveColor = app.app_color_button,
            };
            ui_btn_exit.Click += (se, ev) => {
                this.Close();
            };
            ui_header.Controls.AddRange(new Control[] { ui_btn_mini, ui_btn_exit, });

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
