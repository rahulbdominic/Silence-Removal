using System;
using System.IO;

namespace Silence_Removal
{
    class Program
    {
        static void Main(string[] args)
        {
            string path, format;
            int count = 0;
            Console.WriteLine("Enter folder path of files.");
            path = Console.ReadLine();
            Console.WriteLine("Enter format of audio files to scan for.");
            format = Console.ReadLine();
            DirectoryInfo dir = new DirectoryInfo(path);
            startCuttingSilence(dir, count, format);
            Console.ReadLine();
        }

        public static byte[] openFile(string filepath)
        {
            FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] bytearray = new byte[stream.Length];
            stream.Read(bytearray, 0, bytearray.Length);
            stream.Close();
            return bytearray;
        }

        public static bool writeFile(string path, byte[] arrData)
        {
            FileStream writeStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(writeStream);
            bw.Write(arrData);
            bw.Close();
            writeStream.Close();
            return true;
        }

        public static void startCuttingSilence(DirectoryInfo dir, int count, string format)
        {
            string filter = "*.wav";
            short lowRange = -11000, highRange = 11000;
            if (format == "mp3")
            {
                filter = "*.mp3";
                lowRange = -26000;
                highRange = 26000;
            }
            else if (format == "*.wav")
            {
                filter = "*.wav";
                lowRange = -11000;
                highRange = 11000;
            }

            foreach (var file in dir.GetFiles(filter))
            {
                count++;
                byte[] bytearray;
                int startpos, endpos;
                bytearray = openFile(file.FullName);
                startpos = 0;
                endpos = bytearray.Length - 1;
                for (int i = 0; i < bytearray.Length; i += 2)
                {
                    short mono = BitConverter.ToInt16(bytearray, i);

                    if (mono < highRange && mono > lowRange)
                        startpos = i;
                    else
                        break;
                }
                for (int k = bytearray.Length - 1; k >= 0; k -= 2)
                {
                    short mono = BitConverter.ToInt16(bytearray, k - 1);

                    if (mono < highRange && mono > lowRange)
                        endpos = k;
                    else
                        break;
                }
                byte[] newarr = new byte[(endpos - startpos) + 1];
                int z, n;
                z = 0;
                for (n = startpos; n <= endpos; n++)
                {
                    newarr[z] = bytearray[n];
                    z++;
                }
                if (writeFile(file.FullName + " (Changed)." + format, newarr) == true)
                    Console.WriteLine("Amended file {0}", count);
            }
        }
    }
}
