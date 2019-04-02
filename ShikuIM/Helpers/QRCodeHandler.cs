using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using ThoughtWorks.QRCode.Codec;

namespace ShikuIM
{
    public static class QRCodeHandler
    {
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="QRString">二维码字符串</param>
        /// <param name="QRCodeEncodeMode">二维码编码(Byte、AlphaNumeric、Numeric)</param>
        /// <param name="QRCodeScale">二维码尺寸(Version为0时，1：26x26，每加1宽和高各加25</param>
        /// <param name="QRCodeVersion">二维码密集度0-40</param>
        /// <param name="QRCodeErrorCorrect">二维码纠错能力(L：7% M：15% Q：25% H：30%)</param>
        /// <param name="hasLogo">是否有logo(logo尺寸50x50，QRCodeScale>=5，QRCodeErrorCorrect为H级)</param>
        /// <param name="logoFilePath">logo路径</param>
        /// <returns></returns>
        public static BitmapSource CreateQRCode(string QRString, string QRCodeEncodeMode, short QRCodeScale, int QRCodeVersion, string QRCodeErrorCorrect, bool hasLogo, string logoFilePath)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            switch (QRCodeEncodeMode)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = QRCodeScale;
            qrCodeEncoder.QRCodeVersion = QRCodeVersion;

            switch (QRCodeErrorCorrect)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                case "H":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }

            try
            {
                System.Drawing.Image image = qrCodeEncoder.Encode(QRString, System.Text.Encoding.UTF8);
                if (hasLogo)
                {
                    System.Drawing.Image copyImage = System.Drawing.Image.FromFile(logoFilePath);
                    Graphics g = Graphics.FromImage(image);
                    int x = image.Width / 2 - copyImage.Width / 2;
                    int y = image.Height / 2 - copyImage.Height / 2;
                    g.DrawImage(copyImage, new Rectangle(x, y, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                    g.Dispose();
                    copyImage.Dispose();
                }
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image);

                image.Dispose();
                return Helpers.ConvertBitmapToBitmapSource(bmp);
            }
            catch (Exception ex)
            {
                ConsoleLog.Output("QRCode Error:"+ex.Message);
                LogHelper.log.Info("QRCode Error:" + ex.Message);
                return null;
            }
            
        }
    }
}
