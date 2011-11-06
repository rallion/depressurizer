using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace Depressurizer {
    public partial class FormMain : Form {

        const string CAT_ALL_NAME = "<All>";
        const string CAT_FAV_NAME = "<Favorite>";
        const string CAT_UNC_NAME = "<Uncategorized>";

        const string COMBO_NEWCAT = "Add new...";
        const string COMBO_NONE = "Remove category";

        GameData gameData;
        int sortColumn = 1;
        int sortDirection = 1;

        object lastSelectedCat = null;

        public FormMain() {
            gameData = new GameData();
            InitializeComponent();
            combFavorite.SelectedIndex = 0;
            UpdateGameSorter();
            FillCategoryList();
        }

        public void ManualLoad() {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult res = dlg.ShowDialog();
            if( res == DialogResult.OK ) {
                Cursor = Cursors.WaitCursor;
                try {
                    int loadedGames = gameData.LoadSteamFile( dlg.FileName );
                    if( loadedGames == 0 ) {
                        MessageBox.Show( "Warning: No game info found in the specified file." );
                    } else {
                        //TODO: Add number of loaded games to status bar.
                        FillCategoryList();
                    }
                } catch( ParseException e ) {
                    MessageBox.Show( e.Message, "File parsing error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                } catch( IOException e ) {
                    MessageBox.Show( e.Message, "Error reading file", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                Cursor = Cursors.Default;
            }
        }

        public void ManualSave() {
            SaveFileDialog dlg = new SaveFileDialog();
            DialogResult res = dlg.ShowDialog();
            if( res == DialogResult.OK ) {
                Cursor = Cursors.WaitCursor;
                try {
                    gameData.SaveSteamFile( dlg.FileName );
                    //TODO: display confirmation in status bar
                } catch( IOException e ) {
                    MessageBox.Show( e.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                Cursor = Cursors.Default;
            }
        }

        public void ManualProfileLoad() {
            GetStringDlg dlg = new GetStringDlg( "", "Load profile", "Enter profile name:", "Load Profile" );
            if( dlg.ShowDialog() == DialogResult.OK ) {
                Cursor = Cursors.WaitCursor;
                try {
                    int loadedGames = gameData.LoadProfile( dlg.Value );
                    if( loadedGames == 0 ) {
                        MessageBox.Show( "No game data found. Please make sure the profile is spelled correctly, and that the profile is public.", "No data found", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                    } else {
                        // TODO: Display status update in status bar
                        FillGameList();
                    }
                } catch( System.Net.WebException e ) {
                    MessageBox.Show( e.Message, "Error loading profile data", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                Cursor = Cursors.Default;
            }
        }

        #region UI Event Handlers
        #region Drag and drop

        private void lstCategories_DragEnter( object sender, DragEventArgs e ) {
            e.Effect = DragDropEffects.Move;
        }

        private void lstCategories_DragDrop( object sender, DragEventArgs e ) {
            if( e.Data.GetDataPresent( typeof( int[] ) ) ) {
                Point clientPoint = lstCategories.PointToClient( new Point( e.X, e.Y ) );
                object dropItem = lstCategories.Items[lstCategories.IndexFromPoint( clientPoint )];
                if( dropItem is Category ) {
                    gameData.SetGameCategories( (int[])e.Data.GetData( typeof( int[] ) ), (Category)dropItem);
                    FillGameList();
                } else if( dropItem is string ) {
                    if( (string)dropItem == CAT_FAV_NAME ) {
                        gameData.SetGameFavorites( (int[])e.Data.GetData( typeof( int[] ) ), true );
                        FillGameList();
                    } else if( (string)dropItem == CAT_UNC_NAME ) {
                        gameData.SetGameCategories( (int[])e.Data.GetData( typeof( int[] ) ), null );
                        FillGameList();
                    }
                }
            }
        }

        private void lstGames_ItemDrag( object sender, ItemDragEventArgs e ) {
            int[] selectedGames = new int[lstGames.SelectedItems.Count];
            for( int i = 0; i < lstGames.SelectedItems.Count; i++ ) {
                selectedGames[i] = ( (Game)lstGames.SelectedItems[i].Tag ).Id;
            }
            lstGames.DoDragDrop( selectedGames, DragDropEffects.Move );
        }
        #endregion
        #region Main menu
        private void menu_File_Load_Click( object sender, EventArgs e ) {
            ManualLoad();
        }

        private void menu_File_ProfileLoad_Click( object sender, EventArgs e ) {
            ManualProfileLoad();
        }

        private void menu_File_Save_Click( object sender, EventArgs e ) {
            ManualSave();
        }

        private void menu_File_Exit_Click( object sender, EventArgs e ) {
            this.Close();
        }
        #endregion
        #region Buttons
        private void cmdCatAdd_Click( object sender, EventArgs e ) {
            CreateCategory();
        }

        private void cmdCatRename_Click( object sender, EventArgs e ) {
            if( lstCategories.SelectedItems.Count > 0 ) {
                RenameCategory( lstCategories.SelectedItem as Category );
            }
        }

        private void cmdCatDelete_Click( object sender, EventArgs e ) {
            if( lstCategories.SelectedItems.Count > 0 ) {
                DeleteCategory( lstCategories.SelectedItem as Category );
            }
        }

        private void cmdGameSetCategory_Click( object sender, EventArgs e ) {
            Category c;
            if( GetSelectedCategoryFromCombo( out c ) ) {
                AssignCategoryToSelectedGames( c );
                FillGameList();
            }
        }

        private void cmdGameSetFavorite_Click( object sender, EventArgs e ) {
            AssignFavoriteToSelectedGames( GetSelectedFavorite() );
            FillGameList();
        }
        #endregion

        private void lstCategories_SelectedIndexChanged( object sender, EventArgs e ) {
            if( lstCategories.SelectedItem != lastSelectedCat ) {
                FillGameList();
                lastSelectedCat = lstCategories.SelectedItem;
            }
        }

        private void lstGames_ColumnClick( object sender, ColumnClickEventArgs e ) {
            if( e.Column == this.sortColumn ) {
                this.sortDirection *= -1;
            } else {
                this.sortDirection = 1;
                this.sortColumn = e.Column;
            }
            UpdateGameSorter();
        }

        #endregion

        private bool GetSelectedFavorite() {
            return combFavorite.SelectedItem as string == "Yes";
        }

        public bool GetSelectedCategoryFromCombo( out Category result ) {
            result = null;
            if( combCategory.SelectedItem is Category ) {
                result = combCategory.SelectedItem as Category;
                return true;
            } else if( combCategory.SelectedItem is string ) {
                if( (string)combCategory.SelectedItem == COMBO_NEWCAT ) {
                    result = CreateCategory();
                    return result != null;
                } else if( (string)combCategory.SelectedItem == COMBO_NONE ) {
                    return true;
                }
            }
            return false;

        }

        public bool DeleteCategory( Category c ) {
            if( c != null ) {
                DialogResult res = MessageBox.Show( string.Format( "Delete category '{0}'?", c.Name ), "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Warning );
                if( res == System.Windows.Forms.DialogResult.Yes ) {
                    if( gameData.RemoveCategory( c ) ) {
                        FillCategoryList();
                        FillGameList();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RenameCategory( Category c ) {
            if( c != null ) {
                GetStringDlg dlg = new GetStringDlg( c.Name, string.Format( "Rename category: {0}", c.Name ), "Enter new name:", "Rename" );
                if( dlg.ShowDialog() == DialogResult.OK ) {
                    if( gameData.RenameCategory( c, dlg.Value ) ) {
                        FillCategoryList();
                        //OPTIMIZE: This game list refresh could be made more efficient
                        FillGameList();
                        return true;
                    }
                }
            }
            return false;
        }

        /*
        private bool ShouldDisplayGame( Game g ) {
            if( lstCategories.SelectedItem == null ) {
                return false;
            }
            if( lstCategories.SelectedItem is string ) {
                if( (string)lstCategories.SelectedItem == CAT_ALL_NAME ) {
                    return true;
                }
                if( (string)lstCategories.SelectedItem == CAT_FAV_NAME ) {
                    return g.Favorite;
                }
                if( (string)lstCategories.SelectedItem == CAT_UNC_NAME ) {
                    return g.Category == null;
                }
            } else if( lstCategories.SelectedItem is Category ) {
                return g.Category == (Category)lstCategories.SelectedItem;
            }
            return false;
        }
        */
        #region Utility
        private void FillGameList() {
            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            if( lstCategories.SelectedItems.Count > 0 ) {
                object catObj = lstCategories.SelectedItem;
                bool showAll = false;
                bool showFav = false;
                if( catObj is string ) {
                    if( (string)catObj == CAT_ALL_NAME ) {
                        showAll = true;
                    } else if( (string)catObj == CAT_FAV_NAME ) {
                        showFav = true;
                    }
                }
                Category cat = lstCategories.SelectedItem as Category;

                foreach( Game g in gameData.Games.Values ) {
                    if( showAll || ( showFav && g.Favorite ) || ( !showFav && g.Category == cat ) ) {
                        AddGameToList( g );
                    }
                }
                lstGames.Sort();
            }
            lstGames.EndUpdate();
        }

        private void AddGameToList( Game g ) {
            string catName = ( g.Category == null ) ? CAT_UNC_NAME : g.Category.Name;
            ListViewItem item = new ListViewItem( new string[] { g.Id.ToString(), g.Name, catName, g.Favorite ? "Y" : "N" } );
            item.Tag = g;
            lstGames.Items.Add( item );
        }

        private void FillCategoryList() {
            gameData.Categories.Sort();
            object[] catList = gameData.Categories.ToArray();

            lstCategories.BeginUpdate();
            object selected = lstCategories.SelectedItem;
            lstCategories.Items.Clear();
            lstCategories.Items.Add( CAT_ALL_NAME );
            lstCategories.Items.Add( CAT_FAV_NAME );
            lstCategories.Items.Add( CAT_UNC_NAME );
            lstCategories.Items.AddRange( catList );
            if( selected == null || !lstCategories.Items.Contains( selected ) ) {
                lstCategories.SelectedIndex = 0;
            } else {
                lstCategories.SelectedItem = selected;
            }
            lstCategories.EndUpdate();

            combCategory.BeginUpdate();
            selected = combCategory.SelectedItem;
            combCategory.Items.Clear();
            combCategory.Items.Add( COMBO_NEWCAT );
            combCategory.Items.Add( COMBO_NONE );
            combCategory.Items.Add( "" );
            combCategory.Items.AddRange( catList );
            combCategory.SelectedItem = selected;
            combCategory.EndUpdate();
        }

        private void UpdateGameSorter() {
            lstGames.ListViewItemSorter = new GameListViewItemComparer( sortColumn, sortDirection, sortColumn == 0 );
        }

        private void AssignCategoryToSelectedGames( Category cat ) {
            foreach( ListViewItem item in lstGames.SelectedItems ) {
                ( item.Tag as Game ).Category = cat;
            }
        }

        private void AssignFavoriteToSelectedGames( bool fav ) {
            foreach( ListViewItem item in lstGames.SelectedItems ) {
                ( item.Tag as Game ).Favorite = fav;
            }
        }

        public Category CreateCategory() {
            GetStringDlg dlg = new GetStringDlg( string.Empty, "Create category", "Enter new category name:", "Create" );
            if( dlg.ShowDialog() == DialogResult.OK ) {
                Category newCat = gameData.AddCategory( dlg.Value );
                if( newCat != null ) {
                    FillCategoryList();
                    combCategory.SelectedItem = newCat;
                    return newCat;
                } else {
                    MessageBox.Show( String.Format( "Could not add category \"{0}\"", dlg.Value ), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
            }
            return null;
        }
        #endregion
    }

    // Implements the manual sorting of items by columns.
    class GameListViewItemComparer : IComparer {
        private int col;
        private int direction;
        private bool asInt;
        public GameListViewItemComparer( int column = 0, int dir = 1, bool asInt = false ) {
            this.col = column;
            this.direction = dir;
            this.asInt = asInt;
        }

        public int Compare( object x, object y ) {
            if( asInt ) {
                int a, b;
                if( int.TryParse( ( (ListViewItem)x ).SubItems[col].Text, out a ) && int.TryParse( ( (ListViewItem)y ).SubItems[col].Text, out b ) ) {
                    return direction * ( a - b );
                }
                return (int)x - (int)y;
            } else {
                return direction * String.Compare( ( (ListViewItem)x ).SubItems[col].Text, ( (ListViewItem)y ).SubItems[col].Text );
            }
        }
    }
}
