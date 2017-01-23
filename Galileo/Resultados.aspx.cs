using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Galileo
{
    public partial class Resultados : System.Web.UI.Page
    {
        static string DC = System.Configuration.ConfigurationManager.ConnectionStrings["sqlconn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            //DropDownList1.SelectedIndex = 0;
            //txtDetalle.Text = DropDownList1.SelectedItem.Text;
        }

        public void sendmail()
        {
            if (txtEmail.Text.Length > 0)
            {
                
                SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("apl_mx@ppenaw.com");
                mail.To.Add(txtEmail.Text);
                mail.CC.Add("aponcedeleon51@hotmail.com");
                mail.CC.Add("a.ponce@aec.mx");
                mail.Subject = "Evaluación Galileo OMR";
                mail.IsBodyHtml = true;
                string htmlbody = "Pronto nos pondremos en contacto, agradecemos su evaluación. Gracias";
                mail.Body = htmlbody;
                SmtpServer.Port = 8889;
                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential("apl_mx@ppenaw.com", "Pru3b4_");
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
            }
        }

        public void sendalarma()
        {
            if (txtEmail.Text != null)
            { 
                SqlConnection sqlConn = new SqlConnection(DC);
                sqlConn.Open();
                SqlDataAdapter sqlID = new SqlDataAdapter();
                DataSet ds = new DataSet();
                sqlID.SelectCommand = new SqlCommand();
                sqlID.SelectCommand.Connection = sqlConn;
                sqlID.SelectCommand.CommandText = "AlarmaEjemploAmai";
                sqlID.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlID.SelectCommand.ExecuteNonQuery();
                sqlID.Fill(ds, "Ejemploweb");
                int ID = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                if (ID <= 50)
                {
                    SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
                    var mail = new MailMessage();
                    mail.From = new MailAddress("aponcedeleon51@hotmail.com.com");
                    mail.To.Add(txtEmail.Text);
                    mail.CC.Add("aponcedeleon51@hotmail.com");
                    mail.CC.Add("andres.ponce.de.leon.huerta@gmail.com");
                    mail.Subject = "Ingresar 200 registros más";
                    mail.IsBodyHtml = true;
                    string htmlbody = "Urge ingresar otros 200 registros.";
                    mail.Body = htmlbody;
                    SmtpServer.Port = 8889; //587
                    SmtpServer.UseDefaultCredentials = false; 
                    SmtpServer.Credentials = new System.Net.NetworkCredential("aponcedeleon51@hotmail.com", "ep1tafi0");
                    SmtpServer.EnableSsl = true; //false
                    SmtpServer.Send(mail);
                }
                sqlConn.Close();
            }
        }


        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblW.Visible = false;
            lblW.Text = "";
            Image1.Visible = false;
            txtDetalle.Text = ddlEncuestado.SelectedItem.Text;
            DataSet ds = new DataSet();
            SqlConnection sqlConn = new SqlConnection(DC);
            SqlDataAdapter sqlDARespuestas = new SqlDataAdapter();
            sqlDARespuestas.SelectCommand = new SqlCommand();
            sqlDARespuestas.SelectCommand.Connection = sqlConn;
            sqlDARespuestas.SelectCommand.CommandText = "Ejemploweb";
            sqlDARespuestas.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDARespuestas.SelectCommand.Parameters.Add("@EncuestadoId", SqlDbType.Int).Value = int.Parse(ddlEncuestado.SelectedValue);
            int Id = int.Parse(ddlEncuestado.SelectedValue);

            try
                {
                    sqlConn.Open();
                    SqlCommand cmdImg = new SqlCommand("Select foto from imagen where Encuestaid = @Encuestaid and Encuestadoid = @Encuestadoid and SheetID = @SheetID", sqlConn);
                    cmdImg.Parameters.Add("@Encuestaid", SqlDbType.Int).Value = 4;
                    cmdImg.Parameters.Add("@Encuestadoid", SqlDbType.Int).Value = int.Parse(ddlEncuestado.SelectedValue);
                    cmdImg.Parameters.Add("@SheetID", SqlDbType.Int).Value = 38;
                    SqlDataReader drIng = cmdImg.ExecuteReader();
                    while (drIng.Read() )
                    {
                        if (drIng[0] != System.DBNull.Value)
                        {
                            byte[] bytes = new Byte[0];
                            bytes = (Byte[])(drIng[0]);
                            string b64Str = Convert.ToBase64String(bytes, 0, bytes.Length);
                            Image1.ImageUrl = "data:image/jpeg;base64," + b64Str;
                            Image1.Visible = true;
                        }
                    }

                    drIng.Close();
                    sqlDARespuestas.SelectCommand.ExecuteNonQuery();
                    sqlDARespuestas.Fill(ds, "Ejemploweb");
                    r1.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][0].ToString())-1;
                    r2.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][1].ToString())-1;
                    r3.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][2].ToString())-1;
                    r4.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][3].ToString())- 1;
                    r5.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][4].ToString())-1;
                    r6.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][5].ToString())-1;
                    r7.SelectedIndex = int.Parse(ds.Tables[0].Rows[0][6].ToString())-1;
                    //ddlr9.SelectedValue = int.Parse(ds.Tables[0].Rows[0][7].ToString());
                    
                    string str;
                    str = ds.Tables[0].Rows[0][7].ToString();
                    int v;
                    v = int.Parse(str);
                    if (v >= 13)
                        str = "13";
                    
                    ddlr9.SelectedItem.Selected = false;
                    if (str != null && str != "")
                        ddlr9.Items.FindByValue(str).Selected = true;
                    else
                        ddlr9.SelectedIndex = -1;

                    txtEdad.Text = ds.Tables[0].Rows[0][8].ToString();
                    sqlDARespuestas.Dispose();
                    sqlConn.Close();

                }
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                { 
                    sqlConn.Close();
                }
                
                ddlEncuestado.DataBind();
                ddlEncuestado.SelectedValue = Id.ToString();
            }



        }

        protected void ibNuevo_Click(object sender, ImageClickEventArgs e)
        {
            Image1.ImageUrl = null;
            Image1.Visible = false;
            ddlEncuestado.Visible = false;
            r1.SelectedIndex = -1;
            r2.SelectedIndex = -1;
            r3.SelectedIndex = -1;
            r4.SelectedIndex = -1;
            r5.SelectedIndex = -1; 
            r6.SelectedIndex = -1;
            r7.SelectedIndex = -1;
            ddlr9.SelectedIndex = -1;
            txtEdad.Text = "";
            ibGuardar.Visible = true;
            lblW.Visible = true;
            lblW.Text = "Esta por insertar un nuevo registro.";
            lbl1.Visible = true;
            lbl2.Visible = true;
            lbl4.Visible = true;
            txtEmail.Visible = true;
            txtDetalle.Visible = true;
            txtDetalle.Text = "";
            ibNuevo.Visible = false;
            lbl3.Visible = false;
        }
        public void OMR()
        {

        }

        protected void ibGuardar_Click(object sender, ImageClickEventArgs e)
        {
            Image1.ImageUrl = null;
            Image1.Visible = false;
            SqlConnection sqlConn = new SqlConnection(DC);
            if (txtDetalle.Text != "")
         {       
            try
            {
                sqlConn.Open();

                SqlDataAdapter sqlID = new SqlDataAdapter();
                DataSet ds = new DataSet();
                sqlID.SelectCommand = new SqlCommand();
                sqlID.SelectCommand.Connection = sqlConn;
                sqlID.SelectCommand.CommandText = "GetIdWeb";
                sqlID.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlID.SelectCommand.ExecuteNonQuery();
                sqlID.Fill(ds, "Ejemploweb");
                int ID = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                sendalarma();
                SqlDataAdapter sqlDARespuestas = new SqlDataAdapter();
                sqlDARespuestas.UpdateCommand = new SqlCommand();
                sqlDARespuestas.UpdateCommand.Connection = sqlConn;
                sqlDARespuestas.UpdateCommand.CommandText = "UpdateWeb";
                sqlDARespuestas.UpdateCommand.CommandType = CommandType.StoredProcedure;
                sqlDARespuestas.UpdateCommand.Parameters.Add("@Encuestadokey", SqlDbType.Int).Value = ID;
                sqlDARespuestas.UpdateCommand.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = txtDetalle.Text;
                sqlDARespuestas.UpdateCommand.ExecuteNonQuery();
                sqlDARespuestas.Dispose();

                SqlDataAdapter sqlDARespuestas2 = new SqlDataAdapter();
                sqlDARespuestas2.UpdateCommand = new SqlCommand();
                sqlDARespuestas2.UpdateCommand.Connection = sqlConn;
                sqlDARespuestas2.UpdateCommand.CommandText = "RespuestasUpdateWeb";
                sqlDARespuestas2.UpdateCommand.CommandType = CommandType.StoredProcedure;
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Encuestaid", SqlDbType.Int).Value = 4;
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Encuestadoid", SqlDbType.Int).Value = ID;
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta1", SqlDbType.NVarChar).Value = r1.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta2", SqlDbType.NVarChar).Value = r2.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta3", SqlDbType.NVarChar).Value = r3.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta4", SqlDbType.NVarChar).Value = r4.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta5", SqlDbType.NVarChar).Value = r5.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta6", SqlDbType.NVarChar).Value = r6.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta7", SqlDbType.NVarChar).Value = r7.SelectedIndex.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta8", SqlDbType.NVarChar).Value = ddlr9.SelectedValue.ToString();
                sqlDARespuestas2.UpdateCommand.Parameters.Add("@Respuesta9", SqlDbType.NVarChar).Value = txtEdad.Text;
                sqlDARespuestas2.UpdateCommand.ExecuteNonQuery();
                sqlDARespuestas2.Dispose();
                ddlEncuestado.DataBind();
                sqlConn.Close();

                sendmail();
                lblW.Visible = true;
                lblW.Text = "Inserto un nuevo registro. Ya lo puede consultar.";
                }
            
                
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
                ibGuardar.Visible = false;
                lbl1.Visible = false;
                lbl2.Visible = false;
                lbl4.Visible = false;
                txtEmail.Visible = false;
                txtDetalle.Visible = false;
                ddlEncuestado.Visible = true;
                ibNuevo.Visible = true;
                lbl3.Visible = true;
                Image1.Visible = false;
            }
         }
        }

        protected void ddlr9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
