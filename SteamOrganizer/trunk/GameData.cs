using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Depressurizer {
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

    public class Category : IComparable {
        public string Name;

        public Category( string name ) {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

        public int CompareTo( object o ) {
            return Name.CompareTo( Name as string );
        }
    }

    public class GameData {
        #region Fields
        public Dictionary<int, Game> Games;
        public List<Category> Categories;

        private FileNode backingData;
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

        private void SetGameName( int id, string name ) {
            if( !Games.ContainsKey( id ) ) {
                Games.Add( id, new Game( id, name ) );
            } else {
                Games[id].Name = name;
            }
        }

        public Category AddCategory( string name ) {
            if( CategoryExists( name ) ) {
                return null;
            } else {
                Category newCat = new Category( name );
                Categories.Add( newCat );
                Categories.Sort();
                return newCat;
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

        public void SetGameFavorites( int[] gameIDs, bool fav ) {
            for( int i = 0; i < gameIDs.Length; i++ ) {
                Games[gameIDs[i]].Favorite = fav;
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

        public Category GetCategory( string name ) {
            foreach( Category c in Categories ) {
                if( c.Name == name ) return c;
            }
            Category newCat = new Category( name );
            Categories.Add( newCat );
            return newCat;
        }
        #endregion

        public int LoadProfile( string profileName ) {
            string url = string.Format( @"http://steamcommunity.com/id/{0}/games/?tab=all", profileName );
            WebRequest req = HttpWebRequest.Create( url );
            WebResponse response = req.GetResponse();
            StreamReader reader = new StreamReader( response.GetResponseStream() );

            string line;

            // Get to relevant javascript
            do {
                line = reader.ReadLine();
                if( line == null ) return 0;
            } while( line != null && !line.Contains( "rgGames" ) );

            int loadedGames = 0;
            line = reader.ReadLine();
            Regex regex = new Regex( @"rgGames\['(\d+)'\]\s*=\s*'(.*)';" );
            while( !line.StartsWith( "</script>" ) ) {
                Match m = regex.Match( line );
                if( m.Success ) {
                    int id;
                    if( int.TryParse( m.Groups[1].Value, out id ) ) {
                        // TODO: Strip escape characters out
                        SetGameName( id, m.Groups[2].Value );
                        loadedGames++;
                    }
                }
                line = reader.ReadLine();
            }
            return loadedGames;


        }

        public int LoadSteamFile( string filePath ) {
            int loadedGames = 0;
            FileNode dataRoot;

            using( StreamReader reader = new StreamReader( filePath, false ) ) {
                dataRoot = FileNode.Load( reader );
            }

            Games.Clear();
            Categories.Clear();
            this.backingData = dataRoot;

            FileNode appsNode = dataRoot.GetNodeAt( new string[] { "UserLocalConfigStore", "Software", "Valve", "Steam", "apps" }, true );
            foreach( KeyValuePair<string, FileNode> gameNode in (Dictionary<string, FileNode>)appsNode.NodeData ) {
                int gameId;
                if( int.TryParse( gameNode.Key, out gameId ) ) {
                    Category cat = null;
                    bool fav = false;
                    if( gameNode.Value.ContainsKey( "tags" ) ) {
                        FileNode tagsNode = gameNode.Value["tags"];
                        foreach( FileNode tag in ( (Dictionary<string, FileNode>)tagsNode.NodeData ).Values ) {
                            if( tag.NodeType == ValueType.Value ) {
                                string tagName = (string)tag.NodeData;
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
                        loadedGames++;
                    }
                    Games[gameId].Category = cat;
                    Games[gameId].Favorite = fav;
                }
            }
            return loadedGames;

        }

        public void SaveSteamFile( string path ) {
            FileNode appListNode = backingData.GetNodeAt( new string[] { "UserLocalConfigStore", "Software", "Valve", "Steam", "apps" } );

            foreach( Game game in Games.Values ) {
                FileNode gameNode = appListNode[game.Id.ToString()];
                gameNode.RemoveSubnode( "tags" );
                if( game.Category != null || game.Favorite ) {
                    FileNode tagsNode = gameNode["tags"];
                    int key = 0;
                    if( game.Category != null ) {
                        tagsNode[key.ToString()] = new FileNode( game.Category.Name );
                        key++;
                    }
                    if( game.Favorite ) {
                        tagsNode[key.ToString()] = new FileNode( "favorite" );
                    }
                }
            }

            appListNode.CleanTree();

            using( StreamWriter writer = new StreamWriter( path, false ) ) {
                backingData.Save( writer );
            }
        }
    }
}
