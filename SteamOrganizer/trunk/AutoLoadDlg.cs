using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Depressurizer {
    public partial class AutoLoadDlg : Form {
        const string PATH_HELP = "This is the path to your Steam installation.\nThe Program will try to set this automatically.\nIf it does not, type in the correct path, or click the Browse button.";
        const string ID_HELP = "This is your numeric Steam user ID.\nThe program will try to automatically fill the box with found options.\nIf you change the Steam path, click Refresh to reload the list.";
        const string PROF_HELP = "This is your Steam profile ID.\nIn order for the program to get your information,\nyou must be connected to the internet, and your profile must not be private.";
        public AutoLoadDlg() {
            InitializeComponent();
            toolTip.SetToolTip( lnkHelpPath, PATH_HELP );
            toolTip.SetToolTip( lnkHelpId, ID_HELP );
            toolTip.SetToolTip( lnkHelpProfile, PROF_HELP );
        }
    }
}
