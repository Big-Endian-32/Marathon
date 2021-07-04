﻿// ExplosionPackage.cs is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 Knuxfan24
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

namespace Marathon.IO.Formats.Miscellaneous
{
    /// <summary>
    /// <para>File base for a BIN format.</para>
    /// <para>Used in SONIC THE HEDGEHOG for explosion properties.</para>
    /// </summary>
    public class ExplosionPackage : FileBase
    {
        // TODO: Understand the unknowns, clean up code.

        public ExplosionPackage() { }

        public ExplosionPackage(string file)
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

        public class Explosion
        {
            public string EntryName;

            public uint UnknownUInt32_1; // Setting this to 1 allowed a BombBox explosion to take enemies out, 0 & 2 didn't.

            public float Radius,         // The radius that this explosion affects
                         UnknownFloat_1, // Nearly always the same as Radius, barring a few exceptions.
                         UnknownFloat_2,
                         UnknownFloat_3,
                         UnknownFloat_4,
                         Force;          // Not too sure, but increasing this value seemed to affect something to do with how the explosion affects other physics objects?

            public uint Damage,          // How much damage this explosion causes.
                        Behaviour;       // How things should react to this explosion? Has a lot of different values, 46 made every explosion stun enemies like a FlashBox and be unable to damage the player.

            public string ParticleFile,
                          ParticleName,
                          SceneBank,
                          SoundName,
                          LightName;
        }

        public List<Explosion> Entries = new List<Explosion>();

        public override void Load(Stream stream)
        {
            BINAReader reader = new BINAReader(stream);
            reader.ReadHeader();

            // Store first offset after the header.
            long startPosition = reader.BaseStream.Position;

            // Store the offset to the first string entry (repurposed as offset table length).
            uint offsetTableLength = reader.ReadUInt32();

            // Jump back to the first offset so we can iterate through the loop.
            reader.JumpTo(startPosition);

            while (reader.BaseStream.Position < offsetTableLength)
            {
                Explosion explosion = new Explosion();

                uint nameOffset = reader.ReadUInt32();
                explosion.UnknownUInt32_1 = reader.ReadUInt32();
                explosion.Radius = reader.ReadSingle();
                explosion.UnknownFloat_1 = reader.ReadSingle();
                explosion.UnknownFloat_2 = reader.ReadSingle();
                explosion.UnknownFloat_3 = reader.ReadSingle();
                explosion.UnknownFloat_4 = reader.ReadSingle();
                explosion.Force = reader.ReadSingle();
                explosion.Damage = reader.ReadUInt32();
                explosion.Behaviour = reader.ReadUInt32();
                uint particleFileOffset = reader.ReadUInt32();
                uint particleNameOffset = reader.ReadUInt32();
                uint SceneBankOffset = reader.ReadUInt32();
                uint soundNameOffset = reader.ReadUInt32();
                uint lightNameOffset = reader.ReadUInt32();

                reader.JumpAhead(12); //Padding? All nulls in official file. Might be worth experimenting with to see if anything changes.
                long position = reader.BaseStream.Position;

                // Read all the string values.
                reader.JumpTo(nameOffset, true);
                explosion.EntryName = reader.ReadNullTerminatedString();

                reader.JumpTo(particleFileOffset, true);
                explosion.ParticleFile = reader.ReadNullTerminatedString();

                reader.JumpTo(particleNameOffset, true);
                explosion.ParticleName = reader.ReadNullTerminatedString();

                reader.JumpTo(SceneBankOffset, true);
                explosion.SceneBank = reader.ReadNullTerminatedString();

                reader.JumpTo(soundNameOffset, true);
                explosion.SoundName = reader.ReadNullTerminatedString();

                reader.JumpTo(lightNameOffset, true);
                explosion.LightName = reader.ReadNullTerminatedString();

                reader.JumpTo(position);

                Entries.Add(explosion);
            }
        }

