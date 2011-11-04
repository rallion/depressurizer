using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Depressurizer {

    public enum ValueType {
        Array,
        Value
    }

    public class FileNode {
        public ValueType NodeType;
        public Object NodeData;

        public FileNode this[string key] {
            get {
                if( this.NodeType == ValueType.Value ) {
                    throw new Exception( string.Format( "Node is a value, not an array. Cannot get key {0}", key ) );
                }
                Dictionary<string, FileNode> arrayData = (Dictionary<string, FileNode>)NodeData;
                if( !arrayData.ContainsKey( key ) ) {
                    arrayData.Add( key, new FileNode() );
                }
                return arrayData[key];
            }
            set {
                if( this.NodeType == ValueType.Value ) {
                    throw new Exception( string.Format( "Node is a value, not an array. Cannot set key {0}", key ) );
                }
                Dictionary<string, FileNode> arrayData = (Dictionary<string, FileNode>)NodeData;
                if( !arrayData.ContainsKey( key ) ) {
                    arrayData.Add( key, value );
                } else {
                    arrayData[key] = value;
                }
            }
        }
        public Dictionary<string, FileNode> NodeArray {
            get {
                return NodeData as Dictionary<string, FileNode>;
            }
        }

        public FileNode() {
            NodeType = ValueType.Array;
            NodeData = new Dictionary<string, FileNode>();

        }

        public FileNode( string value ) {
            NodeType = ValueType.Value;
            NodeData = value;
        }

        #region Utility

        /// <summary>
        /// Reads a from the specified stream until it reaches a string terminator (double quote with no escaping slash).
        /// The opening double quote should already be read, and the last one will be discarded.
        /// </summary>
        /// <param name="stream">The stream to read from. After the operation, the stream position will be just past the closing quote.</param>
        /// <returns>The string encapsulated by the quotes.</returns>
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
            } while( !stringDone && !stream.EndOfStream );
            if( !stringDone ) {
                if( stream.EndOfStream ) {
                    throw new ParseException( "Unexpected end-of-file reached: Unterminated string." );
                }
            }
            return sb.ToString();
        }

        private static void SkipWhitespace( StreamReader stream ) {
            char nextChar = (char)stream.Peek();
            while( nextChar == ' ' || nextChar == '\r' || nextChar == '\n' || nextChar == '\t' ) {
                stream.Read();
                nextChar = (char)stream.Peek();
            }
        }

        private void WriteFormattedString( StreamWriter stream, string s ) {
            stream.Write( "\"" );
            stream.Write( s.Replace( "\"", "\\\"" ) );
            stream.Write( "\"" );
        }

        private void WriteWhitespace( StreamWriter stream, int indent ) {
            for( int i = 0; i < indent; i++ ) {
                stream.Write( '\t' );
            }
        }

        private bool IsEmpty() {
            if( NodeArray != null ) {
                return NodeArray.Count == 0;
            } else {
                return ( NodeData as string ) == null;
            }
        }
        #endregion
        #region Accessors
        public FileNode GetNodeAt( string[] args, bool create = true, int index = 0 ) {
            if( index >= args.Length ) {
                return this;
            }
            if( this.NodeType == ValueType.Array ) {
                Dictionary<String, FileNode> data = (Dictionary<String, FileNode>)NodeData;
                if( ContainsKey( args[index] ) ) {
                    return data[args[index]].GetNodeAt( args, create, index + 1 );
                } else if( create ) {
                    FileNode newNode = new FileNode();
                    data.Add( args[index], newNode );
                    return newNode.GetNodeAt( args, create, index + 1 );
                }
            }
            return null;
        }

        public bool ContainsKey( string key ) {
            if( NodeType != ValueType.Array ) {
                return false;
            }
            return ( (Dictionary<string, FileNode>)NodeData ).ContainsKey( key );
        }

        #endregion

        public bool RemoveSubnode( string key ) {
            return NodeArray.Remove( key );
        }

        public static FileNode Load( StreamReader stream ) {
            FileNode thisLevel = new FileNode();
            while( !stream.EndOfStream ) {

                SkipWhitespace( stream );
                // Get key
                char nextChar = (char)stream.Read();
                string key = null;
                if( stream.EndOfStream || nextChar == '}' ) {
                    break;
                } else if( nextChar == '"' ) {
                    key = GetStringToken( stream );
                } else {
                    throw new ParseException( string.Format( "Unexpected character '{0}' found when expecting key.", nextChar ) );
                }
                SkipWhitespace( stream );

                // Get value
                nextChar = (char)stream.Read();
                if( nextChar == '"' ) {
                    string value = GetStringToken( stream );
                    thisLevel[key] = new FileNode( value );
                } else if( nextChar == '{' ) {
                    FileNode value = Load( stream );
                    thisLevel[key] = value;
                } else {
                    throw new ParseException( string.Format( "Unexpected character '{0}' found when expecting value.", nextChar ) );
                }
            }
            return thisLevel;
        }

        public void Save( StreamWriter stream, int indent = 0 ) {
            if( NodeType == ValueType.Array ) {
                Dictionary<string, FileNode> data = NodeArray;
                foreach( KeyValuePair<string, FileNode> entry in data ) {
                    if( entry.Value.NodeType == ValueType.Array ) {
                        WriteWhitespace( stream, indent );
                        WriteFormattedString( stream, entry.Key );
                        stream.WriteLine();

                        WriteWhitespace( stream, indent );
                        stream.WriteLine( '{' );

                        entry.Value.Save( stream, indent + 1 );

                        WriteWhitespace( stream, indent );
                        stream.WriteLine( '}' );
                    } else {
                        WriteWhitespace( stream, indent );
                        WriteFormattedString( stream, entry.Key );
                        stream.Write( "\t\t" );

                        WriteFormattedString( stream, entry.Value.NodeData as string );
                        stream.WriteLine();
                    }
                }
            } else {

            }
        }

        public void CleanTree() {
            Dictionary<string, FileNode> nodes = NodeArray;
            if( nodes != null ) {
                string[] keys = nodes.Keys.ToArray<string>();
                foreach( string key in keys ) {
                    nodes[key].CleanTree();
                    if( nodes[key].IsEmpty() ) {
                        NodeArray.Remove( key );
                    }
                }
            }
        }
    }

    public class ParseException : ApplicationException {
        public ParseException() : base() { }
        public ParseException( string message ) : base() { }
    }
}
