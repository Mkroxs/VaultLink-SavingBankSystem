using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Helpers
{
    internal static class UiHelpers
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_COMPOSITED = 0x02000000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void PreloadPages(Panel container, params UserControl[] pages)
        {
            if (container == null) return;
            container.SuspendLayout();
            foreach (var p in pages)
            {
                if (p == null) continue;
                if (!container.Controls.Contains(p))
                {
                    p.Dock = DockStyle.Fill;
                    p.Visible = false;
                    container.Controls.Add(p);
                }
            }
            container.ResumeLayout(false);
        }

        public static UserControl ShowPage(Panel container, UserControl toShow, UserControl currentPage)
        {
            if (container == null || toShow == null) return currentPage;
            if (currentPage == toShow) return currentPage;

            toShow.Dock = DockStyle.Fill;

            if (currentPage == null)
            {
                if (!container.Controls.Contains(toShow))
                {
                    container.SuspendLayout();
                    container.Controls.Add(toShow);
                    container.ResumeLayout(false);
                }
                toShow.Visible = true;
                toShow.BringToFront();
                return toShow;
            }

            bool addedTemporarily = false;
            if (!container.Controls.Contains(toShow))
            {
                container.Controls.Add(toShow);
                addedTemporarily = true;
            }

            try
            {
                container.SuspendLayout();
                try
                {
                    if (container.Controls.Contains(currentPage))
                        currentPage.Visible = false;
                }
                catch { }

                if (!container.Controls.Contains(toShow))
                    container.Controls.Add(toShow);

                toShow.Visible = true;
                toShow.BringToFront();
                container.Refresh();
            }
            finally
            {
                try { container.ResumeLayout(false); } catch { }
            }

            if (container.Controls.Contains(currentPage) && currentPage != toShow)
            {
                try { container.Controls.Remove(currentPage); } catch { }
            }

            try
            {
                if (addedTemporarily && !container.Controls.Contains(toShow))
                    container.Controls.Add(toShow);
            }
            catch { }

            return toShow;
        }

        private static Bitmap CaptureControlBitmap(Control ctl, Size targetSize)
        {
            if (ctl == null)
                return new Bitmap(Math.Max(1, targetSize.Width), Math.Max(1, targetSize.Height));

            try
            {
                ctl.CreateControl();
                ctl.SuspendLayout();
                ctl.Size = targetSize;
                ctl.PerformLayout();
            }
            catch { }
            finally
            {
                try { ctl.ResumeLayout(false); } catch { }
            }

            Bitmap bmp = new Bitmap(Math.Max(1, targetSize.Width), Math.Max(1, targetSize.Height));
            try
            {
                ctl.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            catch
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    try { g.Clear(ctl.BackColor); } catch { g.Clear(SystemColors.Control); }
                }
            }
            return bmp;
        }

        private static Bitmap SetImageOpacity(Bitmap original, float opacity)
        {
            if (original == null) return null;
            opacity = Math.Max(0f, Math.Min(1f, opacity));
            Bitmap bmp = new Bitmap(original.Width, original.Height);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
                    System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
                    cm.Matrix33 = opacity;
                    ia.SetColorMatrix(cm, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    g.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, ia);
                }
            }
            catch
            {
                try { return (Bitmap)original.Clone(); } catch { return bmp; }
            }
            return bmp;
        }

        public static void EnableDoubleBufferingRecursive(Control c)
        {
            if (c == null) return;

            try
            {
                var prop = c.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                prop?.SetValue(c, true, null);
            }
            catch { }

            try
            {
                var controlType = c.GetType();
                var winFormsAssembly = typeof(Control).Assembly;
                bool isWinFormsControl = controlType.Assembly == winFormsAssembly;
                if (isWinFormsControl)
                {
                    var skip = new[]
                    {
                        "ComboBox","DateTimePicker","DataGridView","ListBox","CheckedListBox",
                        "RichTextBox","DomainUpDown","NumericUpDown","MonthCalendar",
                        "MenuStrip","StatusStrip","ToolStrip","TrackBar"
                    };

                    if (Array.IndexOf(skip, controlType.Name) < 0)
                    {
                        var setStyle = controlType.GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        setStyle?.Invoke(c, new object[]
                        {
                            ControlStyles.OptimizedDoubleBuffer |
                            ControlStyles.AllPaintingInWmPaint |
                            ControlStyles.UserPaint,
                            true
                        });

                        var update = controlType.GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        update?.Invoke(c, null);
                    }
                }
            }
            catch { }

            foreach (Control child in c.Controls)
                EnableDoubleBufferingRecursive(child);
        }

        public static void TryEnableComposited(Form f)
        {
            if (f == null) return;
            try
            {
                var handle = f.Handle;
                int ex = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, ex | WS_EX_COMPOSITED);
            }
            catch { }
        }

        public static void FixGuna2TextBoxVisibility(Control container)
        {
            if (container == null) return;
            foreach (Control c in GetAllControls(container))
            {
                if (c.GetType().Name == "Guna2TextBox")
                {
                    try
                    {
                        c.ForeColor = Color.Black;
                        var fill = c.GetType().GetProperty("FillColor", BindingFlags.Public | BindingFlags.Instance);
                        if (fill != null)
                        {
                            Color fc = (Color)fill.GetValue(c, null);
                            int lum = (int)(fc.R * 0.299 + fc.G * 0.587 + fc.B * 0.114);
                            c.ForeColor = lum > 128 ? Color.Black : Color.White;
                        }
                        c.Refresh();
                    }
                    catch { }
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control c in container.Controls)
            {
                yield return c;
                foreach (Control child in GetAllControls(c))
                    yield return child;
            }
        }
        public static void ForceRender(UserControl uc)
        {
            uc.Visible = true;
            uc.CreateControl();
            uc.PerformLayout();
            uc.Refresh();
            uc.Visible = false;
        }


        public static void EnableEnterKeyToClick(TextBox textBox, Button button)
        {
            textBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // Prevent the beep
                    button.PerformClick();     // Trigger the button click
                }
            };
        }
    }


}
