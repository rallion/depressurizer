using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SteamOrganizer {
    public partial class FormMain : Form {

        GameData gameData;

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
                lstGames.Sort();
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
    }
}
