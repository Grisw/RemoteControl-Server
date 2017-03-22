using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace PC_Controller {
    class ImageCore {

        [DllImport("CLibrary.dll")]
        private extern static IntPtr capture(double resolution);
        
        public static byte[] Capture(double resolution) {
            Bitmap bmp = Image.FromHbitmap(capture(resolution));
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
    }
}
