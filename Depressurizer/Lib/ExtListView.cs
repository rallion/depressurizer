using System;
using System.Windows.Forms;

namespace Depressurizer.Lib
{
    internal class ExtListView : ListView
    {
        private bool isSelecting;

        public ExtListView()
        {
            SelectedIndexChanged += ExtListView_SelectedIndexChanged;
        }

        public event EventHandler SelectionChanged;

        private void ExtListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isSelecting)
            {
                isSelecting = true;
                Application.Idle += Application_Idle;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            isSelecting = false;
            Application.Idle -= Application_Idle;
            SelectionChanged(this, new EventArgs());
        }
    }
}