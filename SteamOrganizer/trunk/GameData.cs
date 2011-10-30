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
        public int Category;
        public bool Favorite;

        public Game( int id, string name ) {
            Id = id;
            Name = name;
            Category = -1;
            Favorite = false;
        }
    }

    public class GameData {
        public Dictionary<int, Game> Games;
        public List<string> Categories;

        public GameData() {
            Games = new Dictionary<int, Game>();
            Categories = new List<string>();
        }

        public void AddGame( int id, string name ) {
            if( !Games.ContainsKey( id ) ) {
                Games.Add( id, new Game( id, name ) );
            }
        }

        public Exception AddFromProfile(string profileName) {
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
                while( !line.StartsWith("</script>")) {
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
    }


}
