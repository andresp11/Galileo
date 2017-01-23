using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data;
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
    public partial class Registro : System.Web.UI.Page
    {
        static string DC = System.Configuration.ConfigurationManager.ConnectionStrings["sqlconn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        OMR.OMREngine engine;
        List<System.Drawing.Image> inDepthImages = new List<System.Drawing.Image>();
        List<string> indepthMessages = new List<string>();
        public volatile int asProgress = 0, inDepthListCrsr = -1;
        public volatile string asMessage = "", asMessage2;
        public volatile System.Drawing.Image asImage1, asImage2;

        protected void imgAgregar_Click(object sender, ImageClickEventArgs e)
        {
            int ID = 0;
            Label1.Text = "";
            if (FUpEncuesta.HasFile && txtCorreo.Text.Length > 0)
            {

                String fileExtension = System.IO.Path.GetExtension(FUpEncuesta.FileName).ToLower();
                String allowedExtensions = ".jpg";
                if (fileExtension == allowedExtensions)
                {

                    string filename = System.IO.Path.GetFileName(FUpEncuesta.FileName);
                    FUpEncuesta.SaveAs(Server.MapPath("~/omrtemp/") + filename);
                    calcula(Server.MapPath("~/omrtemp/") + filename, Server.MapPath("~/omrtemp/"));
                    //                Label1.Text = "Archivo correcto uploaded!";
            
                    int pregunta = 0;
                    SqlConnection sqlConn = new SqlConnection(DC);
                    SqlDataAdapter sqlID = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    sqlID.SelectCommand = new SqlCommand();
                    sqlID.SelectCommand.Connection = sqlConn;
                    sqlID.SelectCommand.CommandText = "GetIdWeb";
                    sqlID.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlID.SelectCommand.CommandTimeout = 45;
                    try
                    {
                        sqlConn.Open();
                        sqlID.Fill(ds, "Ejemploweb");
                        ID = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        SqlCommand sqlCommRespuesta = new SqlCommand();
                        sqlCommRespuesta.Connection = sqlConn;
                        sqlCommRespuesta.CommandText = "ActualizaRespuesta";
                        sqlCommRespuesta.CommandType = CommandType.StoredProcedure;
                        sqlCommRespuesta.CommandTimeout = 45;
                        sqlCommRespuesta.Parameters.Add("@EncuestaId", System.Data.SqlDbType.Int);
                        sqlCommRespuesta.Parameters.Add("@EncuestadoId", System.Data.SqlDbType.Int);
                        sqlCommRespuesta.Parameters.Add("@Preguntaid", System.Data.SqlDbType.Int);
                        sqlCommRespuesta.Parameters.Add("@Respuesta", System.Data.SqlDbType.NVarChar);

                        for (int i = 0; i < engine.NumberBlocks.Count; i++)
                        {
                            pregunta = engine.NumberBlocks[i].StartOfInd;
                            sqlCommRespuesta.Parameters["@Encuestaid"].Value = 4;
                            sqlCommRespuesta.Parameters["@Encuestadoid"].Value = ID;
                            sqlCommRespuesta.Parameters["@Preguntaid"].Value = pregunta;
                            sqlCommRespuesta.Parameters["@Respuesta"].Value = engine.NumberBlocks[i].MarkedValue.ToString();
                            sqlCommRespuesta.ExecuteNonQuery();
                        }
                        for (int i = 0; i < engine.AnswerBlocks.Count; i++)
                        {
                            for (int ii = 0; ii < engine.AnswerBlocks[i].NumberOfLines; ii++)
                            {
                                pregunta = engine.AnswerBlocks[i].StartOfInd;
                                string str = "";
                                for (int j = 0; j < engine.AnswerBlocks[i].Options; j++)
                                    if (engine.AnswerBlocks[i].BinaryMaskedOMs[ii, j])
                                        str += (str.Length > 0 ? ", " + (j + 1).ToString() : (j + 1).ToString()) ;

                            sqlCommRespuesta.Parameters["@Encuestaid"].Value = 4;
                            sqlCommRespuesta.Parameters["@Encuestadoid"].Value = ID;
                            sqlCommRespuesta.Parameters["@Preguntaid"].Value = pregunta;
                            sqlCommRespuesta.Parameters["@Respuesta"].Value = str;
                            sqlCommRespuesta.ExecuteNonQuery();

                            }
                        }
                        SqlCommand sqlCommValida = new SqlCommand("ValidaRespuesta", sqlConn);
                        sqlCommValida.CommandType = CommandType.StoredProcedure;
                        sqlCommValida.CommandTimeout = 45;
                        sqlCommValida.Parameters.Add("@EncuestaId", System.Data.SqlDbType.Int);
                        sqlCommValida.Parameters.Add("@EncuestadoId", System.Data.SqlDbType.Int);
                        sqlCommValida.Parameters.Add("@SheetID", System.Data.SqlDbType.Int);
                        sqlCommValida.Parameters.Add("@Inicial", System.Data.SqlDbType.Int);
                        sqlCommValida.Parameters.Add("@Preguntas", System.Data.SqlDbType.NVarChar);
                        sqlCommValida.Parameters["@EncuestaId"].Value = 4;
                        sqlCommValida.Parameters["@EncuestadoId"].Value = ID;
                        sqlCommValida.Parameters["@SheetID"].Value = 38;
                        sqlCommValida.Parameters["@Inicial"].Value = 1;
                        sqlCommValida.Parameters["@Preguntas"].Value = 9;
                        sqlCommValida.ExecuteNonQuery();

                        SqlCommand sqlCommNombre = new SqlCommand("UpdateWEB ", sqlConn);
                        sqlCommNombre.CommandType = CommandType.StoredProcedure;
                        sqlCommNombre.CommandTimeout = 45;
                        sqlCommNombre.Parameters.Add("@EncuestadoKey", System.Data.SqlDbType.Int);
                        sqlCommNombre.Parameters.Add("@descripcion", System.Data.SqlDbType.NVarChar);
                        sqlCommNombre.Parameters["@EncuestadoKey"].Value = ID;
                        sqlCommNombre.Parameters["@descripcion"].Value = txtNombre.Text;
                        sqlCommNombre.ExecuteNonQuery();

                HttpPostedFile file = FUpEncuesta.PostedFile;
                String strFileNamePath = Server.MapPath("~/omrtemp/") + FUpEncuesta.FileName;
                String strSql = " Update Imagen Set Foto = @Imagen";
                
                strSql += " From Imagen Where EncuestaId = @EncuestaId And EncuestadoId = @EncuestadoId And SheetId = @SheetId";
                FileStream fs = new FileStream(strFileNamePath,FileMode.Open);
                
                int FileLen = file.ContentLength;
                if (FileLen > 0)
                {
                    byte[] imagen = new byte[file.InputStream.Length];
                    file.InputStream.Read(imagen,0,imagen.Length);
                        SqlCommand sqlComGraba = new SqlCommand(strSql, sqlConn);
                        sqlComGraba.CommandTimeout = 45;
                        sqlComGraba.Parameters.Add("@Imagen", System.Data.SqlDbType.Image);
                    sqlComGraba.Parameters.Add("@EncuestaId", System.Data.SqlDbType.Int);
                    sqlComGraba.Parameters.Add("@EncuestadoId", System.Data.SqlDbType.Int);
                    sqlComGraba.Parameters.Add("@SheetId", System.Data.SqlDbType.Int);

                    sqlComGraba.Parameters["@Imagen"].Value = imagen;
                    sqlComGraba.Parameters["@EncuestaId"].Value = 4;
                    sqlComGraba.Parameters["@EncuestadoId"].Value = ID;
                    sqlComGraba.Parameters["@SheetId"].Value = 38;

                        sqlComGraba.ExecuteNonQuery();
                }

                        SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
                        var mail = new MailMessage();
                        mail.From = new MailAddress("apl_mx@ppenaw.com");
                        mail.To.Add(txtCorreo.Text);
                        mail.CC.Add("aponcedeleon51@hotmail.com");
                        mail.CC.Add("a.ponce@aec.mx");
                        mail.Subject = "Registro Galileo OMR encuestado: " + ID.ToString();
                        mail.IsBodyHtml = true;
                        string htmlbody = "En breve su registro será validado  . Gracias";
                        mail.Body = htmlbody;
                        SmtpServer.Port = 8889;
                        SmtpServer.UseDefaultCredentials = true;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("apl_mx@ppenaw.com", "Pru3b4_");
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);


                        Label1.Text = "La hoja fue almacenada con el número: " + ID.ToString();
                    }
                    catch (Exception ex)
                    {
                                               ex.ToString();
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                    SystemSounds.Beep.Play();
                }
                else
                {
                    Label1.Text = "No acepta este tipo de archivo";
                }
            }
        }

        private void Engine_OnExtractionFailed(ExtractionFailedArgs e)
        {
            asMessage += "Extracción falló: " + e.FailureException.Message + "\r\n";
        }

        private void Engine_onAsyncCompletion(CompletionEvenArgs e)
        {
            int pregunta = 0;
            try
            {
                for (int i = 0; i < engine.NumberBlocks.Count; i++)
                {
                    pregunta = engine.NumberBlocks[i].StartOfInd;
                    int respuesta = int.Parse(engine.NumberBlocks[i].MarkedValue.ToString());
                    if (pregunta == 8 && 0 <= respuesta)
                        DropDownList1.SelectedIndex = (respuesta > 13 ? 13 : respuesta);
                    if (pregunta == 9)
                        TextBox1.Text = respuesta.ToString();
                }
                for (int i = 0; i < engine.AnswerBlocks.Count; i++)
                {
                    pregunta = engine.AnswerBlocks[i].StartOfInd;
                    for (int ii = 0; ii < engine.AnswerBlocks[i].NumberOfLines; ii++)
                        for (int j = 0; j < engine.AnswerBlocks[i].Options; j++)
                            if (engine.AnswerBlocks[i].BinaryMaskedOMs[ii, j])
                                switch (pregunta)
                                {
                                    case 1:
                                        CheckBoxList1.SelectedIndex = j;
                                        break;
                                    case 2:
                                        CheckBoxList2.SelectedIndex = j;
                                        break;
                                    case 3:
                                        CheckBoxList3.SelectedIndex = j;
                                        break;
                                    case 4:
                                        CheckBoxList4.SelectedIndex = j;
                                        break;
                                    case 5:
                                        CheckBoxList5.SelectedIndex = j;
                                        break;
                                    case 6:
                                        CheckBoxList6.SelectedIndex = j;
                                        break;
                                    case 7:
                                        CheckBoxList7.SelectedIndex = j;
                                        break;
                                    default:
                                        //                                Console.WriteLine("Default case");
                                        break;
                                }
                }
            }
            catch (Exception ex)
            {
                //                        ex.ToString();
            }
            finally
            {
            }
            SystemSounds.Beep.Play();

        }

        private void Engine_onAsyncProgressUpdated(ProgressEventArgs e)
        {
            asProgress = e.Value;
        }

        void engine_onAsyncProgressUpdated(object sender, ProgressEventArgs e)
        {
            asProgress = e.Value;
        }

        private void Engine_onInDepthProessUpdate(ProgressUpdateEventArgs e)
        {
            if (e.LatestImage != null && e.IsMainImage == true)
            { asImage1 = e.LatestImage; }
            else if (e.LatestImage != null && e.IsMainImage == false)
            {
                asImage2 = e.LatestImage;
                inDepthImages.Add(e.LatestImage);
                indepthMessages.Add(e.UpdateText);
                if (indepthMessages.Count >= 1)
                {
                    inDepthListCrsr = 0;
                }

            }
            asMessage2 = e.UpdateText + "\r\n";
        }

        private void calcula(string filenamet, string ruta)
        {
            string sheetAddTB = "Data Source=SQL5031.SmarterASP.NET;Initial Catalog=DB_A094FD_CEOP;Persist Security Info=True;User ID=DB_A094FD_CEOP_admin;Password=ep1tafi0";
            string sheetNameTB = "Completa";
            engine = new OMREngine(filenamet, sheetAddTB, sheetNameTB, "Encuesta", ruta);
            inDepthImages = new List<System.Drawing.Image>();
            engine.inDepthFeedBack = true;
            engine.onAsyncProgressUpdated += Engine_onAsyncProgressUpdated;
            engine.onAsyncCompletion += Engine_onAsyncCompletion;
            engine.OnExtractionFailed += Engine_OnExtractionFailed;
            engine.onInDepthProessUpdate += Engine_onInDepthProessUpdate;
            //            engine.StartProcessAsync();
            engine.StartProcess();
            inDepthImages.Clear();
        }

    }
}