using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZAxisLevelTool
{
    class NodeTree
    {
        public enum ChunkType
        {
            FORM = 0x4D524F46, // file root
            TGEO = 0x4F454754, // geometry object
            TGHD = 0x44484754, // geometry object header
            TGTD = 0x44544754, // 
            TGPT = 0x54504754, // geometry vertex data
            TGPH = 0x48504754,
            TGVP = 0x50564754, // vertex position array
            TGVN = 0x4E564754, // vertex normals array
            TGVC = 0x43564754, // vertex color?
            TGVU = 0x55564754,
            TIVU = 0x55564954,
            TMTB = 0x42544D54,
            MATI = 0x4954414D, // material?
            TGLO = 0x4F4C4754,
            TGCI = 0x49434754,
            TGFE = 0x45464754,
            TGFM = 0x4D464754,
            TGFN = 0x4E464754,
            TGFP = 0x50464754, // triangle array
            TGFR = 0x52464754,
            TGFT = 0x54464754,
            TXRC = 0x43525854,
            TXRH = 0x48525854, // texture resource header?
            TDDS = 0x53444454, // DDS image
            TXPR = 0x52505854, // XPR image
            TBMP = 0x504D4254, // BMP image
            TZMP = 0x504D5A54, // ZMP image?
            TTGA = 0x41475454, // TGA image
            TGIH = 0x48494754, // geometry instance header?
            TGIN = 0x4E494754, // geometry instance?
            TGIT = 0x54494754,
            TIDA = 0x41444954,
            TIVN = 0x4E564954,
            TIFN = 0x4E464954,
            TIVC = 0x43564954,
            TII8 = 0x38494954,
            TII6 = 0x36494954,
            DIDF = 0x46444944,
            GRID = 0x44495247,
            DLEN = 0x4E454C44,
            OFST = 0x5453464F,
            CLEN = 0x4E454C43,
            NAME = 0x454D414E,
            ZFSD = 0x4453465A,
            SDPT = 0x54504453,
            SDTD = 0x44544453,
            ZPVS = 0x5356505A,
            PVSH = 0x48535650,
            PVSO = 0x4F535650,
            ZDYN = 0x4E59445A,
            DDEF = 0x46454444,
            DBDY = 0x59444244,
            DCON = 0x4E4F4344,
            DBNC = 0x434E4244,
            MOBO = 0x4F424F4D,
            ANIM = 0x4D494E41,
            DEFI = 0x49464544,
            AROT = 0x544F5241,
            ATRA = 0x41525441,
            ATRW = 0x57525441,
            ASCA = 0x41435341,
            ACBH = 0x48424341,
            ACBD = 0x44424341,
            ACB = 0x49424341,
            ZFFD = 0x4446465A,
            ZPEC = 0x4345505A,
            ZPEO = 0x4F45505A,
            ZPED = 0x4445505A,
            WSMC = 0x434D5357,
            WSMO = 0x4F4D5357,
            SDRD = 0x44524453,
            ZLIT = 0x54494C5A,
            LITH = 0x4854494C,
            LITO = 0x4F54494C,

            // BMXXX
            ZLZO = 0x4F5A4C5A,
            ZSCD = 0x4443535A,
            ZSCR = 0x5243535A,
            ZFPH = 0x4850465A,
            ZFPD = 0x4450465A,
            ZFPT = 0x5450465A,
        };

        public class NodeChunk
        {
            public ChunkType chunkType;
            public uint chunkSize;
            public byte[] data;
            public List<NodeChunk> chunks;

            public NodeChunk()
            {
                chunks = new List<NodeChunk>();
            }

            public bool isNull()
            {
                if (chunkSize > 0)
                    return false;
                else
                    return true;
            }

            public bool HasChunk(ChunkType chunkType)
            {
                int chunkCount = chunks.Count();

                for (int i = 0; i < chunkCount; i++)
                {
                    if (chunks[i].chunkType == chunkType)
                        return true;
                }

                return false;
            }

            public NodeChunk GetChunk(ChunkType chunkType)
            {
                int chunkCount = chunks.Count();

                for (int i = 0; i < chunkCount; i++)
                {
                    if (chunks[i].chunkType == chunkType)
                        return chunks[i];
                }

                return null;
            }

            public List<NodeChunk> GetChunks(ChunkType chunkType)
            {
                List<NodeChunk> returnNodes = new List<NodeChunk>();

                int chunkCount = chunks.Count();

                for (int i = 0; i < chunkCount; i++)
                {
                    if (chunks[i].chunkType == chunkType)
                        returnNodes.Add(chunks[i]);
                }

                return returnNodes;
            }

        }

        static public bool isKnownChunkType(ChunkType chunkType)
        {
            switch (chunkType)
            {
                case ChunkType.FORM:
                case ChunkType.TGEO:
                case ChunkType.TGHD:
                case ChunkType.TGTD:
                case ChunkType.TGPT:
                case ChunkType.TGPH:
                case ChunkType.TGVP:
                case ChunkType.TGVN:
                case ChunkType.TGVC:
                case ChunkType.TGVU:
                case ChunkType.TMTB:
                case ChunkType.MATI:
                case ChunkType.TGLO:
                case ChunkType.TGCI:
                case ChunkType.TGFE:
                case ChunkType.TGFM:
                case ChunkType.TGFN:
                case ChunkType.TGFP:
                case ChunkType.TGFR:
                case ChunkType.TGFT:
                case ChunkType.TXRC:
                case ChunkType.TXRH:
                case ChunkType.TDDS:
                case ChunkType.TXPR:
                case ChunkType.TBMP:
                case ChunkType.TZMP:
                case ChunkType.TTGA:
                case ChunkType.TGIH:
                case ChunkType.TGIN:
                case ChunkType.TGIT:
                case ChunkType.TIVU:
                case ChunkType.TIDA:
                case ChunkType.TIVN:
                case ChunkType.TIFN:
                case ChunkType.TIVC:
                case ChunkType.TII8:
                case ChunkType.TII6:
                case ChunkType.DIDF:
                case ChunkType.GRID:
                case ChunkType.DLEN:
                case ChunkType.OFST:
                case ChunkType.CLEN:
                case ChunkType.NAME:
                case ChunkType.ZFSD:
                case ChunkType.SDPT:
                case ChunkType.SDTD:
                case ChunkType.ZPVS:
                case ChunkType.PVSH:
                case ChunkType.PVSO:
                case ChunkType.ZDYN:
                case ChunkType.DDEF:
                case ChunkType.DBDY:
                case ChunkType.DCON:
                case ChunkType.DBNC:
                case ChunkType.MOBO:
                case ChunkType.ANIM:
                case ChunkType.DEFI:
                case ChunkType.AROT:
                case ChunkType.ATRA:
                case ChunkType.ATRW:
                case ChunkType.ASCA:
                case ChunkType.ACBH:
                case ChunkType.ACBD:
                case ChunkType.ACB:
                case ChunkType.ZFFD:
                case ChunkType.ZPEC:
                case ChunkType.ZPEO:
                case ChunkType.ZPED:
                case ChunkType.WSMC:
                case ChunkType.WSMO:
                case ChunkType.SDRD:

                case ChunkType.ZLIT:
                case ChunkType.LITH:
                case ChunkType.LITO:

                case ChunkType.ZLZO:
                case ChunkType.ZSCD:
                case ChunkType.ZSCR:
                case ChunkType.ZFPH:
                case ChunkType.ZFPD:
                case ChunkType.ZFPT:
                    return true;
                default:
                    return false;
            }
        }

        static public NodeChunk ReadChunk(BinaryReader br, NodeChunk root, bool from_root)
        {
            NodeChunk gNode = new NodeChunk();
            gNode.chunkType = (ChunkType)br.ReadUInt32();

            //if (gNode.chunkType == ChunkType.TXRC)
            //    Console.WriteLine("{1} -> TXRC @{0}", br.BaseStream.Position - 4, root.chunkType);

            //if (gNode.chunkType == ChunkType.TXRH)
            //    Console.WriteLine("{1} -> TXRH @{0}", br.BaseStream.Position - 4, root.chunkType);

            if (isKnownChunkType(gNode.chunkType))
            {
                gNode.chunkSize = br.ReadUInt32();
                long chunkStartOffset = br.BaseStream.Position;
                long chunkEndOffset;

                gNode.data = br.ReadBytes((int)gNode.chunkSize);
                chunkEndOffset = br.BaseStream;
                br.BaseStream.Seek(chunkStartOffset, SeekOrigin.Begin);

                while (br.BaseStream.Position < chunkEndOffset)
                {
                    if (br.BaseStream.Position + 4 > br.BaseStream.Length)
                        break;

                    NodeChunk gChild = ReadChunk(br, gNode, false);

                    if (gChild != null)
                    {
                        gNode.chunks.Add(gChild);
                    }
                    else
                    {
                        br.BaseStream.Seek(chunkEndOffset, SeekOrigin.Begin);
                    }
                }
            }
            else
            {
                if (Utils.IsString(BitConverter.GetBytes((uint)gNode.chunkType)))
                {
                    Console.WriteLine("Unknown ChunkType '{0}' 0x{1} @{2}", System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes((uint)gNode.chunkType)), gNode.chunkType, br.BaseStream.Position);
                }
                return null;
            }

            return gNode;

        }

        static public NodeChunk ReadRoot(BinaryReader br)
        {
            NodeChunk gRoot = new NodeChunk();
            gRoot.chunkType = (ChunkType)br.ReadUInt32();
            gRoot.chunkSize = br.ReadUInt32();

            long chunkStartOffset = br.BaseStream.Position;

            gRoot.data = br.ReadBytes((int)gRoot.chunkSize);
            br.BaseStream.Seek(chunkStartOffset, SeekOrigin.Begin);

            while (br.BaseStream.Position < chunkStartOffset + gRoot.chunkSize)
            {
                if (br.BaseStream.Position + 4 > br.BaseStream.Length)
                    break;

                NodeChunk gChild = ReadChunk(br, gRoot, true);

                if (gChild != null)
                {
                    gRoot.chunks.Add(gChild);
                }

            }

            return gRoot;
        }
    }
}
