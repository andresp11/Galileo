using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Drawing;
using System.Configuration;

namespace OMR.helpers
{
    public class dbOps
    {
        public static DataSet rawSelectCommand(string command, string dataBaseAddress)
        {
//            OleDbConnection dbCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + dataBaseAddress + "\"");
            DataSet ds = new DataSet();
//            OleDbDataAdapter da = new OleDbDataAdapter(command, dbCon);
//            SqlConnection dbCon = new SqlConnection("Data Source=SQL5031.SmarterASP.NET;Initial Catalog=DB_A094FD_CEOP;Persist Security Info=True;User ID=DB_A094FD_CEOP_admin;Password=ep1tafi0");
            SqlConnection dbCon = new SqlConnection(dataBaseAddress);
            
            SqlDataAdapter da = new SqlDataAdapter(command, dbCon);
            dbCon.Open();
            try
            {
                da.SelectCommand.ExecuteNonQuery();
                da.Fill(ds);
            }
            catch (Exception ex) { throw new Exception("Instrucción select invalida"); }
            finally { dbCon.Close(); }
            return ds;
        }
        public static void oneRecordInsertCommand(int propID, string name, object value, string dataBaseAddress)
        {
            rawInsertCommand(String.Format(
                    "INSERT INTO propDetails (pID, pName, pValue) VALUES ({0}, '{1}', {2});",
                    propID, name, value
                    ), dataBaseAddress);
        }
        public static void rawInsertCommand(string command, string dataBaseAddress)
        {
//            OleDbConnection dbCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + dataBaseAddress + "\"");
//            OleDbDataAdapter da = new OleDbDataAdapter();
//            da.InsertCommand = new OleDbCommand(command, dbCon);
            SqlConnection dbCon = new SqlConnection(dataBaseAddress);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.InsertCommand = new SqlCommand(command, dbCon);
            dbCon.Open();
            try
            {
                da.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw new Exception("Instrucción insert invalida"); }
            finally { dbCon.Close(); }
        }
        public static void rawDeleteCommnad(string command, string dataBaseAddress)
        {
//            OleDbConnection dbCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + dataBaseAddress + "\"");
//            DataSet ds = new DataSet();
//            OleDbDataAdapter da = new OleDbDataAdapter();
//            da.DeleteCommand = new OleDbCommand(command, dbCon);
            SqlConnection dbCon = new SqlConnection(dataBaseAddress);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.DeleteCommand = new SqlCommand(command, dbCon);
            dbCon.Open();
            try
            {
                da.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw new Exception("Instrucción delete invalida"); }
            finally { dbCon.Close(); }
        }