        public override void Save(Stream fileStream)
        {
            BINAv1Header Header = new BINAv1Header();
            BINAWriter writer = new BINAWriter(fileStream, Header);

            // Write the explosion entries.
            for (int i = 0; i < Entries.Count; i++)
            {
                writer.AddString($"entry{i}Name", Entries[i].EntryName);
                writer.Write(Entries[i].UnknownUInt32_1);
                writer.Write(Entries[i].Radius);
                writer.Write(Entries[i].UnknownFloat_1);
                writer.Write(Entries[i].UnknownFloat_2);
                writer.Write(Entries[i].UnknownFloat_3);
                writer.Write(Entries[i].UnknownFloat_4);
                writer.Write(Entries[i].Force);
                writer.Write(Entries[i].Damage);
                writer.Write(Entries[i].Behaviour);
                writer.AddString($"entry{i}ParticleFile", Entries[i].ParticleFile);
                writer.AddString($"entry{i}ParticleName", Entries[i].ParticleName);
                writer.AddString($"entry{i}SceneBank", Entries[i].SceneBank);
                writer.AddString($"entry{i}SoundName", Entries[i].SoundName);
                writer.AddString($"entry{i}LightName", Entries[i].LightName);
                writer.WriteNulls(12);
            }

            // Write the footer.
            writer.WriteNulls(4);
            writer.FinishWrite(Header);
        }

        public void ExportXML(string filepath)
        {
            // Root element.
            XElement rootElem = new XElement("Explosion");

            // Explosion elements.
            foreach (Explosion explosion in Entries)
            {
                XElement explosionElem = new XElement("Explosion");
                XAttribute NameAttr = new XAttribute("ObjectName", explosion.EntryName);
                XElement UInt1Elem = new XElement("UnknownUInt32_1", explosion.UnknownUInt32_1);
                XElement Float1Elem = new XElement("Radius", explosion.Radius);
                XElement Float2Elem = new XElement("UnknownFloat_1", explosion.UnknownFloat_1);
                XElement Float3Elem = new XElement("UnknownFloat_2", explosion.UnknownFloat_2);
                XElement Float4Elem = new XElement("UnknownFloat_3", explosion.UnknownFloat_3);
                XElement Float5Elem = new XElement("UnknownFloat_4", explosion.UnknownFloat_4);
                XElement Float6Elem = new XElement("Force", explosion.Force);
                XElement UInt2Elem = new XElement("Damage", explosion.Damage);
                XElement UInt3Elem = new XElement("Behaviour", explosion.Behaviour);

                XAttribute ParticleBankAttr = new XAttribute("ParticleBank", explosion.ParticleFile);
                XElement ParticleElem = new XElement("Particle", explosion.ParticleName);
                ParticleElem.Add(ParticleBankAttr);

                XAttribute SceneBankAttr = new XAttribute("SceneBank", explosion.SceneBank);
                XElement SoundElem = new XElement("Sound", explosion.SoundName);
                SoundElem.Add(SceneBankAttr);

                XElement LightElem = new XElement("Light", explosion.LightName);

                explosionElem.Add
                (
                    NameAttr, UInt1Elem, Float1Elem, Float2Elem, Float3Elem,
                    Float4Elem, Float5Elem, Float6Elem, UInt2Elem, UInt3Elem,
                    ParticleElem, SoundElem, LightElem
                );
                
                rootElem.Add(explosionElem);
            }

            // Save XML.
            XDocument xml = new XDocument(rootElem);
            xml.Save(filepath);
        }

        public void ImportXML(string filepath)
        {
            // Load XML.
            XDocument xml = XDocument.Load(filepath);

            // Loop through explosion nodes.
            foreach (XElement explosionElem in xml.Root.Elements("Explosion"))
            {
                // Read explosion values.
                Explosion entry = new Explosion
                {
                    EntryName = explosionElem.Attribute("ObjectName").Value,
                    UnknownUInt32_1 = uint.Parse(explosionElem.Element("UnknownUInt32_1").Value),
                    Radius = float.Parse(explosionElem.Element("Radius").Value),
                    UnknownFloat_1 = float.Parse(explosionElem.Element("UnknownFloat_1").Value),
                    UnknownFloat_2 = float.Parse(explosionElem.Element("UnknownFloat_2").Value),
                    UnknownFloat_3 = float.Parse(explosionElem.Element("UnknownFloat_3").Value),
                    UnknownFloat_4 = float.Parse(explosionElem.Element("UnknownFloat_4").Value),
                    Force = float.Parse(explosionElem.Element("Force").Value),
                    Damage = uint.Parse(explosionElem.Element("Damage").Value),
                    Behaviour = uint.Parse(explosionElem.Element("Behaviour").Value),
                    ParticleFile = explosionElem.Element("Particle").Attribute("ParticleBank").Value,
                    ParticleName = explosionElem.Element("Particle").Value,
                    SceneBank = explosionElem.Element("Sound").Attribute("SceneBank").Value,
                    SoundName = explosionElem.Element("Sound").Value,
                    LightName = explosionElem.Element("Light").Value
                };

                // Add object to Entries list.
                Entries.Add(entry);
            }
        }
    }
}
