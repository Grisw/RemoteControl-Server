using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PC_Controller {
    class ImageCore {

        [DllImport("CLibrary.dll")]
        private extern static IntPtr capture(int reset);
        
        public static byte[] Capture(double resolution) {
            Bitmap bmp = Image.FromHbitmap(capture(0));
            if (resolution != 1) {
                bmp = new Bitmap(bmp, Convert.ToInt32(bmp.Width * resolution), Convert.ToInt32(bmp.Height * resolution));
            }
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Jpeg);
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
