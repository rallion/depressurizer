using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SteamOrganizer {
    public partial class FormMain : Form {

        const int CAT_ALL_ID = 0;
        const string CAT_ALL_NAME = "<All>";
        const int CAT_FAV_ID = 1;
        const string CAT_FAV_NAME = "<Favorite>";
        const int CAT_UNC_ID = 2;
        const string CAT_UNC_NAME = "<Uncategorized>";

        GameData gameData;
        int sortColumn = 1;
        int sortDirection = 1;

        public FormMain() {
            gameData = new GameData();
            InitializeComponent();
            UpdateGameSorter();
            lstCategories.ListViewItemSorter = new CategoryListViewItemComparer();
            FillCategoryList();
        }

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
            ListViewItem item = new ListViewItem(CAT_ALL_NAME );
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
        }

        #region UI Event Handlers
        #region Drag and drop
        private void lstCategories_DragEnter( object sender, DragEventArgs e ) {
            e.Effect = DragDropEffects.Move;
        }

        private void lstCategories_DragDrop( object sender, DragEventArgs e ) {
            Point clientPoint = lstCategories.PointToClient( new Point( e.X, e.Y ) );
            ListViewItem dropItem = lstCategories.GetItemAt( clientPoint.X, clientPoint.Y );
            if( dropItem != null && dropItem.Index > 1 ) {
                Category newCat = dropItem.Tag as Category;
                if( newCat != null && e.Data.GetDataPresent( typeof( int[] ) ) ) {
                    gameData.SetGameCategories( (int[])e.Data.GetData( typeof( int[] ) ), newCat );
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
        private void menuToolsGetList_Click( object sender, EventArgs e ) {
            gameData.AddFromProfile( "rallion" );
            FillGameList();
        }
        #endregion

        private void cmdCatAdd_Click( object sender, EventArgs e ) {
            gameData.AddCategory();
            FillCategoryList();
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

        private void UpdateGameSorter() {
            lstGames.ListViewItemSorter = new GameListViewItemComparer( sortColumn, sortDirection, sortColumn == 0 );
        }

        private void lstCategories_BeforeLabelEdit( object sender, LabelEditEventArgs e ) {
            ListViewItem item = lstCategories.Items[e.Item];
            Category cat = item.Tag as Category;
            if( cat == null ) {
                e.CancelEdit = true;
            }
        }
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
