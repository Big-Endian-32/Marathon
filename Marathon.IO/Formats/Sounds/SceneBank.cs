﻿// SceneBank.cs is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 Radfordhound
 * Copyright (c) 2021 Knuxfan24
 * Copyright (c) 2021 HyperBE32
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
using System.Xml.Linq;
using System.Collections.Generic;
using Marathon.IO.Headers;
using Marathon.IO.Exceptions;

namespace Marathon.IO.Formats.Sounds
{
    /// <summary>
    /// <para>File base for the SBK format.</para>
    /// <para>Used in SONIC THE HEDGEHOG for defining <a href="https://www.criware.com/">CriWare</a> sound effects in <see cref="CueSheetBinary"/> files.</para>
    /// </summary>
    public class SceneBank : FileBase
    {
        /* TODO: Experiment with the Unknown values in the Cue entries and see what they do.
                 Also try messing with the Unknown value in the Header and see if it affects anything. */

        public SceneBank() { }

        public SceneBank(string file)
        {
            switch (Path.GetExtension(file))
            {
                case ".xml":
                    ImportXML(file);
                    break;

                default:
                    Load(file);
                    break;
            }
        }

        public class SoundCue
        {
            public string Name;                            // Name of this Cue in the Scene Bank

            public uint Category;                          // Uncertain what exactly this affects

            public float UnknownSingle_1,                  // TODO: Unknown - possibly a flag?
                         UnknownSingle_2;                  // TODO: Unknown - possibly a flag?

            public string Stream;                          // XMA this Cue uses, if null, assume it uses a CSB instead
        }

        public const string Signature = "SBNK",
                            Extension = ".sbk";

        public string Name;

        public List<SoundCue> Cues = new List<SoundCue>();

        public override void Load(Stream fileStream)
        {
            BINAReader reader = new BINAReader(fileStream);
            reader.ReadHeader();

            string signature = reader.ReadSignature(4);
            if (signature != Signature)
                throw new InvalidSignatureException(Signature, signature);

            uint UnknownUInt32_1 = reader.ReadUInt32();   // These four bytes seems to always be { 20, 06, 07, 00 } in official files.

            uint bankNameOffset    = reader.ReadUInt32(); // Offset to the Scene Bank name (seemingly always { 00, 00, 00, 18 } in official files).
            uint cueNameOffset     = reader.ReadUInt32(); // Offset of the first entry in the Scene Bank (seemingly always { 00, 00, 00, 64 } in official files).
            uint cueIndiciesOffset = reader.ReadUInt32(); // Offset to the number list for non-stream indices (set to { 00, 00, 00, 00 } if the file doesn't have any).
            uint streamOffset      = reader.ReadUInt32(); // Offset to the table for XMA names (set to { 00, 00, 00, 00 } if the file doesn't have any).

            Name                = new string(reader.ReadChars(64)); // Scene Bank's name.
            uint cueCount       = reader.ReadUInt32();              // Total Number of Cues in this Scene Bank.
            uint csbCueCount    = reader.ReadUInt32();              // Amount of Cues in this Scene Bank which pull their data from a corresponding CSB file.
            uint streamCueCount = reader.ReadUInt32();              // Amount of Cues in this Scene Bank which use XMA files.

            int streams = 0; // Keep track of which stream we're on so we know where to jump to in the XMA String Table.

            for (int i = 0; i < cueCount; i++)
            {
                SoundCue cue = new SoundCue() { Name = new string(reader.ReadChars(32)) };

                uint cueType  = reader.ReadUInt32();
                uint cueIndex = reader.ReadUInt32();

                cue.Category        = reader.ReadUInt32();
                cue.UnknownSingle_1 = reader.ReadSingle();
                cue.UnknownSingle_2 = reader.ReadSingle();

                if (cueType == 1)
                {
                    // Store current position for later so we can jump back.
                    long pos = reader.BaseStream.Position;

                    reader.JumpTo(streamOffset, true);
                    reader.JumpAhead(4 * streams);                  // Jump ahead to the right offset for our Cue's XMA.
                    reader.JumpTo(reader.ReadUInt32(), true);
                    cue.Stream = reader.ReadNullTerminatedString(); // Read the XMA's name for this Cue.

                    reader.JumpTo(pos);                             // Jump back to where we were.

                    streams++;
                }

                // Save Cue to list.
                Cues.Add(cue); 
            }
        }

