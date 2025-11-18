using Net.Codecrete.QrCodeGenerator;
using OtpNet;
using Svg;
using System;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace DotNetGuiApp
{
    public partial class Form1 : Form
    {
        private byte[] _secretKey;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            _secretKey = KeyGeneration.GenerateRandomKey(20);
            var secretKeyBase32 = Base32Encoding.ToString(_secretKey);

            var issuer = "YourAppName";
            var accountName = "user@example.com";

            var totpUri = $"otpauth://totp/{HttpUtility.UrlEncode(issuer)}:{HttpUtility.UrlEncode(accountName)}" +
                          $"?secret={secretKeyBase32}" +
                          $"&issuer={HttpUtility.UrlEncode(issuer)}" +
                          "&algorithm=SHA1" + // Or SHA256, SHA512 if using those modes with Otp.NET
                          "&digits=6" + // Or 8, depending on your setup
                          "&period=30"; // Or other period, depending on your setup

            var svgQrCode = QrCode.EncodeText(totpUri, QrCode.Ecc.Medium).ToSvgString(2);

            qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(svgQrCode);

            var svgDoc = SvgDocument.Open(xmlDocument);

            var svgBitmap = new Bitmap(300, 300);
            svgDoc.Draw(svgBitmap);

            qrPictureBox.Image = svgBitmap;
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            var code = totpTextBox.Text;

            var totp = new Totp(_secretKey);

            if (totp.VerifyTotp(DateTime.UtcNow, code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                resultLabel.Text = $"Valid (TimeStepMatched = {timeStepMatched})";
            }
            else
            {
                resultLabel.Text = "Invalid.";
            }
        }
    }
}
