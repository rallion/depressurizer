using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Depressurizer {
    public partial class OptionsDlg : Form {
        public OptionsDlg() {
            InitializeComponent();
        }

        private void OptionsForm_Load( object sender, EventArgs e ) {
            FillFieldsFromSettings();
        }

        private void FillFieldsFromSettings() {
            DepSettings settings = DepSettings.Instance();
            txtSteamPath.Text = settings.SteamPath;
            txtDefaultProfile.Text = settings.ProfileToLoad;
            if( settings.LoadProfileOnStartup ) {
                radStartupLoad.Checked = true;
            } else {
                radStartupNoLoad.Checked = true;
            }
            chkAutoSaveToSteam.Checked = settings.AutoSaveToSteam;
            chkPLoadSteamCats.Checked = settings.LoadSteamCatsOnProfileLoad;
            chkPLoadUpdateGameList.Checked = settings.DownloadGamelistOnProfileLoad;
            chkRemoveUnwantedEntries.Checked = settings.RemoveExtraEntries;
        }

        private void SaveFieldsToSettings() {
            DepSettings settings = DepSettings.Instance();

            settings.SteamPath = txtSteamPath.Text;
            settings.LoadProfileOnStartup = radStartupLoad.Checked;
            settings.ProfileToLoad = txtDefaultProfile.Text;
            settings.DownloadGamelistOnProfileLoad = chkPLoadUpdateGameList.Checked;
            settings.LoadSteamCatsOnProfileLoad = chkPLoadSteamCats.Checked;
            settings.AutoSaveToSteam = chkAutoSaveToSteam.Checked;
            settings.RemoveExtraEntries = chkRemoveUnwantedEntries.Checked;
            //TODO: Add exception handling here
            settings.Save();
        }

        private void cmdCancel_Click( object sender, EventArgs e ) {
            this.Close();
        }

        private void cmdAccept_Click( object sender, EventArgs e ) {
            SaveFieldsToSettings();
            this.Close();
        }
    }
}
