﻿/*
Copyright 2011, 2012, 2013 Steve Labbe.

This file is part of Depressurizer.

Depressurizer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Depressurizer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Depressurizer.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Depressurizer.Lib;
using Depressurizer.Properties;

namespace Depressurizer.DBEdit
{
    public partial class DBEditDlg : Form
    {
        private readonly MultiColumnListViewComparer listSorter = new MultiColumnListViewComparer();

        private readonly StringBuilder statusBuilder = new StringBuilder();
        private bool filterSuspend;

        public DBEditDlg()
        {
            InitializeComponent();
            ((ToolStripDropDownMenu) menu_File.DropDown).ShowImageMargin = false;
        }

        #region Actions

        private void SaveAs()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "gz",
                AddExtension = true,
                CheckFileExists = false,
                Filter = GlobalStrings.DBEditDlg_DialogFilter
            };
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
                AddStatusMsg(Save(dlg.FileName) ? GlobalStrings.DBEditDlg_FileSaved : GlobalStrings.DBEditDlg_SaveFailed);
        }

        private bool SaveDB()
        {
            if (Save("GameDB.xml.gz"))
            {
                AddStatusMsg(GlobalStrings.DBEditDlg_DatabaseSaved);
                UnsavedChanges = false;
                return true;
            }
            AddStatusMsg(GlobalStrings.DBEditDlg_DatabaseSaveFailed);
            return false;
        }

        private bool Save(string filename)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                Program.GameDB.Save(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(GlobalStrings.DBEditDlg_ErrorSavingFile, e.Message));
                Cursor = Cursors.Default;
                return false;
            }
            Cursor = Cursors.Default;
            return true;
        }

        private void LoadGames()
        {
            if (CheckForUnsaved())
            {
                var dlg = new OpenFileDialog
                {
                    DefaultExt = "gz",
                    AddExtension = true,
                    CheckFileExists = true,
                    Filter = GlobalStrings.DBEditDlg_DialogFilter
                };
                DialogResult res = dlg.ShowDialog();
                if (res == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    Program.GameDB.Load(dlg.FileName);
                    RefreshGameList();
                    AddStatusMsg(GlobalStrings.DBEditDlg_FileLoaded);
                    UnsavedChanges = true;
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ClearList()
        {
            if (MessageBox.Show(GlobalStrings.DBEditDlg_AreYouSureToClear, GlobalStrings.DBEditDlg_Confirm,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                == DialogResult.Yes)
            {
                if (Program.GameDB.Games.Count > 0)
                {
                    UnsavedChanges = true;
                    Program.GameDB.Games.Clear();
                    AddStatusMsg(GlobalStrings.DBEditDlg_ClearedAllData);
                }
                RefreshGameList();
            }
        }

        private void VisitStorePage(GameDBEntry game)
        {
            if (game != null)
            {
                Process.Start(string.Format(Resources.UrlSteamStore, game.Id));
            }
        }

        private void FetchList()
        {
            Cursor = Cursors.WaitCursor;

            var dlg = new FetchPrcDlg();
            DialogResult res = dlg.ShowDialog();

            if (dlg.Error != null)
            {
                MessageBox.Show(string.Format(GlobalStrings.DBEditDlg_ErrorWhileUpdatingGameList, dlg.Error.Message),
                    GlobalStrings.DBEditDlg_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                AddStatusMsg(GlobalStrings.DBEditDlg_ErrorUpdatingGameList);
            }
            else
            {
                if (res == DialogResult.Cancel || res == DialogResult.Abort)
                {
                    AddStatusMsg(GlobalStrings.DBEditDlg_CanceledListUpdate);
                }
                else
                {
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_UpdatedGameList, dlg.Added));
                    UnsavedChanges = true;
                }
            }

            RefreshGameList();
            UpdateForSelectChange();
            Cursor = Cursors.Default;
        }

        private void AddNewGame()
        {
            var dlg = new GameDBEntryDialog();
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Game != null)
            {
                if (Program.GameDB.Games.ContainsKey(dlg.Game.Id))
                {
                    MessageBox.Show(GlobalStrings.DBEditDlg_GameIdAlreadyExists, GlobalStrings.DBEditDlg_Warning,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_FailedToAddGame, dlg.Game.Id));
                }
                else
                {
                    Program.GameDB.Games.Add(dlg.Game.Id, dlg.Game);
                    AddGameToList(dlg.Game);
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_AddedGame, dlg.Game.Id));
                    UnsavedChanges = true;
                    UpdateForSelectChange();
                }
            }
        }

        private void EditSelectedGame()
        {
            if (lstGames.SelectedIndices.Count > 0)
            {
                var game = lstGames.SelectedItems[0].Tag as GameDBEntry;
                if (game != null)
                {
                    var dlg = new GameDBEntryDialog(game);
                    DialogResult res = dlg.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        UpdateGameAtIndex(lstGames.SelectedIndices[0]);
                        AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_EditedGame, game.Id));
                        UnsavedChanges = true;
                    }
                }
            }
        }

        private void DeleteSelectedGames()
        {
            if (lstGames.SelectedItems.Count > 0)
            {
                DialogResult res =
                    MessageBox.Show(
                        string.Format(GlobalStrings.DBEditDlg_AreYouSureDeleteGames, lstGames.SelectedItems.Count),
                        GlobalStrings.DBEditDlg_Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1);
                if (res == DialogResult.Yes)
                {
                    int deleted = 0;
                    foreach (ListViewItem item in lstGames.SelectedItems)
                    {
                        var game = item.Tag as GameDBEntry;
                        if (game != null)
                        {
                            Program.GameDB.Games.Remove(game.Id);
                            deleted++;
                        }
                    }
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_DeletedGames, deleted));
                    if (deleted > 0)
                    {
                        UnsavedChanges = true;
                        UpdateSelectedGames();
                    }
                    UpdateForSelectChange();
                }
            }
        }

        private void ScrapeGamesOfType(AppType type)
        {
            Cursor = Cursors.WaitCursor;

            var gamesToScrape = new Queue<int>();

            foreach (GameDBEntry g in Program.GameDB.Games.Values)
            {
                if (g.Type == type)
                {
                    gamesToScrape.Enqueue(g.Id);
                }
            }

            ScrapeGames(gamesToScrape);

            Cursor = Cursors.Default;
        }

        private void ScrapeSelected()
        {
            if (lstGames.SelectedItems.Count > 0)
            {
                Cursor = Cursors.WaitCursor;

                var gamesToScrape = new Queue<int>();

                foreach (int index in lstGames.SelectedIndices)
                {
                    var game = lstGames.Items[index].Tag as GameDBEntry;
                    if (game != null)
                    {
                        gamesToScrape.Enqueue(game.Id);
                    }
                }

                ScrapeGames(gamesToScrape);

                UpdateForSelectChange();

                Cursor = Cursors.Default;
            }
        }

        private void ScrapeGames(Queue<int> gamesToScrape)
        {
            if (gamesToScrape.Count > 0)
            {
                var dlg = new DbScrapeDlg(gamesToScrape);
                DialogResult res = dlg.ShowDialog();

                if (dlg.Error != null)
                {
                    AddStatusMsg(GlobalStrings.DBEditDlg_ErrorUpdatingGames);
                    MessageBox.Show(string.Format(GlobalStrings.DBEditDlg_ErrorWhileUpdatingGames, dlg.Error.Message),
                        GlobalStrings.DBEditDlg_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (res == DialogResult.Cancel)
                {
                    AddStatusMsg(GlobalStrings.DBEditDlg_UpdateCanceled);
                }
                else if (res == DialogResult.Abort)
                {
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_AbortedUpdate, dlg.JobsCompleted, dlg.JobsTotal));
                }
                else
                {
                    AddStatusMsg(string.Format(GlobalStrings.DBEditDlg_UpdatedEntries, dlg.JobsCompleted));
                }
                if (dlg.JobsCompleted > 0)
                {
                    UnsavedChanges = true;
                    RefreshGameList();
                }
            }
            else
            {
                AddStatusMsg(GlobalStrings.DBEditDlg_NoGamesToScrape);
            }
        }

        #endregion

        #region UI Updaters

        private void RefreshGameList()
        {
            lstGames.BeginUpdate();
            lstGames.ListViewItemSorter = null;
            lstGames.Items.Clear();

            foreach (GameDBEntry g in Program.GameDB.Games.Values)
            {
                if (ShouldDisplayGame(g))
                {
                    AddGameToList(g);
                }
            }
            lstGames.ListViewItemSorter = listSorter;
            lstGames.EndUpdate();
        }

        private void AddGameToList(GameDBEntry g)
        {
            var item = new ListViewItem(new[] {g.Name, g.Id.ToString(), g.Genre, g.Type.ToString()});
            item.Tag = g;
            lstGames.Items.Add(item);
        }

        private void UpdateSelectedGames()
        {
            int selIndex = 0;
            lstGames.BeginUpdate();
            lstGames.ListViewItemSorter = null;
            while (selIndex < lstGames.SelectedItems.Count)
            {
                if (UpdateGameAtIndex(lstGames.SelectedIndices[selIndex]))
                {
                    selIndex++;
                }
            }
            lstGames.ListViewItemSorter = listSorter;
            lstGames.EndUpdate();
        }

        private bool UpdateGameAtIndex(int index)
        {
            ListViewItem item = lstGames.Items[index];
            var game = item.Tag as GameDBEntry;
            if (game == null || !Program.GameDB.Games.ContainsKey(game.Id) || !ShouldDisplayGame(game))
            {
                lstGames.Items.RemoveAt(index);
                return false;
            }
            item.SubItems[0].Text = game.Name;
            item.SubItems[1].Text = game.Id.ToString();
            item.SubItems[2].Text = game.Genre;
            item.SubItems[3].Text = game.Type.ToString();
            return true;
        }

        private bool ShouldDisplayGame(GameDBEntry g)
        {
            return
                chkAll.Checked ||
                (g.Type == AppType.DLC && chkDLC.Checked) ||
                (g.Type == AppType.WebError && chkWebError.Checked) ||
                (g.Type == AppType.SiteError && chkSiteError.Checked) ||
                (g.Type == AppType.Game && chkGame.Checked) ||
                (g.Type == AppType.IdRedirect && chkRedirect.Checked) ||
                (g.Type == AppType.NonApp && chkNonApp.Checked) ||
                (g.Type == AppType.NotFound && chkNotFound.Checked) ||
                (g.Type == AppType.Unknown && chkUnknown.Checked) ||
                (g.Type == AppType.New && chkNew.Checked) ||
                (g.Type == AppType.AgeGated && chkAgeGate.Checked);
        }

        private void UpdateForSelectChange()
        {
            statSelected.Text = string.Format(GlobalStrings.DBEditDlg_SelectedDisplayedTotal,
                lstGames.SelectedItems.Count, lstGames.Items.Count, Program.GameDB.Games.Count);
            cmdDeleteGame.Enabled =
                cmdEditGame.Enabled = cmdStore.Enabled = cmdUpdateSelected.Enabled = (lstGames.SelectedItems.Count >= 1);
        }

        private void AddStatusMsg(string s)
        {
            statusBuilder.Append(s);
            statusBuilder.Append(' ');
        }

        private void ClearStatusMsg()
        {
            statusBuilder.Clear();
        }

        private void FlushStatusMsg()
        {
            statusMsg.Text = statusBuilder.ToString();
        }

        #endregion

        #region Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            listSorter.AddIntCol(1);
            lstGames.ListViewItemSorter = listSorter;
            lstGames.SetSortIcon(listSorter.GetSortCol(),
                (listSorter.GetSortDir() == 1) ? SortOrder.Ascending : SortOrder.Descending);
            RefreshGameList();
            UpdateForSelectChange();
        }

        private void menu_File_Load_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            LoadGames();
            UpdateForSelectChange();
            FlushStatusMsg();
        }

        private void menu_File_Save_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            SaveDB();
            FlushStatusMsg();
        }

        private void menu_File_SaveAs_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            SaveAs();
            FlushStatusMsg();
        }

        private void menu_File_Clear_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            ClearList();
            UpdateForSelectChange();
            FlushStatusMsg();
        }

        private void menu_File_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lstGames_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listSorter.SetSortCol(e.Column);
            lstGames.SetSortIcon(listSorter.GetSortCol(),
                (listSorter.GetSortDir() == 1) ? SortOrder.Ascending : SortOrder.Descending);
            lstGames.Sort();
        }

        private void lstGames_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        foreach (ListViewItem item in lstGames.Items)
                        {
                            item.Selected = true;
                        }
                    }
                    break;
                case Keys.Enter:
                    if (lstGames.SelectedItems.Count > 0)
                    {
                        ClearStatusMsg();
                        EditSelectedGame();
                        FlushStatusMsg();
                    }
                    break;
                case Keys.N:
                    if (e.Modifiers == Keys.Control)
                    {
                        ClearStatusMsg();
                        AddNewGame();
                        FlushStatusMsg();
                    }
                    break;
                case Keys.Delete:
                    if (lstGames.SelectedItems.Count > 0)
                    {
                        ClearStatusMsg();
                        DeleteSelectedGames();
                        FlushStatusMsg();
                    }
                    break;
            }
        }

        private void cmdFetch_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            FetchList();
            FlushStatusMsg();
        }

        private void cmdStore_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedItems.Count > 0)
            {
                VisitStorePage(lstGames.SelectedItems[0].Tag as GameDBEntry);
            }
        }

        private void cmdAddGame_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            AddNewGame();
            FlushStatusMsg();
        }

        private void cmdEditGame_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            EditSelectedGame();
            FlushStatusMsg();
        }

        private void cmdDeleteGame_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            DeleteSelectedGames();
            FlushStatusMsg();
        }

        private void cmdUpdateSelected_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            ScrapeSelected();
            FlushStatusMsg();
        }

        private void cmdUpdateUnchecked_Click(object sender, EventArgs e)
        {
            ClearStatusMsg();
            ScrapeGamesOfType(AppType.New);
            FlushStatusMsg();
            UpdateForSelectChange();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!filterSuspend)
            {
                filterSuspend = true;
                if (chkAll.Checked)
                {
                    chkDLC.Checked = chkSiteError.Checked = chkWebError.Checked = chkGame.Checked = chkNonApp.Checked
                        =
                        chkNotFound.Checked =
                            chkRedirect.Checked = chkNew.Checked = chkUnknown.Checked = chkAgeGate.Checked = false;
                }
                filterSuspend = false;
                RefreshGameList();
                UpdateForSelectChange();
            }
        }

        private void chkAny_CheckedChanged(object sender, EventArgs e)
        {
            if (!filterSuspend)
            {
                filterSuspend = true;
                if (((CheckBox) sender).Checked)
                {
                    chkAll.Checked = false;
                }
                filterSuspend = false;
                RefreshGameList();
                UpdateForSelectChange();
            }
        }

        private void lstGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForSelectChange();
        }

        private void DBEditDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckForUnsaved())
            {
                e.Cancel = true;
            }
        }

        #endregion

        private bool UnsavedChanges { get; set; }

        /// <summary>
        ///     If there are any unsaved changes, asks the user if they want to save. Also gives the user the option to cancel the
        ///     calling action.
        /// </summary>
        /// <returns>True if the action should proceed, false otherwise.</returns>
        private bool CheckForUnsaved()
        {
            if (!UnsavedChanges)
            {
                return true;
            }

            DialogResult res = MessageBox.Show(GlobalStrings.DBEditDlg_UnsavedChangesSave,
                GlobalStrings.DBEditDlg_UnsavedChanges, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (res == DialogResult.No)
            {
                // Don't save, just continue
                return true;
            }
            if (res == DialogResult.Cancel)
            {
                // Don't save, don't continue
                return false;
            }
            return SaveDB();
        }
    }
}