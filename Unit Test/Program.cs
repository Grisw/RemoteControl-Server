using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Unit_Test {
    class Program {
        [DllImport("CLibrary.dll")]
        private extern static IntPtr capture(int reset);

        [DllImport("CLibrary.dll")]
        private extern static IntPtr cal(IntPtr hBitMap);

        [DllImport("CLibrary.dll")]
        private extern static IntPtr img(byte[] data,int size);

        static void Main(string[] args) {
            Image.FromHbitmap(cal(new Bitmap(@"D:\0.bmp").GetHbitmap()));
            Image.FromHbitmap(cal(new Bitmap(@"D:\1.bmp").GetHbitmap()));
            Console.ReadKey();
        }

        public static byte[] Compress(byte[] inputBytes) {
            using (MemoryStream outStream = new MemoryStream()) {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true)) {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }

        public static byte[] Bmp2Byte(Bitmap bmp) {
            double resolution = 1;
            Bitmap b;
            if (resolution == 1) {
                b = bmp;
            } else {
                b = new Bitmap(bmp, Convert.ToInt32(bmp.Width * resolution), Convert.ToInt32(bmp.Height * resolution));
            }
            MemoryStream stream = new MemoryStream();
            b.Save(stream, ImageFormat.Jpeg);
            bmp.Dispose();
            b.Dispose();
            byte[] data = stream.GetBuffer();
            stream.Close();
            return data;
        }
    }
}
