using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace SteamOrganizer {
    public class Game {
        public string Name;
        public int Id;
        public Category Category;
        public bool Favorite;

        public Game( int id, string name ) {
            Id = id;
            Name = name;
            Category = null;
            Favorite = false;
        }
    }

    public class Category {
        public string Name;

        public Category( string name ) {
            Name = name;
        }
    }

    public class GameData {
        #region Fields
        public Dictionary<int, Game> Games;
        public List<Category> Categories;
        #endregion

        public GameData() {
            Games = new Dictionary<int, Game>();
            Categories = new List<Category>();
        }

        #region Modifiers
        public void AddGame( int id, string name ) {
            if( !Games.ContainsKey( id ) ) {
                Games.Add( id, new Game( id, name ) );
            }
        }

        public bool AddCategory() {
            return AddCategory( GetNewCategoryName() );
        }

        public bool AddCategory( string name ) {
            if( CategoryExists( name ) ) {
                return false;
            } else {
                Categories.Add( new Category( name ) );
                return true;
            }
        }

        public void RemoveCategory( Category c ) {
            Categories.Remove( c );
            foreach( Game g in Games.Values ) {
                if( g.Category == c ) g.Category = null;
            }
        }

        public bool RenameCategory( Category c, string newName ) {
            if( !CategoryExists( newName ) ) {
                c.Name = newName;
                return true;
            }
            return false;
        }

        public void SetGameCategories( int[] gameIDs, Category newCat ) {
            for( int i = 0; i < gameIDs.Length; i++ ) {
                Games[gameIDs[i]].Category = newCat;
            }
        }
        #endregion

        #region Accessors
        public bool CategoryExists( string name ) {
            foreach( Category c in Categories ) {
                if( c.Name == name ) {
                    return true;
                }
            }
            return false;
        }

        public string GetNewCategoryName() {
            string baseName = "New Category";
            int number = 0;
            string currName;
            do {
                number++;
                currName = string.Format( "{0} {1}", baseName, number );
            } while( CategoryExists( currName ) );
            return currName;
        }
        #endregion

        public Exception AddFromProfile( string profileName ) {
            try {
                string url = string.Format( @"http://steamcommunity.com/id/{0}/games/?tab=all", profileName );
                WebRequest req = HttpWebRequest.Create( url );
                WebResponse response = req.GetResponse();
                Stream respStream = response.GetResponseStream();
                StreamReader reader = new StreamReader( respStream );
                string line;

                // Get to relevant javascript
                do {
                    line = reader.ReadLine();
                } while( !line.Contains( "rgGames" ) );

                line = reader.ReadLine();
                Regex regex = new Regex( @"rgGames\['(\d+)'\]\s*=\s*'(.*)';" );
                while( !line.StartsWith( "</script>" ) ) {
                    Match m = regex.Match( line );
                    if( m.Success ) {
                        int id;
                        if( int.TryParse( m.Groups[1].Value, out id ) ) {
                            AddGame( id, m.Groups[2].Value );
                        }
                    }
                    line = reader.ReadLine();
                }
                return null;
            } catch( WebException e ) {
                return e;
            }

        }

        /*   private void SetGameInfo( int gameId, string title, string category, bool? favorite ) {
               if( Games.ContainsKey() ) {

               } else {
                   Game newGame = new Game( gameId, (title == null)?string.Empty:title );
                   if( cate
                   Games.Add( gameId, newGame );
               }
           }

   */
        public Category GetCategory( string name ) {
            foreach( Category c in Categories ) {
                if( c.Name == name ) return c;
            }
            Category newCat = new Category( name );
            Categories.Add( newCat );
            return newCat;
        }

        public Exception GetDataFromSteamFile( FileInfo file ) {
            StreamReader reader = new StreamReader( file.OpenRead() );
            FileData dataRoot = FileData.ParseText( reader );
            reader.Close();

            FileData appsNode = dataRoot["UserLocalConfigStore"]["Software"]["Valve"]["Steam"]["apps"];
            foreach( KeyValuePair<string, FileData> gameNode in (Dictionary<string, FileData>)appsNode.ValueData ) {
                int gameId;
                if( int.TryParse( gameNode.Key, out gameId ) ) {
                    Category cat = null;
                    bool fav = false;
                    if( gameNode.Value.ContainsKey( "tags" ) ) {
                        FileData tagsNode = gameNode.Value["tags"];
                        foreach( FileData tag in ( (Dictionary<string, FileData>)tagsNode.ValueData ).Values ) {
                            if( tag.ValueType == ValueType.Value ) {
                                string tagName = (string)tag.ValueData;
                                if( tagName == "favorite" ) {
                                    fav = true;
                                } else {
                                    cat = GetCategory( tagName );
                                }
                            }
                        }
                    }
                    if( !Games.ContainsKey( gameId ) ) {
                        Game newGame = new Game( gameId, string.Empty );
                        Games.Add( gameId, newGame );
                    }
                    Games[gameId].Category = cat;
                    Games[gameId].Favorite = fav;
                }
            }
            return null;
        }
    }


}
