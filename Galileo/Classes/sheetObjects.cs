using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OMR.helpers;
using System.Data;

namespace OMR.sheetObjectTypes
{   
    /// <summary>
    /// a constructor class representing an answerBlock
    /// </summary>
    public class answerBlock
    {
        /// <summary>
        /// Updates information about properties which can be sought from the database
        /// </summary>
        /// <param name="dbPath">Address of access DB</param>
        /// <param name="sheetName">sheet name</param>
        /// <param name="ind">index of block from the sheet</param>
        public void PopulateProperties(string dbPath, string sheetName, int ind)
        {
            //start collecting data from database and keep the end user posted of what we are upto
            DataRowCollection abrows = dbOps.getBlockPropIds(Enums.sheetProperties.AnswerBlock, sheetName, dbPath);
            int propId = Convert.ToInt32(abrows[ind][0].ToString());
            //these lines are similar. Just getting information about the current answers block from the database and filling them in proper class properties
            gRectangle = (RectangleF)dbOps.getProperty(Enums.sheetProperties.AnswerBlock, sheetName, dbPath, ind);
            BlockID = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.AnswerBlock, propId.ToString(), "blockID", sheetName, dbPath));
            NumberOfLines = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.AnswerBlock, propId.ToString(), "rows", sheetName, dbPath));
            StartOfInd = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.AnswerBlock, propId.ToString(), "startOfInd", sheetName, dbPath));
            Options = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.AnswerBlock, propId.ToString(), "options", sheetName, dbPath));
            //initiallize binary marks array
            BinaryMaskedOMs = new bool[NumberOfLines, Options];
        }
        /// <summary>
        /// If given with an answerkey, this will calculate the scores of each question which can be aquired using IndividualScore/TotalScore properties
        /// </summary>
        /// <param name="dbPath">Path of database</param>
        /// <param name="answerKey">Name of Answer Key</param>
        /// <param name="randomizationIndex">Randomization index of sheet. Each question may have different answers on different block based on randomization index of sheet. </param>
        public void RateScores(string dbPath, string answerKeyTitle, int randomizationIndex, int positiveMarking, int negativeMarking, bool useAndOpForMultipleAns, string sheetName)
        {
            List<List<bool>> answers = dbOps.getAnswers(answerKeyTitle, startOfInd, numberOfLines, randomizationIndex, dbPath, sheetName);
            scores = new int[NumberOfLines];
            string res = "";
            for (int i = 0; i < answers[i].Count; i++)
                for (int j = 0; j < answers[i].Count; j++)
                    res = BinaryMaskedOMs[i, j].ToString();
            //DataTable dt = dbOps.rawSelectCommand(string.Format(
            //    "SELECT answerKeys.positiveMarking, answerKeys.negativeMarking, answerKeys.andOpForMultiple FROM answerKeys WHERE(((answerKeys.title) = \"{0}\"));",
            //    answerKeyTitle), dbPath).Tables[0];
            for (int i = 0; i < answers.Count; i++)
            {
                if (useAndOpForMultipleAns)
                {
                    bool allRight = true;
                    bool hasMarked = false;
                    for (int j = 0; j < answers[i].Count; j++)
                    {
                        if (BinaryMaskedOMs[i, j] != answers[i][j])
                            allRight = false;
                        if (BinaryMaskedOMs[i, j])
                            hasMarked = true;
                    }
                    if (!hasMarked)
                        IndividualScores[i] = 0;
                    else if (allRight)
                        IndividualScores[i] = positiveMarking;
                    else
                        IndividualScores[i] = negativeMarking;
                }
                else
                {
                    bool noWrong = true;
                    bool hasRight = false;

                    for (int j = 0; j < answers[i].Count; j++)
                    {
                        if (BinaryMaskedOMs[i, j] == answers[i][j])
                            hasRight = true;
                        else if (answers[i][j] == false && BinaryMaskedOMs[i, j] == true)// ans said false, then say false
                            noWrong = false;
                    }
                    if (noWrong && !hasRight)
                        IndividualScores[i] = 0;
                    else if (noWrong && hasRight)
                        IndividualScores[i] = positiveMarking;
                    else
                        IndividualScores[i] = negativeMarking;
                }
            }
        }
        /// <summary>
        /// A rectangle describing the actual zone of image where this block lies. Units are scalled on the OMR reader scale. So these Values must be used only using the in-built functions
        /// </summary>
        public RectangleF gRectangle { get { return rect_; } set { rect_ = value; } }
        /// <summary>
        /// Total number of Options in the Answer block
        /// </summary>
        public int Options { get { return options; } set { options = value; } }
        /// <summary>
        /// total number of answer lines in this block
        /// </summary>
        public int NumberOfLines { get { return numberOfLines; } set { numberOfLines = value; } }
        /// <summary>
        /// A zero-based index of current block in other (if any) blocks of same kind in the sheet
        /// </summary>
        public int BlockID { get { return bid; } set { bid = value; } }
        /// <summary>
        /// in the scope of whole sheet, this return a one-based index of the first answer represented by this block
        /// </summary>
        public int StartOfInd { get { return startOfInd; } set { startOfInd = value; } }
        /// <summary>
        /// a visualization of cropped block image
        /// </summary>
        public Image CroppedImage { get { return img; } set { img = value; } }
        /// <summary>
        /// The visual Marks location as written in binary table. Table is formated as NumberOfAnswerLines x Options
        /// </summary>
        public bool[,] BinaryMaskedOMs { get { return binMask; } set { binMask = value; } }
        /// <summary>
        /// Scores of a solved answersheet as rated according to the rule set by answerkey
        /// </summary>
        public int[] IndividualScores { get { return scores; } set { scores = value; } }
        /// <summary>
        /// Total score of this block as rated according to the rule set by answerkey
        /// </summary>
        public int TotalScore { get { return scores.Sum(); } }
        /// <summary>
        /// Randomization ID of current sheet which was used to count the marks of this block
        /// </summary>
        public int RandomizationID{get{ return randId; }}
        /// <summary>
        /// cropped image of each line in the block. It contains a line for each digit of Number number
        /// </summary>
        public Image[] CroppedAnswerLines { get { return chImgs; } set { chImgs = value; } }
        /// <summary>
        /// a human readable result of this answer block
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "";
            if (numberOfLines > 0)
            {
                str += "Bloque ID: " + BlockID.ToString() + "\r\n";
                str += "Rango respuesta [" + startOfInd.ToString() + "," + (startOfInd + numberOfLines - 1).ToString() + "]\r\n";
                str += "Respuesta: {";
                for (int i = 0; i < NumberOfLines; i++)
                {
                    bool alreadyMarked = false;
                    int marked = -1;
                    for (int j = 0; j < Options; j++)
                    {
                        if (BinaryMaskedOMs[i, j])
                        {
                            if (alreadyMarked)
                            {
                                str += "Multiple";
                                j = Options;
                            }
                            else
                            {
                                alreadyMarked = true;
                                marked = j;
                            }
                            if (j == Options - 1)
                                str += (marked + 1).ToString();
                        }
                        else if (j == Options - 1 && !alreadyMarked)
                            str += "None";
                        else if (j == Options - 1)
                        {
                            str += (marked + 1).ToString();
                        }
                    }
                    if (i < numberOfLines - 1)
                        str += ", ";
                    else
                        str += "}";

                }

/*                if (scores.Length > 0)
                {
                    str += "\r\nAnswers Scores: {";
                    for (int i = 0; i < NumberOfLines; i++)
                    {
                        str += scores[i].ToString();
                        if (i < numberOfLines - 1)
                            str += ", ";
                        else
                            str += "}";
                    }
                }*/
                return str;
            }
            else return "NO hay respuesta";
        }
        RectangleF rect_ = new RectangleF();
        int options = 0, numberOfLines = 0, bid, startOfInd, randId;
        Image img;
        bool[,] binMask;
        int[] scores;
        
        Image[] chImgs;
    }
    /// <summary>
    /// A constructor class representing a Number block
    /// </summary>
    public class numberBlock
    {
        /// <summary>
        /// Updates information about properties which can be sought from the database
        /// </summary>
        /// <param name="sheetPath">Address of access DB</param>
        /// <param name="sheetPath">sheet name</param>
        /// <param name="ind">index of block from the sheet</param>
        public void PopulateProperties(string sheetPath, string sheetName, int i)
        {
            DataRowCollection rbrows = dbOps.getBlockPropIds(Enums.sheetProperties.NumberBlock, sheetName, sheetPath);
            int propId = Convert.ToInt32(rbrows[i][0].ToString());
            gRectangle = (RectangleF)dbOps.getProperty(Enums.sheetProperties.NumberBlock,sheetName, sheetPath, i);
            BlockID = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.NumberBlock, propId.ToString(), "blockID", sheetName, sheetPath));
            StartOfInd = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.NumberBlock, propId.ToString(), "startOfInd", sheetName, sheetPath));
            Digits = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.NumberBlock, propId.ToString(), "digits", sheetName, sheetPath));
            BinaryMaskedOMs = new bool[Digits, 10];
        }
        /// <summary>
        /// A rectangle describing the actual zone of image where this block lies. Units are scalled on the OMR reader scale. So these Values must be used only using the in-built functions
        /// </summary>
        public RectangleF gRectangle { get { return rect_; } set { rect_ = value; } }
        /// <summary>
        /// Total number of digits in the Number block
        /// </summary>
        public int Digits { get { return digits; } set { digits = value; } }
        /// <summary>
        /// A zero-based index of current block in other (if any) blocks of same kind in the sheet
        /// </summary>
        public int BlockID { get { return bid; } set { bid = value; } }
        /// <summary>
        /// a visualization of cropped block image
        /// </summary>
        public Image CroppedImage { get { return img; } set { img = value; } }
        /// <summary>
        /// in the scope of whole sheet, this return a one-based index of the first answer represented by this block
        /// </summary>
        public int StartOfInd { get { return startOfInd; } set { startOfInd = value; } }
        /// <summary>
        /// The visual Marks location as written in binary table. Table is formated as NumberOfDigits x 10 (Counting from 0 to 9)
        /// </summary>
        public bool[,] BinaryMaskedOMs { get { return binMask; } set { binMask = value; } }
        /// <summary>
        /// cropped image of each line in the block. It contains a line for each digit of Number number
        /// </summary>
        public Image[] CroppedDigitLines { get { return chImgs; } set { chImgs = value; } }
        /// <summary>
        /// Inetgral value expressing the marked number. Returns -1 in case wrong options are marked or a line is completely unmarked. 
        /// It means that while marking the sheet, the Zeros on the Left of number are significant
        /// </summary>
        public int MarkedValue
        {
            get
            {
                if (Digits == 0) return -1;
                int ans = 0;
                bool lineContainsAMark;
                for (int i = 0; i < Digits; i++)
                {
                    lineContainsAMark = false;
                    for (int j = 0; j < 10; j++)
                    {
                        if (binMask[i, j])
                        {
                            if (lineContainsAMark)
                                return -1;
                            else ans += Convert.ToInt32(Math.Round(Math.Pow(10, digits - i - 1))) * j;
                            lineContainsAMark = true;
                        }
                        else
                        {
                            if (j == 9 && !lineContainsAMark)
                                return -1;
                        }
                    }

                }
                return ans;
            }
        }
        
        bool[,] binMask;
        Image[] chImgs;
        RectangleF rect_ = new RectangleF();
        int digits = 0, bid, startOfInd;
        Image img;

    }
    /// <summary>
    /// A constructor class representing an Empty/Text Block
    /// </summary>
    public class emptyBlock
    {
        /// <summary>
        /// Updates information about properties which can be sought from the database
        /// </summary>
        /// <param name="sheetPath">Address of access DB</param>
        /// <param name="sheetName">sheet name</param>
        /// <param name="ind">index of block from the sheet</param>
        public void PopulateProperties(string sheetPath, string sheetName, int i)
        {
            DataRowCollection ebrows = dbOps.getBlockPropIds(Enums.sheetProperties.EmptyBlock, sheetName, sheetPath);
            int propId = Convert.ToInt32(ebrows[i][0].ToString());
            gRectangle = (RectangleF)dbOps.getProperty(Enums.sheetProperties.EmptyBlock, sheetName, sheetPath, i);
            BlockID = Convert.ToInt32((string)dbOps.getPropDetail(Enums.sheetProperties.EmptyBlock, propId.ToString(), "blockID", sheetName, sheetPath));
        }
        public RectangleF gRectangle { get { return rect_; } set { rect_ = value; } }
        public int BlockID { get { return bid; } set { bid = value; } }
        public Image CroppedImage { get { return img; } set { img = value; } }

        RectangleF rect_ = new RectangleF();
        int bid;
        Image img;
    }
    
}
