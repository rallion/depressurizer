using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Depressurizer {
    class AppSettings {

        protected static AppSettings instance = null;
        public static AppSettings Instance {
            get {
                if( instance == null ) {
                    instance = new AppSettings();
                }
                return instance;
            }
        }

        protected object threadLock = new object();

        protected bool outOfDate = false;

        protected Dictionary<string, string> settingsStore;

        public string FilePath;

        protected AppSettings() {
            settingsStore = new Dictionary<string, string>();
            FilePath = System.Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) + @"\Depressurizer\Settings.xml";
        }

        protected void Set( string key, string val ) {
            lock( threadLock ) {
                if( !settingsStore.ContainsKey( key ) || settingsStore[key] != val ) {
                    outOfDate = true;
                }
                settingsStore[key] = val;
            }
        }

        protected void Set( string key, int val ) {
            Set( key, val.ToString() );
        }

        protected string Get( string key, string defVal ) {
            lock( threadLock ) {
                string val;
                if( settingsStore.TryGetValue( key, out val ) ) {
                    return val;
                }
                Set( key, defVal );
                return defVal;
            }
        }

        protected int Get( string key, int defVal ) {
            lock( threadLock ) {
                string strVal;
                if( settingsStore.TryGetValue( key, out strVal ) ) {
                    int val;
                    if( int.TryParse( strVal, out val ) ) {
                        return val;
                    }
                }
                Set( key, defVal );
                return defVal;
            }
        }

        /*public void Clear( string key ) {
            lock( threadLock ) {
                if( settingsStore.ContainsKey( key ) ) {
                    settingsStore.Remove( key );
                }
            }
        }*/

        public void Save( bool force = false ) {
            lock( threadLock ) {
                if( force || outOfDate ) {
                    XmlDocument doc = new XmlDocument();
                    foreach( KeyValuePair<string, string> setting in settingsStore ) {
                        XmlElement element = doc.CreateElement( setting.Key );
                        element.InnerText = setting.Value;
                        doc.AppendChild( element );
                    }
                    doc.Save( FilePath );
                    outOfDate = false;
                }
            }
        }

        public void Load() {
            lock( threadLock ) {
                settingsStore = new Dictionary<string, string>();
                XmlDocument doc = new XmlDocument();
                using( FileStream fStream = File.OpenRead( FilePath ) ) {
                    doc.Load( fStream );
                }

                foreach( XmlNode node in doc.ChildNodes ) {
                    settingsStore[node.Name] = node.InnerText;
                }
                outOfDate = false;
            }
        }
    }
}