        public static List<List<bool>> getAnswers(string answerKeyTitle, int startOfQuestion, int count, int randNumber, string dataBaseAddress, string sheetName)
        {
            if (randNumber == -1)
                randNumber = 1;
            List<List<bool>> answers = new List<List<bool>>();
            int Respuestas = Convert.ToInt32(rawSelectCommand("SELECT sheets.nPregunta FROM sheets WHERE sheets.sheetName = '" + sheetName + "'", dataBaseAddress).Tables[0].Rows[0][0].ToString());

            for (int i = 0; i < Respuestas; i++)
            {
                answers.Add(new List<bool>());
                string ans = "0";
                for (int j = 0; j < ans.Length; j++)
                {
                    answers[i].Add(ans[j] == '1');
                }
            }
            return answers;
        }
        public static void newAnswer(string answerKeyTitle, int questionNumber, List<bool> answer, int randNumber, string dataBaseAddress)
        {

            int answerKeyId = Convert.ToInt32(rawSelectCommand("SELECT akID FROM answerKeys WHERE title = '" + answerKeyTitle + "'", dataBaseAddress).Tables[0].Rows[0][0].ToString());
            newAnswer(answerKeyId, questionNumber, answer, randNumber, dataBaseAddress);
        }
        public static void newAnswer(int answerKeyID, int questionNumber, List <bool> answer, int randNumber,  string dataBaseAddress)
        {
            string ansStr = "";
            for (int i = 0; i < answer.Count; i++)
                ansStr += answer[i] ? "1" : "0";
            rawInsertCommand(string.Format("INSERT INTO answers (randNo, answerKeyID, questionNo, answer) VALUES ({0}, {1}, {2}, '{3}');", randNumber, answerKeyID, questionNumber, ansStr), dataBaseAddress);
        }
        public static void newAnswerKey(string keyTitle, int randomizationKeys, int randBlockInd, string OMRsheetName, int posMarking, int negMarking, bool useAndOpForMultiple, string dataBaseAddress)
        {
            int sheetID = Convert.ToInt32(rawSelectCommand("SELECT sheets.ID FROM sheets WHERE sheets.sheetName = '" + OMRsheetName + "'", dataBaseAddress).Tables[0].Rows[0][0].ToString());
            newAnswerKey(keyTitle, randomizationKeys,randBlockInd, sheetID, posMarking,negMarking,useAndOpForMultiple, dataBaseAddress);
        }
        public static void newAnswerKey(string keyTitle, int randomizationKeys, int randBlockInd, int OMRsheetID, int posMarking, int negMarking, bool useAndOpForMultiple, string dataBaseAddress)
        {
            rawDeleteCommnad("DELETE FROM answerKeys WHERE answerKeys.title = '" + keyTitle + "'", dataBaseAddress);
            rawInsertCommand(string.Format("INSERT INTO answerKeys (title, sheetID, totalRandKeys, randKeyBlockInd, positiveMarking, negativeMarking, andOpForMultiple) VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, {6});", keyTitle, OMRsheetID, randomizationKeys, randBlockInd, posMarking, negMarking, useAndOpForMultiple.ToString()), dataBaseAddress);
        }
        public static void newSheet(string sheetName, string encuesta, string dataBaseAddress, string nPre, string iPre)
        {
            rawDeleteCommnad("DELETE FROM sheets WHERE sheets.sheetName = '" + sheetName + "'", dataBaseAddress);
            rawInsertCommand("INSERT INTO sheets (sheetName, encuestaid, nPregunta, iPregunta) VALUES ('" + sheetName + "', " + encuesta + ", " + nPre + ", " + iPre + ")", dataBaseAddress);
        }

        public static void saveProperty(OMR.Enums.sheetProperties sv, object value, string OMRsheetName, string dataBaseAddress, Type objectType)
        {
            saveProperty(sv.ToString(), value, OMRsheetName, dataBaseAddress, OMR.Enums.enumHelper.structureTypeOfvariable(sv));
        }

        public static void saveProperty(string sv, object value, string sheetName, string dataBaseAddress, Type objectType)
        {
            int sheetID = Convert.ToInt32(rawSelectCommand("SELECT sheets.ID FROM sheets WHERE sheets.sheetName = '" + sheetName + "'", dataBaseAddress).Tables[0].Rows[0][0].ToString());
            rawInsertCommand(String.Format("INSERT INTO sheetProps (sheetID, pName) VALUES ({0}, '{1}');", sheetID, sv.ToString()), dataBaseAddress);
            int propID = -1;
            DataSet ds = rawSelectCommand(String.Format(
                 "SELECT sheetProps.pID FROM sheetProps WHERE sheetProps.sheetID = {0} AND sheetProps.pName = '{1}' ORDER BY sheetProps.pID;",
                 sheetID, sv
                 ), dataBaseAddress);

            propID = Convert.ToInt32(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0].ToString());