        public override void Save(Stream fileStream)
        {
            BINAv1Header Header = new BINAv1Header();
            BINAWriter writer = new BINAWriter(fileStream, Header);

            // Determine amount of Cues that use a CSB and amount that use an XMA.
            int csbCueCount = 0;
            int streamCueCount = 0;

            for (int i = 0; i < Cues.Count; i++)
            {
                if (string.IsNullOrEmpty(Cues[i].Stream))
                    csbCueCount++;
                else
                    streamCueCount++;
            }

            writer.WriteSignature(Signature);

            writer.Write(537265920); // Hardcoded as all official files seem to have this number in this position.

            writer.AddOffset("banksOffset");
            writer.AddOffset("cueNamesOffset");
            writer.AddOffset("cueIndicesOffset");
            writer.AddOffset("streamsOffset");

            writer.FillInOffset("banksOffset", true);

            writer.WriteNullPaddedString(Name, 64);
            writer.Write(Cues.Count);
            writer.Write(csbCueCount);
            writer.Write(streamCueCount);

            // Cue Information
            writer.FillInOffset("cueNamesOffset", true);

            // Keep track of the entry types and IDs.
            int csbCueID = 0;
            int streamCueID = 0;

            for (int i = 0; i < Cues.Count; i++)
            {
                writer.WriteNullPaddedString(Cues[i].Name, 32);

                // Write a CSB-based entry.
                if (string.IsNullOrEmpty(Cues[i].Stream))
                {
                    writer.Write(0);
                    writer.Write(csbCueID);
                    csbCueID++;
                }

                // Write an XMA-based entry.
                else
                {
                    writer.Write(1);
                    writer.Write(streamCueID);
                    streamCueID++;
                }

                writer.Write(Cues[i].Category);
                writer.Write(Cues[i].UnknownSingle_1);
                writer.Write(Cues[i].UnknownSingle_2);
            }

            // CSB Cue ID List (if any are present).
            if (csbCueCount != 0)
            {
                writer.FillInOffset("cueIndicesOffset", true);

                for (int i = 0; i < csbCueCount; i++)
                    writer.Write(i);
            }

            // Stream Names (if any are present).
            if (streamCueCount != 0)
            {
                writer.FillInOffset("streamsOffset", true);

                for (int i = 0; i < Cues.Count; i++)
                {
                    if (Cues[i].Stream != null)
                        writer.AddString($"streamOffset{i}", $"{Cues[i].Stream}");
                }
            }

            // Write the footer.
            writer.FinishWrite(Header);
        }

        public void ExportXML(string filepath)
        {
            // Root element.
            XElement rootElem = new XElement("SBK");

            // Replace empty characters with nothing.
            XAttribute sbkNameAttr = new XAttribute("Name", Name.Replace("\0", ""));
            rootElem.Add(sbkNameAttr);

            // Cue elements.
            foreach (SoundCue cue in Cues)
            {
                XElement cueElem = new XElement("Cue");

                // Replace empty characters with nothing.
                XElement cueNameElm      = new XElement("Name", cue.Name.Replace("\0", ""));
                XElement cueCategoryElem = new XElement("Category", cue.Category);
                XElement cueUnknown1Elem = new XElement("Unknown1", cue.UnknownSingle_1);
                XElement cueUnknown2Elem = new XElement("Unknown2", cue.UnknownSingle_2);
                XElement cueStreamElem   = new XElement("Stream", cue.Stream);

                // Add Nodes to appropriate XML Elements.
                cueElem.Add(cueNameElm, cueCategoryElem, cueUnknown1Elem, cueUnknown2Elem, cueStreamElem);
                rootElem.Add(cueElem);
            }

            // Save XML.
            XDocument xml = new XDocument(rootElem);
            xml.Save(filepath);
        }

        public void ImportXML(string filepath)
        {
            // Load XML and get Name value.
            XDocument xml = XDocument.Load(filepath);
            Name = xml.Root.Attribute("Name").Value.PadRight(64, '\0');

            // Loop through cue nodes.
            foreach (XElement cueElem in xml.Root.Elements("Cue"))
            {
                SoundCue cue = new SoundCue()
                {
                    Name            = cueElem.Element("Name").Value.PadRight(32, '\0'),
                    Category        = uint.Parse(cueElem.Element("Category").Value),
                    UnknownSingle_1 = float.Parse(cueElem.Element("Unknown1").Value),
                    UnknownSingle_2 = float.Parse(cueElem.Element("Unknown2").Value),
                };

                // Check if the stream actually has a value before setting it, otherwise, leave it as null.
                if (cueElem.Element("Stream").Value != string.Empty) 
                    cue.Stream = cueElem.Element("Stream").Value;
                
                // Add to list of Cues in this file.
                Cues.Add(cue);
            }
        }
    }
}
