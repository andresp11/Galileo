using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Media;
using System.Xml;
using System.Net;

namespace OMR.helpers
{
    public class duplicateFile
    {
        public static string createDuplicate(string fileName, string ruta)
        {
//            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/") + "omrtemp"))
//                Directory.CreateDirectory("omrtemp");
            string tempName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            string dupFile = ruta + tempName + "_tmp"+ Path.GetExtension(fileName);
            try
            {
                File.Copy(ruta + fileName, dupFile, false);
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
            return dupFile;
        }
        public static void cleanUpFiles(string ruta)
        {
//            if (!Directory.Exists("omrtemp"))
//                Directory.CreateDirectory("omrtemp");
            string[] files = Directory.GetFiles(ruta,"*_tmp.*");
            //Quitar comentarios
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
    }
}
