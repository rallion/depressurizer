using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SteamOrganizer {

    public enum ValueType {
        Array,
        Value
    }

    public class FileData {
        public ValueType ValueType;
        public Object ValueData;

        public FileData this[string key] {
            get {
                if( this.ValueType == ValueType.Value ) {
                    throw new Exception( string.Format( "Node is a value, not an array. Cannot get key {0}", key ) );
                }
                Dictionary<string, FileData> arrayData = (Dictionary<string, FileData>)ValueData;
                if( !arrayData.ContainsKey( key ) ) {
                    arrayData.Add( key, new FileData( ) );
                }
                return arrayData[key];
            }
            set {
                if( this.ValueType == ValueType.Value ) {
                    throw new Exception( string.Format( "Node is a value, not an array. Cannot set key {0}", key ) );
                }
                Dictionary<string, FileData> arrayData = (Dictionary<string, FileData>)ValueData;
                if( !arrayData.ContainsKey( key ) ) {
                    arrayData.Add( key, value );
                } else {
                    arrayData[key] = value;
                }
            }
        }

        public FileData() {
            ValueType = ValueType.Array;
            ValueData = new Dictionary<string, FileData>();

        }

        public FileData( string value ) {
            ValueType = ValueType.Value;
            ValueData = value;
        }

        public bool ContainsKey( string key ) {
            if( ValueType != ValueType.Array ) {
                return false;
            }
            return ( (Dictionary<string, FileData>)ValueData ).ContainsKey( key );
        }

        public static FileData ParseText( StreamReader stream ) {
            FileData thisLevel = new FileData();
            while( !stream.EndOfStream ) {

                SkipWhitespace( stream );
                // Get key
                char nextChar = (char)stream.Read();
                string key = null;
                if( nextChar == '"' ) {
                    key = GetStringToken( stream );
                } else {
                    break;
                }
                SkipWhitespace( stream );

                // Get value
                nextChar = (char)stream.Read();
                if( nextChar == '"' ) {
                    string value = GetStringToken( stream );
                    thisLevel[key] = new FileData( value );
                } else if( nextChar == '{' ) {
                    FileData value = ParseText( stream );
                    thisLevel[key] = value;
                }
            }
            return thisLevel;
        }

        private static string GetStringToken( StreamReader stream ) {
            bool escaped = false;
            bool stringDone = false;
            StringBuilder sb = new StringBuilder();
            char nextChar;
            do {
                nextChar = (char)stream.Read();
                if( escaped ) {
                    switch( nextChar ) {
                        case '\\':
                            sb.Append( '\\' );
                            break;
                        case '"':
                            sb.Append( '"' );
                            break;
                        case '\'':
                            sb.Append( '\'' );
                            break;
                    }
                    escaped = false;
                } else {
                    switch( nextChar ) {
                        case '\\':
                            escaped = true;
                            break;
                        case '"':
                            stringDone = true;
                            break;
                        default:
                            sb.Append( nextChar );
                            break;
                    }
                }
            } while( !stringDone );
            return sb.ToString();
        }

        private static void SkipWhitespace( StreamReader stream ) {
            char nextChar = (char)stream.Peek();
            while( nextChar == ' ' || nextChar == '\r' || nextChar == '\n' || nextChar == '\t' ) {
                stream.Read();
                nextChar = (char)stream.Peek();
            }
        }
        /*
        public Object GetAt( string[] args, int index = 0 ) {
            if( index >= args.Length ) {
                return this;
            }
            if( this.ValueType == ValueType.Array ) {
                Dictionary<String, FileData> data = (Dictionary<String, FileData>)Data;
                string childKey = args[index];
                if( data.ContainsKey( childKey ) ) {
                    FileData child = data[childKey];
                    return child.GetAt( args, index + 1 );
                }
            }
            return null;
        }

        public bool SetAt( string[] args, string value, int argIndex = 0 ) {
            if( argIndex >= args.Length ) {
                this.ValueType = ValueType.Value;
                this.Data = value;
                return true;
            }
            if( this.ValueType == ValueType.Value ) {
                return false;
            }

            
        }
        */
    }
}
