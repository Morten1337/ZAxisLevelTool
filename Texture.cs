using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZAxisLevelTool
{
    public class Texture
    {
        public XPRTexture __xpr;

		public string m__name;
		public string m__extenstion;

        public uint m__texture_id;
        public uint m__width;
        public uint m__height;
        public uint m__dxt;

        public byte[] m__data;

        public Texture()
        {
            m__name = "";
        }

        static public bool TextureNameExists(string name, List<Texture> textures)
        {
            int chunkCount = textures.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (textures[i].m__name == name)
                    return true;
            }

            return false;
        }

        static public bool TextureIdExists(uint id, List<Texture> textures)
        {
            int chunkCount = textures.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (textures[i].m__texture_id == id)
                    return true;
            }

            return false;
        }

        static public Texture GetObjectById(uint id, List<Texture> textures)
        {
            int chunkCount = textures.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (textures[i].m__texture_id == id)
                    return textures[i];
            }

            return null;
        }

        static public Texture GetObjectByName(string name, List<Texture> textures)
        {
            int chunkCount = textures.Count();

            for (int i = 0; i < chunkCount; i++)
            {
                if (textures[i].m__name == name)
                    return textures[i];
            }

            return null;
        }

        // dump dds textures to folder
        public static void dump_textures(string path, List<Texture> textures, ZAxisLevelTool.Form1.Game game)
        {
            // HACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // HACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // HACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            for (int i = 0; i < textures.Count(); i++)
            {
                //if (textures[i].m__dxt == 0)
                //    continue;
				using (BinaryWriter writer = new BinaryWriter(File.Create(Path.Combine(path, String.Format("{0}.{1}", textures[i].m__name, textures[i].m__extenstion)))))
                {
                    /*
                    if (game == Form1.Game.AGGRESSIVE_INLINE)
                    {
                        writer.Write(0x20534444); // "DDS "
                        writer.Write(0x0000007c);
                        writer.Write(0x00a01007);
                        writer.Write(textures[i].m__height);
                        writer.Write(textures[i].m__width);
                        writer.Write(1);
                        writer.Write(0x00000000);
                        writer.Write(0x00000007);
                        writer.Write(new byte[44]);
                        writer.Write(0x00000020);
                        writer.Write(0x00000004);

                        if (textures[i].m__dxt == 0)
                        {
                            writer.Write(0x31545844);
                        }
                        else if (textures[i].m__dxt == 1)
                        {
                            writer.Write(0x32545844);
                        }

                        writer.Write(new byte[20]);

                        writer.Write(0x00401008);

                        writer.Write(new byte[16]);
                    }
                    */

                    writer.Write(textures[i].m__data);

                    writer.Close();
                }
            }
        }

    }

    public enum XPRFormat : byte
    {
        ARGB = 6,
        DXT1 = 12,
        None = 0
    }
    public class XPRTexture
    {
        public XPRHeader Header;
        public byte[] ImageData;

        public XPRTexture()
        {
        }

        public XPRTexture(BinaryReader br)
        {
            Header = new XPRHeader(br);
            br.BaseStream.Seek(2020, SeekOrigin.Current);
            ImageData = br.ReadBytes((int)Header.FileSize - (int)Header.HeaderSize);
        }

        public XPRFormat Format
        {
            get
            {
                if (this.Header == null)
                {
                    return XPRFormat.None;
                }
                return (XPRFormat)this.Header.TextureFormat;
            }
        }

        public int Height
        {
            get
            {
                if (this.Header == null)
                {
                    return -1;
                }
                return (int)Math.Pow(2.0, (double)this.Header.TextureRes2);
            }
        }

        public bool IsValid
        {
            get
            {
                if (this.Header == null)
                {
                    return false;
                }
                return this.Header.IsValid;
            }
        }

        public int Width
        {
            get
            {
                if (this.Header == null)
                {
                    return -1;
                }
                return (int)Math.Pow(2.0, (double)this.Header.TextureRes2);
            }
        }
        public class XPRHeader
        {
            public uint FileSize;
            public uint HeaderSize;
            public bool IsValid;
            public uint MagicBytes;
            public uint TextureCommon;
            public uint TextureData;
            public byte TextureFormat;
            public uint TextureLock;
            public byte TextureMisc1;
            public byte TextureRes1;
            public byte TextureRes2;

            public XPRHeader()
            {
            }

            public XPRHeader(BinaryReader br)
            {
                this.MagicBytes = br.ReadUInt32();
                if (this.MagicBytes == 0x30525058)
                {
                    FileSize = br.ReadUInt32();
                    HeaderSize = br.ReadUInt32();
                    TextureCommon = br.ReadUInt32();
                    TextureData = br.ReadUInt32();
                    TextureLock = br.ReadUInt32();
                    TextureMisc1 = br.ReadByte();
                    TextureFormat = br.ReadByte();
                    TextureRes1 = br.ReadByte();
                    TextureRes2 = br.ReadByte();
                    IsValid = true;
                }
            }
        }
    }
}
