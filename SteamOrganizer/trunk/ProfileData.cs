using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Depressurizer {
    class ProfileData {

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

        public void SaveProfile( string path ) {
            XmlWriter writer = XmlWriter.Create( path );
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
        }

        /*public void SaveProfile( string path ) {
            XmlDocument doc = new XmlDocument();

            XmlElement profileElement = doc.CreateElement( "profile" );

            if( AccountID != null ) {
                XmlElement elem = doc.CreateElement( "account_id" );
                elem.InnerText = AccountID;
                profileElement.AppendChild( elem );
            }

            if( CommunityName != null ) {
                XmlElement elem = doc.CreateElement( "community_name" );
                elem.InnerText = CommunityName;
                profileElement.AppendChild( elem );
            }

            //TODO: bools

            XmlElement gameListElement = doc.CreateElement( "games" );

            foreach( Game g in GameData.Games.Values ) {
                XmlElement gameElement = doc.CreateElement( "game" );

                XmlElement elem = doc.CreateElement( "id" );
                elem.InnerText = g.Id.ToString();
                gameElement.AppendChild( elem );

                if( g.Name != null ) {
                    elem = doc.CreateElement( "name" );
                    elem.InnerText = g.Name;
                    gameElement.AppendChild( elem );
                }

                if( g.Category != null ) {
                    elem = doc.CreateElement( "category" );
                    elem.InnerText = g.Category.Name;
                    gameElement.AppendChild( elem );
                }

                if( g.Favorite ) {
                    elem = doc.CreateElement( "favorite" );
                    gameElement.AppendChild( elem );
                }

                gameListElement.AppendChild( gameElement );
            }

            profileElement.AppendChild( gameListElement );

            XmlElement exclusionListElement = doc.CreateElement( "exclusions" );

            foreach( int i in ExclusionList ) {
                XmlElement elem = doc.CreateElement( "exclusion" );
                elem.InnerText = i.ToString();
                exclusionListElement.AppendChild( elem );
            }

            profileElement.AppendChild( exclusionListElement );

            doc.AppendChild( profileElement );

            doc.Save( path );
        }*/
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
