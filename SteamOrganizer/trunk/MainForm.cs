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

        GameData gameData;
        int sortColumn = 0;
        int sortDirection = 1;

        public FormMain() {
            gameData = new GameData();
            InitializeComponent();
        }

        private void FillGameList() {
            lstGames.BeginUpdate();
            lstGames.Items.Clear();
            if( lstCategories.SelectedItems.Count > 0 ) {
                bool showAll = lstCategories.SelectedIndices[0] == 0;
                Category cat = lstCategories.SelectedItems[0].Tag as Category;
                foreach( Game g in gameData.Games.Values ) {
                    if( showAll || g.Category == cat ) {
                        string catName = ( g.Category == null ) ? "<Uncategorized>" : g.Category.Name;
                        ListViewItem item = new ListViewItem( new string[] { g.Name, catName, g.Id.ToString() } );
                        item.Tag = g;
                        lstGames.Items.Add( item );
                    }
                }
            }
            lstGames.EndUpdate();
        }

        private void FillCategoryList() {
            lstCategories.BeginUpdate();
            lstCategories.Items.Clear();
            lstCategories.Items.Add( "<All>" );
            lstCategories.Items.Add( "<Uncategorized>" );
            foreach( Category c in gameData.Categories ) {
                ListViewItem item = new ListViewItem( c.Name );
                item.Tag = c;
                lstCategories.Items.Add( item );
            }
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
            lstGames.ListViewItemSorter = new ListViewItemComparer( sortColumn, sortDirection, sortColumn == 2 );
        }
    }

    // Implements the manual sorting of items by columns.
    class ListViewItemComparer : IComparer {
        private int col;
        private int direction;
        private bool asInt;
        public ListViewItemComparer( int column = 0, int dir = 1, bool asInt = false ) {
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
