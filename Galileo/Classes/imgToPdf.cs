using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using PdfSharp.Drawing;
//using PdfSharp.Pdf;

using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace OMR.helpers
{
    public class imgToPdf
    {
        public static void toPDF(string src, string destinaton, double widthCM, double heightCM)
        {
            try
            {
/*                PdfDocument doc = new PdfDocument();
                doc.Pages.Add(new PdfPage());
                doc.Pages[0].Width = new XUnit(widthCM, XGraphicsUnit.Centimeter);
                doc.Pages[0].Height = new XUnit(heightCM, XGraphicsUnit.Centimeter);
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);

                XImage img = XImage.FromFile(src);
                xgr.DrawImage(img,
                    new Rectangle(0, 0, (int)doc.Pages[0].Width.Point, (int)doc.Pages[0].Height.Point),
                    new Rectangle(0, 0, (int)img.Width, (int)img.Height), XGraphicsUnit.Point); 
                doc.Save(destinaton);
                doc.Close();*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
