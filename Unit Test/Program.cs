using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Unit_Test {
    class Program {
        [DllImport("CLibrary.dll")]
        private extern static IntPtr capture(double resolution);

        [DllImport("CLibrary.dll")]
        private extern static IntPtr xor(IntPtr hBitMap);

        [DllImport("CLibrary.dll")]
        private extern static void reset(IntPtr hBitMap);

        [DllImport("CLibrary.dll")]
        private extern static IntPtr cal(IntPtr hBitMap);

        static void Main(string[] args) {
            IntPtr p = capture(0.5);
            Bitmap bmp = Image.FromHbitmap(p);
            bmp.Save("D:\\a.bmp");
            //for (int i = 0; i < 1000; i++) {
            //    IntPtr p = capture();
            //    Bitmap bmp = Image.FromHbitmap(p);
            //    xor(p);
            //    bmp.Dispose();
            //    Console.WriteLine(i);
            //}
            //Console.ReadKey();
        }

        private static double lastReso = -1;
        public static byte[] Capture(double resolution) {
            if (lastReso == -1) {
                lastReso = resolution;
            }
            Bitmap bmp = Image.FromHbitmap(capture(1));
            if (resolution != 1) {
                bmp = new Bitmap(bmp, Convert.ToInt32(bmp.Width * resolution), Convert.ToInt32(bmp.Height * resolution));
            }
            if (lastReso != resolution) {
                reset(bmp.GetHbitmap());
                lastReso = resolution;
            } else {
                xor(bmp.GetHbitmap());
            }
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Bmp);
            bmp.Dispose();
            byte[] data = stream.GetBuffer();
            stream.Close();
            return data;
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
