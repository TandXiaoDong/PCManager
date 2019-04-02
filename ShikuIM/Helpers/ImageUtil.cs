using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace ShikuIM
{
    public static class ImageUtil
    {
        #region bitmap转bitmapImage
        /// <summary>
        /// 传入bitmap对象
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
        #endregion

        #region 将bitmapImage对象转换成Bitmap对象
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return bitmap;
            }
        }
        #endregion

        #region byte 转bitmapImage
        /// <summary>
        /// byte[]转为BitmapImage
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static BitmapImage ToImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }
        #endregion

        #region bitmapImage转byte
        /// <summary>
        /// BitmapImage转为byte[]
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(BitmapImage bmp)
        {
            byte[] ByteArray = null;

            try
            {
                Stream stream = bmp.StreamSource;
                if (stream != null && stream.Length > 0)
                {
                    stream.Position = 0;
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        ByteArray = br.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch
            {
                return null;
            }

            return ByteArray;
        }
        #endregion

    }
}
