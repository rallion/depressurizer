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

        const int CAT_ALL_ID = 0;
        const string CAT_ALL_NAME = "<All>";
        const int CAT_FAV_ID = 1;
        const string CAT_FAV_NAME = "<Favorite>";
        const int CAT_UNC_ID = 2;
        const string CAT_UNC_NAME = "<Uncategorized>";

        const string COMBO_NEWCAT = "Add new...";
        const string COMBO_NONE = "Remove category";

        GameData gameData;
        int sortColumn = 1;
        int sortDirection = 1;

        public FormMain() {
            gameData = new GameData();
            InitializeComponent();
            combFavorite.SelectedIndex = 0;
            UpdateGameSorter();
            lstCategories.ListViewItemSorter = new CategoryListViewItemComparer();
            FillCategoryList();
        }

        public void ManualLoad() {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult res = dlg.ShowDialog();
            if( res == DialogResult.OK ) {
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
            }
        }

        public void ManualSave() {
            SaveFileDialog dlg = new SaveFileDialog();
            DialogResult res = dlg.ShowDialog();
            if( res == DialogResult.OK ) {
                try {
                    gameData.SaveSteamFile( dlg.FileName );
                    //TODO: display confirmation in status bar
                } catch( IOException e ) {
                    MessageBox.Show( e.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }

        public void ManualProfileLoad() {
            GetStringDlg dlg = new GetStringDlg( "", "Load profile", "Enter profile name:", "Load Profile" );
            if( dlg.ShowDialog() == DialogResult.OK ) {
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
                ListViewItem dropItem = lstCategories.GetItemAt( clientPoint.X, clientPoint.Y );
                if( dropItem != null ) {
                    Category newCat = dropItem.Tag as Category;
                    if( newCat != null ) {
                        gameData.SetGameCategories( (int[])e.Data.GetData( typeof( int[] ) ), newCat );
                        FillGameList();
                    } else if( dropItem.Tag is int && (int)dropItem.Tag == CAT_FAV_ID ) {
                        gameData.SetGameFavorites( (int[])e.Data.GetData( typeof( int[] ) ), true );
                        FillGameList();
                    } else if( dropItem.Tag is int && (int)dropItem.Tag == CAT_UNC_ID ) {
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
            if( CreateCategory() != null ) {
                FillCategoryList();
            }
        }

        private void cmdCatRename_Click( object sender, EventArgs e ) {
            if( lstCategories.SelectedItems.Count > 0 ) {
                RenameCategory( lstCategories.SelectedItems[0].Tag as Category );
            }
        }

        private void cmdCatDelete_Click( object sender, EventArgs e ) {
            if( lstCategories.SelectedItems.Count > 0 ) {
                DeleteCategory( lstCategories.SelectedItems[0].Tag as Category );
            }
        }

        private void cmdGameSetCategory_Click( object sender, EventArgs e ) {
            Category c;
            bool newCat;
            if( GetSelectedCategory( out c, out newCat ) ) {
                AssignCategoryToSelectedGames(c);
            }
            if( newCat ) {
                FillCategoryList();
            }
            FillGameList();
        }

        private void cmdGameSetFavorite_Click( object sender, EventArgs e ) {
            if( combFavorite.SelectedItem as string == "Yes" ) {
                AssignFavoriteToSelectedGames( true );
            } else if( combFavorite.SelectedItem as string == "No" ) {
                AssignFavoriteToSelectedGames( false );
            }
            FillGameList();
        }
        #endregion

        private void lstCategories_ItemSelectionChanged( object sender, ListViewItemSelectionChangedEventArgs e ) {
            FillGameList();
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

        public bool GetSelectedCategory( out Category result, out bool newCat ) {
            result = null;
            newCat = false;
            if( combCategory.SelectedItem is Category ) {
                result = combCategory.SelectedItem as Category;
                return true;
            } else if( combCategory.SelectedItem is string ) {
                if( (string)combCategory.SelectedItem == COMBO_NEWCAT ) {
                    result = CreateCategory();
                    newCat = true;
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
                        FillGameList();
                        return true;
                    }
                }
            }
            return false;
        }

        #region Utility
        private void FillGameList() {
            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            if( lstCategories.SelectedItems.Count > 0 ) {
                object catObj = lstCategories.SelectedItems[0].Tag;
                bool showAll = false;
                bool showFav = false;
                if( catObj is int ) {
                    if( (int)catObj == CAT_ALL_ID ) {
                        showAll = true;
                    } else if( (int)catObj == CAT_FAV_ID ) {
                        showFav = true;
                    }
                }
                Category cat = lstCategories.SelectedItems[0].Tag as Category;

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
            lstCategories.BeginUpdate();
            lstCategories.Items.Clear();
            ListViewItem item = new ListViewItem( CAT_ALL_NAME );
            item.Tag = CAT_ALL_ID;
            lstCategories.Items.Add( item );
            item = new ListViewItem( CAT_FAV_NAME );
            item.Tag = CAT_FAV_ID;
            lstCategories.Items.Add( item );
            item = new ListViewItem( CAT_UNC_NAME );
            item.Tag = CAT_UNC_ID;
            lstCategories.Items.Add( item );
            foreach( Category c in gameData.Categories ) {
                item = new ListViewItem( c.Name );
                item.Tag = c;
                lstCategories.Items.Add( item );
            }
            lstCategories.Sort();
            lstCategories.EndUpdate();

            combCategory.BeginUpdate();
            combCategory.Items.Clear();
            combCategory.Items.Add( COMBO_NEWCAT );
            combCategory.Items.Add( COMBO_NONE );
            combCategory.Items.Add( "" );
            foreach( Category c in gameData.Categories ) {
                combCategory.Items.Add( c );
            }
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

    class CategoryListViewItemComparer : IComparer {
        public CategoryListViewItemComparer() { }

        public int Compare( object x, object y ) {
            ListViewItem a = (ListViewItem)x;
            ListViewItem b = (ListViewItem)y;
            Category aC = a.Tag as Category;
            Category bC = b.Tag as Category;

            if( aC != null ) {
                if( bC != null ) {
                    return String.Compare( aC.Name, bC.Name );
                } else {
                    return 1;
                }
            } else {
                if( bC != null ) {
                    return -1;
                } else {
                    return (int)a.Tag - (int)b.Tag;
                }
            }
        }
    }
}
