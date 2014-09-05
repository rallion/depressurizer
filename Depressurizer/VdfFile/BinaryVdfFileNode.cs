﻿/*
Copyright 2014 Juan Luis Podadera.

This file is part of Depressurizer.

Depressurizer is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Depressurizer is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Depressurizer.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Depressurizer.VdfFile
{
    /// <summary>
    ///     Represents a single node in a Valve's VDF binary file.
    /// </summary>
    public class BinaryVdfFileNode : VdfFileNode
    {
        /// <summary>
        ///     Creates a new array-type node
        /// </summary>
        public BinaryVdfFileNode()
        {
        }

        /// <summary>
        ///     Creates a new value-type node
        /// </summary>
        /// <param name="value">Value of the string</param>
        public BinaryVdfFileNode(string value)
            : base(value)
        {
        }

        public BinaryVdfFileNode(int value)
            : base(value)
        {
        }

        #region Utility

        /// <summary>
        ///     Reads a from the specified stream until it reaches a string terminator (double quote with no escaping slash).
        ///     The opening double quote should already be read, and the last one will be discarded.
        /// </summary>
        /// <returns>The string encapsulated by the quotes.</returns>
        private static string GetStringToken(BinaryReader reader)
        {
            bool endOfStream = false;
            bool stringDone = false;
            var sb = new StringBuilder();
            do
            {
                try
                {
                    byte nextByte = reader.ReadByte();

                    if (nextByte == 0)
                    {
                        stringDone = true;
                    }
                    else
                    {
                        sb.Append((char) nextByte);
                    }
                }
                catch (EndOfStreamException)
                {
                    endOfStream = true;
                }
            } while (!stringDone && !(endOfStream));

            if (!stringDone)
            {
                throw new ParseException(GlobalStrings.TextVdfFile_UnexpectedEOF);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Writes a array key to a stream, adding start/end bytes.
        /// </summary>
        /// <param name="writer">Stream to write to</param>
        /// <param name="arrayKey">String to write</param>
        private void WriteArrayKey(BinaryWriter writer, string arrayKey)
        {
            writer.Write((byte) 0);
            writer.Write(arrayKey.ToCharArray());
            writer.Write((byte) 0);
        }

        /// <summary>
        ///     Writes a pair o key and value to a stream, adding star/end and separator bytes
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private void WriteStringValue(BinaryWriter writer, string key, string val)
        {
            writer.Write((byte) 1);
            writer.Write(key.ToCharArray());
            writer.Write((byte) 0);
            writer.Write(val.ToCharArray());
            writer.Write((byte) 0);
        }

        private void WriteIntegerValue(BinaryWriter writer, string key, int val)
        {
            writer.Write((byte) 2);
            writer.Write(key.ToCharArray());
            writer.Write((byte) 0);
            writer.Write(val);
        }

        /// <summary>
        ///     Write an end byte to stream
        /// </summary>
        /// <param name="writer"></param>
        private void WriteEndByte(BinaryWriter writer)
        {
            writer.Write((byte) 8);
        }

        #endregion

        #region Saving and loading

        /// <summary>
        ///     Loads a FileNode from stream.
        /// </summary>
        /// <param name="stream">Stream to load from</param>
        /// <returns>FileNode representing the contents of the stream.</returns>
        public static BinaryVdfFileNode Load(BinaryReader stream)
        {
            var thisLevel = new BinaryVdfFileNode();

            bool endOfStream = false;


            while (!endOfStream)
            {
                //SkipWhitespace( stream );
                byte nextByte;
                try
                {
                    nextByte = stream.ReadByte();
                }
                catch (EndOfStreamException)
                {
                    endOfStream = true;
                    nextByte = 8;
                }
                // Get key
                string key;
                if (endOfStream || nextByte == 8)
                {
                    break;
                }
                if (nextByte == 0)
                {
                    key = GetStringToken(stream);
                    BinaryVdfFileNode newNode = Load(stream);
                    thisLevel[key] = newNode;
                }
                else if (nextByte == 1)
                {
                    key = GetStringToken(stream);
                    thisLevel[key] = new BinaryVdfFileNode(GetStringToken(stream));
                }
                else if (nextByte == 2)
                {
                    key = GetStringToken(stream);
                    int val = stream.ReadInt32();
                    thisLevel[key] = new BinaryVdfFileNode(val);
                }
                else
                {
                    throw new ParseException(string.Format(GlobalStrings.TextVdfFile_UnexpectedCharacterKey,
                        nextByte));
                }
            }
            return thisLevel;
        }

        /// <summary>
        ///     Write complete VdfFileNode to a stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public void Save(BinaryWriter stream)
        {
            Save(stream, null);
        }

        /// <summary>
        ///     Writes this FileNode and children to a stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="actualKey">Name of node to write.</param>
        private void Save(BinaryWriter stream, string actualKey)
        {
            switch (NodeType)
            {
                case ValueType.Array:
                    if (!string.IsNullOrEmpty(actualKey))
                        WriteArrayKey(stream, actualKey);
                    Dictionary<string, VdfFileNode> data = NodeArray;
                    foreach (var entry in data)
                    {
                        ((BinaryVdfFileNode) entry.Value).Save(stream, entry.Key);
                    }
                    WriteEndByte(stream);
                    break;
                case ValueType.String:
                    if (!string.IsNullOrEmpty(actualKey))
                        WriteStringValue(stream, actualKey, NodeString);
                    break;
                case ValueType.Int:
                    if (!string.IsNullOrEmpty(actualKey))
                    {
                        WriteIntegerValue(stream, actualKey, NodeInt);
                    }
                    break;
            }
        }

        #endregion

        protected override VdfFileNode CreateNode()
        {
            return new BinaryVdfFileNode();
        }
    }
}