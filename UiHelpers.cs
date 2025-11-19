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

        public static void ShowPage(Panel container, UserControl toShow, ref UserControl currentPage)
        {
            if (container == null || toShow == null) return;
            if (currentPage == toShow) return;

            container.SuspendLayout();

            if (currentPage != null)
            {
                currentPage.Visible = false;
            }

            toShow.Visible = true;
            toShow.BringToFront();

            container.ResumeLayout(false);

            currentPage = toShow;
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
                var winFormsAssembly = typeof(System.Windows.Forms.Control).Assembly;

                bool isWinFormsControl = controlType.Assembly == winFormsAssembly;

                if (isWinFormsControl)
                {
                    
                    var skipTypes = new[]
                    {
                "ComboBox",
                "DateTimePicker",
                "DataGridView",
                "ListBox",
                "CheckedListBox",
                "RichTextBox",
                "DomainUpDown",
                "NumericUpDown",
                "MonthCalendar",
                "MenuStrip",
                "StatusStrip",
                "ToolStrip",
                "TrackBar"
            };

                    if (Array.IndexOf(skipTypes, controlType.Name) < 0)
                    {
                        var setStyle = controlType.GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        setStyle?.Invoke(c, new object[] {
                    (System.Windows.Forms.ControlStyles)(
                        System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer
                        | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint
                        | System.Windows.Forms.ControlStyles.UserPaint),
                    true
                });

                        var updateStyles = controlType.GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        updateStyles?.Invoke(c, null);
                    }
                }
            }
            catch
            {
                
            }

          
            foreach (Control child in c.Controls)
            {
                EnableDoubleBufferingRecursive(child);
            }
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
            catch
            {
                
            }
        }

        
        public static void FixGuna2TextBoxVisibility(Control container)
        {
            if (container == null) return;

            foreach (Control control in GetAllControls(container))
            {
               
                if (control.GetType().Name == "Guna2TextBox")
                {
                    try
                    {
                        
                        control.ForeColor = Color.Black;

                       
                        var fillColorProp = control.GetType().GetProperty("FillColor", BindingFlags.Public | BindingFlags.Instance);
                        if (fillColorProp != null)
                        {
                            Color fillColor = (Color)fillColorProp.GetValue(control, null);
                            
                            int luminance = (int)(fillColor.R * 0.299 + fillColor.G * 0.587 + fillColor.B * 0.114);
                            control.ForeColor = luminance > 128 ? Color.Black : Color.White;
                        }

                        control.Refresh();
                    }
                    catch {  }
                }
            }
        }

      
        private static System.Collections.Generic.IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;
                foreach (Control child in GetAllControls(control))
                {
                    yield return child;
                }
            }
        }
    }
}