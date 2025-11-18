using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Helpers
{
    internal static class UiHelpers
    {
        // Win32 constants
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_COMPOSITED = 0x02000000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Add pages into a panel once (Dock=Fill, Visible=false)
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

        // Show a preloaded page (hide previous) — minimal layout ops
        public static void ShowPage(Panel container, UserControl toShow, ref UserControl currentPage)
        {
            if (container == null || toShow == null) return;
            if (currentPage == toShow) return;

            container.SuspendLayout();

            if (currentPage != null)
            {
                currentPage.Visible = false;
            }

            // reveal new page and bring to front in the same layout batch
            toShow.Visible = true;
            toShow.BringToFront();

            container.ResumeLayout(false);

            currentPage = toShow;
        }

        // Try to set DoubleBuffered on the control and all children via reflection (best-effort)
        public static void EnableDoubleBufferingRecursive(Control c)
        {
            if (c == null) return;

            // Set protected DoubleBuffered property if available
            try
            {
                var prop = c.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                prop?.SetValue(c, true, null);
            }
            catch { /* best-effort */ }

            // Attempt to call protected SetStyle method to set optimized double buffer flags
            try
            {
                var setStyle = c.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                setStyle?.Invoke(c, new object[] {
                    (System.Windows.Forms.ControlStyles)(
                        System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer
                        | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint
                        | System.Windows.Forms.ControlStyles.UserPaint),
                    true
                });
                var updateStyles = c.GetType().GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                updateStyles?.Invoke(c, null);
            }
            catch { /* best-effort */ }

            // Recurse children
            foreach (Control child in c.Controls)
            {
                EnableDoubleBufferingRecursive(child);
            }
        }

        // Try to enable WS_EX_COMPOSITED (call AFTER handle created)
        public static void TryEnableComposited(Form f)
        {
            if (f == null) return;
            try
            {
                var handle = f.Handle; // ensure created
                int ex = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, ex | WS_EX_COMPOSITED);
            }
            catch
            {
                // Don't throw — best-effort only
            }
        }
    }
}
