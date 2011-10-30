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
            foreach( Game g in gameData.Games.Values ) {
                string cat = ( g.Category >= 0 )?gameData.Categories[g.Category]:"<Uncategorized>";
                lstGames.Items.Add( new ListViewItem( new string[] { g.Name, cat, g.Id.ToString() } ) );
            }
            lstGames.Sort();
            lstGames.EndUpdate();
        }

        private void lstCategories_DragEnter( object sender, DragEventArgs e ) {
            e.Effect = DragDropEffects.Move;
        }


        private void lstGames_DragOver( object sender, DragEventArgs e ) {
            //e.Effect = DragDropEffects.Move;
        }

        private void lstCategories_DragDrop( object sender, DragEventArgs e ) {
            Point clientPoint = lstCategories.PointToClient( new Point( e.X, e.Y ) ); 
            ListViewItem dropItem = lstCategories.GetItemAt( clientPoint.X, clientPoint.Y );
            if( dropItem != null && dropItem.Index > 1 ) {
                string destCat = dropItem.Text;
                string[] data = (string[])e.Data.GetData( typeof( string[] ) );
            }
        }

        private void lstGames_ItemDrag( object sender, ItemDragEventArgs e ) {
            string[] selectedNames = new string[lstGames.SelectedItems.Count];
            for( int i = 0; i < lstGames.SelectedItems.Count; i++ ) {
                selectedNames[i] = lstGames.SelectedItems[i].Text;
            }
            lstGames.DoDragDrop( selectedNames, DragDropEffects.Move );
        }

        private void menuToolsGetList_Click( object sender, EventArgs e ) {
            gameData.AddFromProfile( "rallion" );
            FillGameList();
        }
    }
}
