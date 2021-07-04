﻿// EnemyScriptPackage.cs is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 HyperBE32
 * Copyright (c) 2021 Sajid
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.IO;
using System.Collections.Generic;
using Marathon.IO.Headers;

namespace Marathon.IO.Formats.Miscellaneous
{
    public class ScriptPackage : FileBase
    {
        public ScriptPackage() { }

        public ScriptPackage(string file)
        {
            switch (Path.GetExtension(file))
            {
                case ".xml":
                    // ImportXML(file); // TODO: add XML reading/writing.
                    break;

                default:
                    Load(file);
                    break;
            }
        }

        public class ScriptParameter
        {
            /// <summary>
            /// Name of the state.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// <para>0 - Grounded</para>
            /// <para>1 - Flying (?)</para>
            /// <para>2 - Flying (?)</para>
            /// <para>3 - Wall (?)</para>
            /// <para>4 - Wall (?)</para>
            /// <para>5 - Idle Attack</para>
            /// </summary>
            public int State { get; set; }

            /// <summary>
            /// Total health points.
            /// </summary>
            public int Health { get; set; }

            /// <summary>
            /// Score given to the player upon dying.
            /// </summary>
            public int Score { get; set; }

            public float UnknownSingle1 { get; set; }

            /// <summary>
            /// The range the player must be in for the enemies to find them.
            /// </summary>
            public float FoundRangeIn { get; set; }

            /// <summary>
            /// TODO: needs more research.
            /// </summary>
            public float FoundRangeOut { get; set; }

            public float UnknownSingle4 { get; set; }
            public float UnknownSingle5 { get; set; }
            public float UnknownSingle6 { get; set; }
            public float UnknownSingle7 { get; set; }
            public float UnknownSingle8 { get; set; }

            /// <summary>
            /// The speed the enemies rotate at when searching for the player.
            /// </summary>
            public float RotationSpeed { get; set; }

            /// <summary>
            /// Something to do with rotation speed.
            /// </summary>
            public float UnknownSingle10 { get; set; }

            /// <summary>
            /// Something to do with rotation speed.
            /// </summary>
            public float UnknownSingle11 { get; set; }

            public float UnknownSingle12 { get; set; }
            public float UnknownSingle13 { get; set; }
            public float UnknownSingle14 { get; set; }
            public float UnknownSingle15 { get; set; }

            /// <summary>
            /// Total time taken for the enemies to react to the player in the search range.
            /// </summary>
            public float FoundReactionTime { get; set; }

            public float UnknownSingle17 { get; set; }

            /// <summary>
            /// The size of this struct.
            /// </summary>
            public const uint Size = 0x54;

            public ScriptParameter(BINAReader reader)
                => Read(reader);

            public void Read(BINAReader reader)
            {
                int nameOffset = reader.ReadInt32();

                State = reader.ReadInt32();
                Health = reader.ReadInt32();
                Score = reader.ReadInt32();

                UnknownSingle1 = reader.ReadSingle();
                FoundRangeIn = reader.ReadSingle();
                FoundRangeOut = reader.ReadSingle();
                UnknownSingle4 = reader.ReadSingle();
                UnknownSingle5 = reader.ReadSingle();
                UnknownSingle6 = reader.ReadSingle();
                UnknownSingle7 = reader.ReadSingle();
                UnknownSingle8 = reader.ReadSingle();
                RotationSpeed = reader.ReadSingle();
                UnknownSingle10 = reader.ReadSingle();
                UnknownSingle11 = reader.ReadSingle();
                UnknownSingle12 = reader.ReadSingle();
                UnknownSingle13 = reader.ReadSingle();
                UnknownSingle14 = reader.ReadSingle();
                UnknownSingle15 = reader.ReadSingle();
                FoundReactionTime = reader.ReadSingle();
                UnknownSingle17 = reader.ReadSingle();

                long pos = reader.BaseStream.Position;

                reader.JumpTo(nameOffset, true);
                Name = reader.ReadNullTerminatedString();

                reader.JumpTo(pos);
            }

            public void Write(BINAWriter writer)
            {
                writer.AddString($"StringOffset{writer.BaseStream.Position}", Name);

                writer.Write(State);
                writer.Write(Health);
                writer.Write(Score);

                writer.Write(UnknownSingle1);
                writer.Write(FoundRangeIn);
                writer.Write(FoundRangeOut);
                writer.Write(UnknownSingle4);
                writer.Write(UnknownSingle5);
                writer.Write(UnknownSingle6);
                writer.Write(UnknownSingle7);
                writer.Write(UnknownSingle8);
                writer.Write(RotationSpeed);
                writer.Write(UnknownSingle10);
                writer.Write(UnknownSingle11);
                writer.Write(UnknownSingle12);
                writer.Write(UnknownSingle13);
                writer.Write(UnknownSingle14);
                writer.Write(UnknownSingle15);
                writer.Write(FoundReactionTime);
                writer.Write(UnknownSingle17);
            }

            public override string ToString()
                => Name;
        }

        public List<ScriptParameter> Parameters { get; set; } = new List<ScriptParameter>();

        public override void Load(Stream fileStream)
        {
            var reader = new BINAReader(fileStream);
            reader.ReadHeader();

            var begin = reader.BaseStream.Position;
            var firstStringOffset = reader.ReadInt32() + reader.Offset;
            var count = firstStringOffset / ScriptParameter.Size;
            
            reader.JumpTo(begin);

            for (int i = 0; i < count; i++)
            {
                Parameters.Add(new ScriptParameter(reader));
            }
        }

        public override void Save(Stream fileStream)
        {
            var header = new BINAv1Header();
            var writer = new BINAWriter(fileStream, header);

            foreach (var parameter in Parameters)
            {
                parameter.Write(writer);
            }

            writer.WriteNulls(4);
            writer.FinishWrite(header);
        }
    }
}