            if (objectType == typeof(int))
            {
                oneRecordInsertCommand(propID, "value", (int)value, dataBaseAddress);
            }
            else if (objectType == typeof(double))
            {
                oneRecordInsertCommand(propID, "value", (double)value, dataBaseAddress);
            }
            else if (objectType == typeof(Size))
            {
                oneRecordInsertCommand(propID, "width", ((Size)value).Width, dataBaseAddress);
                oneRecordInsertCommand(propID, "height", ((Size)value).Height, dataBaseAddress);
            }
            else if (objectType == typeof(Size))
            {
                oneRecordInsertCommand(propID, "width", ((SizeF)value).Width, dataBaseAddress);
                oneRecordInsertCommand(propID, "height", ((SizeF)value).Height, dataBaseAddress);
            }
            else if (objectType == typeof(sheetObjectTypes.answerBlock))
            {
                var toAppend = (sheetObjectTypes.answerBlock)value;
                oneRecordInsertCommand(propID, "width", toAppend.gRectangle.Width, dataBaseAddress);
                oneRecordInsertCommand(propID, "height", toAppend.gRectangle.Height, dataBaseAddress);
                oneRecordInsertCommand(propID, "x", toAppend.gRectangle.X, dataBaseAddress);
                oneRecordInsertCommand(propID, "y", toAppend.gRectangle.Y, dataBaseAddress);
                oneRecordInsertCommand(propID, "options", toAppend.Options, dataBaseAddress);
                oneRecordInsertCommand(propID, "rows", toAppend.NumberOfLines, dataBaseAddress);
                oneRecordInsertCommand(propID, "blockID", toAppend.BlockID, dataBaseAddress);
                oneRecordInsertCommand(propID, "startOfInd", toAppend.StartOfInd, dataBaseAddress);
            }
            else if (objectType == typeof(sheetObjectTypes.numberBlock))
            {
                var toAppend = (sheetObjectTypes.numberBlock)value;
                oneRecordInsertCommand(propID, "width", toAppend.gRectangle.Width, dataBaseAddress);
                oneRecordInsertCommand(propID, "height", toAppend.gRectangle.Height, dataBaseAddress);
                oneRecordInsertCommand(propID, "x", toAppend.gRectangle.X, dataBaseAddress);
                oneRecordInsertCommand(propID, "y", toAppend.gRectangle.Y, dataBaseAddress);
                oneRecordInsertCommand(propID, "digits", toAppend.Digits, dataBaseAddress);
                oneRecordInsertCommand(propID, "blockID", toAppend.BlockID, dataBaseAddress);
                oneRecordInsertCommand(propID, "startOfInd", toAppend.StartOfInd, dataBaseAddress);
            }

            else if (objectType == typeof(sheetObjectTypes.emptyBlock))
            {
                var toAppend = (sheetObjectTypes.emptyBlock)value;
                oneRecordInsertCommand(propID, "width", toAppend.gRectangle.Width, dataBaseAddress);
                oneRecordInsertCommand(propID, "height", toAppend.gRectangle.Height, dataBaseAddress);
                oneRecordInsertCommand(propID, "x", toAppend.gRectangle.X, dataBaseAddress);
                oneRecordInsertCommand(propID, "y", toAppend.gRectangle.Y, dataBaseAddress);
                oneRecordInsertCommand(propID, "blockID", toAppend.BlockID, dataBaseAddress);
            }
            else
            {
                throw new Exception("Tipo de objeto no definida");
            }

        }
        public static object getPropertyAsRaw(OMR.Enums.sheetProperties sv, string sheetName, string dataBaseAddress)
        {
            string propertyName = sv.ToString();
//            OleDbConnection dbCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + dataBaseAddress + "\"");
//            DataSet ds = new DataSet();
//            OleDbDataAdapter da = new OleDbDataAdapter(
//                "SELECT propDetails.pName, propDetails.pValue " +
//                "FROM sheets INNER JOIN (sheetProps INNER JOIN propDetails ON sheetProps.pID = propDetails.pID) ON sheets.ID = sheetProps.sheetID " +
//                "WHERE (((sheets.sheetName)=\"" + sheetName + "\") AND ((sheetProps.pName)=\"" + propertyName + "\")) ORDER BY propDetails.pName;", dbCon);

            SqlConnection dbCon = new SqlConnection(dataBaseAddress);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("SELECT propDetails.pName, propDetails.pValue " +
                "FROM sheets INNER JOIN (sheetProps INNER JOIN propDetails ON sheetProps.pID = propDetails.pID) ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheets.sheetName)= '" + sheetName + "') AND ((sheetProps.pName)='" + propertyName + "')) ORDER BY propDetails.pName;", dbCon);
            
            dbCon.Open();
            try
            {
                da.SelectCommand.ExecuteNonQuery();
                da.Fill(ds);
            }
            catch (Exception ex) { throw new Exception("Invalid Database."); }
            finally { dbCon.Close(); }
            return ds.Tables[0].Rows;
        }

        public static object getSheetUniqueProperty(OMR.Enums.sheetProperties sv, string sheetName, string dataBaseAddress)
        {
            return getProperty(sv, sheetName, dataBaseAddress, 0);
        }
        public static object getProperty(OMR.Enums.sheetProperties sv, string sheetName, string dataBaseAddress, int index)
        {
            return getProperty(sv.ToString(), sheetName, dataBaseAddress, OMR.Enums.enumHelper.dataTypeOfvariable(sv), index);
        }

