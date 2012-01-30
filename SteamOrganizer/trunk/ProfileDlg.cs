using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Depressurizer {
    public partial class ProfileDlg : Form {
        public ProfileDlg() {
            InitializeComponent();
        }

        private void cmdBrowse_Click( object sender, EventArgs e ) {
            SaveFileDialog dlg = new SaveFileDialog();
            
            try {
                FileInfo f = new FileInfo( txtFilePath.Text );
                dlg.InitialDirectory = f.DirectoryName;
                dlg.FileName = f.Name;
            } catch( ArgumentException ) {
            }

            dlg.DefaultExt = "profile";
            dlg.AddExtension = true;
            dlg.Filter = "Profiles (*.profile)|*.profile";
            DialogResult res = dlg.ShowDialog();
            if( res == System.Windows.Forms.DialogResult.OK ) {
                txtFilePath.Text = dlg.FileName;
            }
        }

        private void cmdOk_Click( object sender, EventArgs e ) {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();

        }

        private void cmdCancel_Click( object sender, EventArgs e ) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ProfileDlg_Load( object sender, EventArgs e ) {
            txtFilePath.Text = System.Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) + @"\Depressurizer\Default.profile";
            RefreshIdList();
        }

        private void RefreshIdList() {
            cmbAccountID.BeginUpdate();
            cmbAccountID.Items.Clear();
            cmbAccountID.ResetText();
            cmbAccountID.Items.AddRange( GetSteamIds() );
            if( cmbAccountID.Items.Count > 0 ) {
                cmbAccountID.SelectedIndex = 0;
            }
            cmbAccountID.EndUpdate();
        }

        private string[] GetSteamIds() {
            try {
                DirectoryInfo dir = new DirectoryInfo( DepSettings.Instance().SteamPath + "\\userdata" );
                if( dir.Exists ) {
                    DirectoryInfo[] userDirs = dir.GetDirectories();
                    string[] result = new string[userDirs.Length];
                    for( int i = 0; i < userDirs.Length; i++ ) {
                        result[i] = userDirs[i].Name;
                    }
                    return result;
                }
                return new string[0];
            } catch {
                return new string[0];
            }
        }
    }
}
