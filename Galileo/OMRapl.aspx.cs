using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Media;
using System.Xml;
using System.Net.Mail;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using AForge.Math.Geometry;
using OMR;
using OMR.Enums;
using OMR.helpers;

namespace Galileo
{
    public partial class OMRapl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void ImBEjemplo_Click(object sender, ImageClickEventArgs e)
        {
        }

        protected void Descarga_Click(object sender, ImageClickEventArgs e)
        {
            string fileName = ".\\descargar\\Completa Image.jpg";

            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "aplicattion/jpg";
            Response.AddHeader("Content-Disposition", "inline;filename=encuesta.jpg");
            HttpContext.Current.Response.TransmitFile(fileName);
            Response.End();
        }

    }
}