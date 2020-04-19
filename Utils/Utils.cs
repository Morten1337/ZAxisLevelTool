using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace ZAxisLevelTool
{
    class Utils
    {
        public static byte[] GetStringBytes(string _in)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(_in);
            return bytes;
        }

        public static bool IsString(IEnumerable<byte> chars)
        {
            bool isString;
            foreach (var c in chars)
            {
                isString = (c >= 'A' && c <= 'Z');

                if (!isString)
                    return false;
            }
            return true;
        }

        public static string GetStringFromByteArray(byte[] array)
        {
            string returnString = "";
            foreach (byte element in array)
            {
                if (element == 0)
                    break;

                returnString += (char)element;
            }
            return returnString;
        }

        public static string GetUniqueName(string name, List<string> objects)
        {
            string unique_name = "";
            int _newNameIndex = 0;

            if (!objects.Contains(name))
            {
                unique_name = name;
            }
            else
            {
                while (objects.Contains(String.Format("{0}_{1}", name, _newNameIndex.ToString("D2"))))
                {
                    _newNameIndex++;
                }

                unique_name = String.Format("{0}_{1}", name, _newNameIndex.ToString("D2"));
            }

            return unique_name;
        }
        static public bool is_flag_set(uint src, uint flag)
        {
            if ((src & flag) != 0)
                return true;
            else
                return false;
        }

        static public bool is_flag_set(List<uint> src_list, uint flag)
        {
            foreach (uint src in src_list)
            {
                if ((src & flag) != 0)
                    return true;
            }
            return false;
        }

        static public long seek_relative(BinaryReader br, long offset, long origin_offset)
        {
            return br.BaseStream.Seek(origin_offset + offset, SeekOrigin.Begin);
        }

        static public long seek(BinaryReader br, long offset, SeekOrigin origin)
        {
            return br.BaseStream.Seek(offset, origin);
        }

        static public float readfloat(BinaryReader br)
        {
            return br.ReadSingle();
        }

        static public uint readuint(BinaryReader br)
        {
            return br.ReadUInt32();
        }

        static public ushort readushort(BinaryReader br)
        {
            return br.ReadUInt16();
        }

        static public byte readbyte(BinaryReader br)
        {
            return br.ReadByte();
        }

        static public bool readbool(BinaryReader br)
        {
            return br.ReadBoolean();
        }

        static public bool pad_16(BinaryReader br)
        {
            try
            {
                long rem = br.BaseStream.Position % 16;
                if (rem != 0)
                {
                    for (int i = 0; i < (16 - rem); i++)
                        seek(br, 1, SeekOrigin.Current);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!");
                return false;
            }
        }

        static public bool pad_4(BinaryReader br)
        {
            try
            {
                long rem = br.BaseStream.Position % 4;
                if (rem != 0)
                {
                    for (int i = 0; i < (4 - rem); i++)
                    {
                        seek(br, 1, SeekOrigin.Current);
                        //Console.WriteLine("+1 {0}", br.BaseStream.Position);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!");
                return false;
            }
        }

        static public bool pad(BinaryReader br, int _pad)
        {
            try
            {
                long rem = br.BaseStream.Position % _pad;
                if (rem != 0)
                {
                    for (int i = 0; i < (_pad - rem); i++)
                    {
                        seek(br, 1, SeekOrigin.Current);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!");
                return false;
            }
        }

        static public void read_and_print_vector(BinaryReader br, bool seekBack = false)
        {
            double x = readfloat(br);
            double y = readfloat(br);
            double z = readfloat(br);

            Console.WriteLine("{0} {1} {2}",
                x.ToString("f4", CultureInfo.InvariantCulture),
                y.ToString("f4", CultureInfo.InvariantCulture),
                z.ToString("f4", CultureInfo.InvariantCulture));

            if (seekBack)
            {
                seek(br, -12, SeekOrigin.Current);
            }
        }
    }
}