        public static object getProperty(string sv, string sheetName, string dataBaseAddress, Type returnType, int index)
        {
            string propertyName = sv;
            DataSet ds = new DataSet();
            DataSet ds2 = rawSelectCommand(
                "SELECT DISTINCT sheetProps.pID " +
                "FROM sheets INNER JOIN (sheetProps INNER JOIN propDetails ON sheetProps.pID = propDetails.pID) ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheetProps.pName)='" + sv + "') AND ((sheets.sheetName) = '" + sheetName + "'))" +
                "ORDER BY sheetProps.pID;", dataBaseAddress);
            int[] avaliableInd = new int[ds2.Tables[0].Rows.Count];
            for (int i = 0; i < avaliableInd.Length; i++)
            {
                avaliableInd[i] = Convert.ToInt32(ds2.Tables[0].Rows[i][0].ToString());
            }
            ds = rawSelectCommand(
                "SELECT LOWER(propDetails.pName), propDetails.pValue " +
                "FROM sheets INNER JOIN (sheetProps INNER JOIN propDetails ON sheetProps.pID = propDetails.pID) ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheets.sheetName)= '" + sheetName + "') AND ((sheetProps.pName)='" + propertyName + "') AND (sheetProps.pID = " + avaliableInd[index].ToString() +
                ")) ORDER BY propDetails.pName;", dataBaseAddress);

            //DECODE EACH type. these have to be done: Size, SizeF, Rectangle, RectangleF, Double, int, string,
            Dictionary<string ,object> data = toDict(ds.Tables[0]);
            if (returnType == typeof(Size))
            {
                return new Size((int)Convert.ToInt32(data["width"].ToString()), (int)Convert.ToInt32(data["height"].ToString()));
            }
            else if (returnType == typeof(SizeF))
            {
                return new SizeF((float)Convert.ToDouble(data["width"].ToString()), (float)Convert.ToDouble(data["height"].ToString()));
            }
            else if (returnType == typeof(Rectangle))
            {
                return new Rectangle(
                    (int)Convert.ToInt32(data["x"].ToString()), (int)Convert.ToInt32(data["y"].ToString()),
                    (int)Convert.ToInt32(data["width"].ToString()), (int)Convert.ToInt32(data["height"].ToString()));
            }
            else if (returnType == typeof(RectangleF))
            {
                return new RectangleF(
                    (float)Convert.ToDouble(data["x"].ToString()), (float)Convert.ToDouble(data["y"].ToString()),
                    (float)Convert.ToDouble(data["width"].ToString()), (float)Convert.ToDouble(data["height"].ToString()));
            }
            else if (returnType == typeof(double))
            {
                return Convert.ToDouble(ds.Tables[0].Rows[0][1]);
            }
            throw new Exception("Don't know how to parse this kind of data: " + returnType.ToString());
        }
        public static DataRowCollection getBlockPropIds(OMR.Enums.sheetProperties sv, string sheetName, string dataBaseAddress)
        {
            return rawSelectCommand(
                "SELECT sheetProps.pID " +
                "FROM sheets INNER JOIN sheetProps ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheets.sheetName)= '" + sheetName + "') AND ((sheetProps.pName)= '" + sv.ToString() + "'));",
                dataBaseAddress).Tables[0].Rows;
        }
        public static int countBlocks(OMR.Enums.sheetProperties sv, string sheetName, string dataBaseAddress)
        {
            return rawSelectCommand(
                "SELECT sheetProps.pID " +
                "FROM sheets INNER JOIN sheetProps ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheets.sheetName)= '" + sheetName + "') AND ((sheetProps.pName)='" + sv.ToString() + "'));", dataBaseAddress).Tables[0].Rows.Count;
        }
        public static object getPropDetail(OMR.Enums.sheetProperties sv, string propID, string detailPName, string sheetName, string dataBaseAddress)
        {
            return rawSelectCommand(
                "SELECT propDetails.pValue " +
                "FROM sheets INNER JOIN (sheetProps INNER JOIN propDetails ON sheetProps.pID = propDetails.pID) ON sheets.ID = sheetProps.sheetID " +
                "WHERE (((sheetProps.pID)=" + propID + ") AND ((sheets.sheetName)='" + sheetName + "') AND ((sheetProps.pName)='" + sv.ToString() + "') AND ((propDetails.pName)='" + detailPName + "'));"
                 , dataBaseAddress).Tables[0].Rows[0][0];
        }
        private static Dictionary <string, object> toDict(DataTable dt)
        {
            return dt.AsEnumerable()
              .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
                                        row => row.Field<object>(1));
        }

    }
}
