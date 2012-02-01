using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Depressurizer {
    public class ProfileData {

        public string FilePath = null;

        public GameData GameData = new GameData();

        public string AccountID = null;

        public string CommunityName = null;

        public bool AutoDownload = true;

        public bool AutoImport = false;

        public bool AutoExport = true;

        public bool DiscardExtraOnImport = false;

        public bool DiscardExtraOnExport = true;

        public SortedSet<int> ExclusionList = new SortedSet<int>();

        public static ProfileData LoadProfile( string path ) {
            ProfileData profile = new ProfileData();

            profile.FilePath = path;

            XmlDocument doc = new XmlDocument();
            doc.Load( path );

            XmlNode profileNode = doc.SelectSingleNode( "/profile" );

            if( profileNode != null ) {
                profile.CommunityName = XmlHelper.GetStringFromXmlElement( profileNode, "community_name", null );
                profile.AccountID = XmlHelper.GetStringFromXmlElement( profileNode, "account_id", null );

                profile.AutoDownload = XmlHelper.GetBooleanFromXmlElement( profileNode, "auto_download", profile.AutoDownload );
                profile.AutoImport = XmlHelper.GetBooleanFromXmlElement( profileNode, "auto_import", profile.AutoImport );
                profile.AutoExport = XmlHelper.GetBooleanFromXmlElement( profileNode, "auto_export", profile.AutoExport );
                profile.DiscardExtraOnImport = XmlHelper.GetBooleanFromXmlElement( profileNode, "discard_extra_on_import", profile.DiscardExtraOnImport );
                profile.DiscardExtraOnExport = XmlHelper.GetBooleanFromXmlElement( profileNode, "discard_extra_on_export", profile.DiscardExtraOnExport );

                XmlNode gameListNode = profileNode.SelectSingleNode( "games" );
                if( gameListNode != null ) {
                    XmlNodeList gameNodes = gameListNode.SelectNodes( "game" );
                    foreach( XmlNode node in gameNodes ) {
                        AddGameFromXmlNode( node, profile.GameData );
                    }
                }

                XmlNode exclusionListNode = profileNode.SelectSingleNode( "exclusions" );
                if( exclusionListNode != null ) {
                    XmlNodeList exclusionNodes = exclusionListNode.SelectNodes( "exclusion" );
                    foreach( XmlNode node in exclusionNodes ) {
                        int id;
                        if( XmlHelper.GetIntFromXmlElement( node, ".", out id ) ) {
                            profile.ExclusionList.Add( id );
                        }
                    }
                }

            }

            return profile;
        }

        private static void AddGameFromXmlNode( XmlNode node, GameData data ) {
            int id;
            if( XmlHelper.GetIntFromXmlElement( node, "id", out id ) ) {
                string name = XmlHelper.GetStringFromXmlElement( node, "name", null );
                Game game = new Game( id, name );
                data.Games.Add( id, game );

                string catName;
                if( XmlHelper.GetStringFromXmlElement( node, "category", out catName ) ) {
                    game.Category = data.GetCategory( catName );
                }

                game.Favorite = ( node.SelectSingleNode( "favorite" ) != null );
            }
        }

        public void SaveProfile() {
            SaveProfile( FilePath );
        }

        public bool SaveProfile( string path ) {
            XmlWriterSettings writeSettings = new XmlWriterSettings();
            writeSettings.CloseOutput = true;
            writeSettings.Indent = true;
            
            XmlWriter writer;
            try {
                writer = XmlWriter.Create( path, writeSettings );
            } catch {
                return false;
            }
            writer.WriteStartElement( "profile" );

            if( AccountID != null ) {
                writer.WriteElementString( "account_id", AccountID );
            }

            if( CommunityName != null ) {
                writer.WriteElementString( "community_name", CommunityName );
            }

            writer.WriteElementString( "auto_download", AutoDownload.ToString() );
            writer.WriteElementString( "auto_import", AutoImport.ToString() );
            writer.WriteElementString( "auto_export", AutoExport.ToString() );
            writer.WriteElementString( "discard_extra_on_import", DiscardExtraOnImport.ToString() );
            writer.WriteElementString( "discard_extra_on_export", DiscardExtraOnExport.ToString() );

            writer.WriteStartElement( "games" );

            foreach( Game g in GameData.Games.Values ) {
                writer.WriteStartElement( "game" );

                writer.WriteElementString( "id", g.Id.ToString() );

                if( g.Name != null ) {
                    writer.WriteElementString( "name", g.Name );
                }

                if( g.Category != null ) {
                    writer.WriteElementString( "category", g.Category.Name );
                }

                if( g.Favorite ) {
                    writer.WriteElementString( "favorite", true.ToString() );
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement( "exclusions" );

            foreach( int i in ExclusionList ) {
                writer.WriteElementString( "exclusion", i.ToString() );
            }

            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.Close();
            FilePath = path;
            return true;
        }
    }

    public static class XmlHelper {

        public static bool GetStringFromXmlElement( XmlNode refNode, string path, out string val ) {
            if( refNode != null ) {
                XmlNode node = refNode.SelectSingleNode( path + "/text()" );
                if( node != null && !string.IsNullOrEmpty( node.InnerText ) ) {
                    val = node.InnerText;
                    return true;
                }
            }
            val = null;
            return false;
        }

        public static string GetStringFromXmlElement( XmlNode refNode, string path, string defVal ) {
            string val;
            if( GetStringFromXmlElement( refNode, path, out val ) ) {
                return val;
            }
            return defVal;
        }

        public static bool GetIntFromXmlElement( XmlNode refNode, string path, out int val ) {
            string strVal;
            if( GetStringFromXmlElement( refNode, path, out strVal ) ) {
                if( int.TryParse( strVal, out val ) ) {
                    return true;
                }
            }
            val = 0;
            return false;
        }

        public static int GetIntFromXmlElement( XmlNode refNode, string path, int defVal ) {
            int val;
            if( GetIntFromXmlElement( refNode, path, out val ) ) {
                return val;
            }
            return defVal;
        }

        public static bool GetBooleanFromXmlElement( XmlNode refNode, string path, out bool val ) {
            string strVal;
            if( GetStringFromXmlElement( refNode, path, out strVal ) ) {
                if( bool.TryParse( strVal, out val ) ) {
                    return true;
                }
            }
            val = false;
            return false;
        }

        public static bool GetBooleanFromXmlElement( XmlNode refNode, string path, bool defVal ) {
            bool val;
            if( GetBooleanFromXmlElement( refNode, path, out val ) ) {
                return val;
            }
            return defVal;
        }
    }
}
