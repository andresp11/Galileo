using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Media;
using System.Xml;
using OMR;
using OMR.helpers;
using System.Drawing;
using OMR.sheetObjectTypes;
using System.Data;

namespace OMR
{
    public delegate void AsyncProgressEventHandler(ProgressEventArgs e);
    public delegate void AsyncCompletionEventHandler (CompletionEvenArgs e);
    public delegate void ExtractionFailedHandler(ExtractionFailedArgs e);

    
    public class ExtractionFailedArgs : EventArgs
    {
        
        Exception exception;
        int ind;
        public int EngineIndex { get { return ind; } }
        public Exception FailureException { get { return exception; } }
        public ExtractionFailedArgs(Exception ex_, int engineIndex)
        { exception = ex_; ind = engineIndex;
            //throw new Exception("Failed detection and handling is still in development. "+
            //    "Currently, this has been done using try and catch calls. "+
            //    "But apparently, this also causes memory overflows which cause process failure.");
        }
    }
    public class ProgressEventArgs : EventArgs
    {
        int val = 0;
        int ind;
        public int EngineIndex { get { return ind; } }
        public int Value { get { return val; } }
        public ProgressEventArgs(int val_, int engineIndex)
        { val = val_; ind = engineIndex; }
    }
    public class CompletionEvenArgs : EventArgs
    {
        int ind = 0;
        public int EngineIndex { get { return ind; } }
        public CompletionEvenArgs(int engineIndex_)
        { ind = engineIndex_; }
    }
    /// <summary>
    /// This is a wrapper for original OMR reader. 
    /// Unlike OMRReader class, this class uses more object oriented approach. 
    /// For more easy and practical usage, this class is more preferable.
    /// </summary>
    public class OMREngine
    {
        /// <summary>
        /// occurs when an engine fails during process.
        /// </summary>
        public event ExtractionFailedHandler OnExtractionFailed;
        /// <summary>
        /// Occurs when the engine has some progress update. event gars can be used to get the percentage of process completed.
        /// </summary>
        public event AsyncProgressEventHandler onAsyncProgressUpdated;
        /// <summary>
        /// This function is only called if inDepthFeedback flag is set true
        /// for those users who wish to have a more closer look into what the engine is doing, this event will give several updates during process
        /// These updates include a text description and (not always) an image of latest operation performed
        /// </summary>
        public event ProcessUpdatedHandler onInDepthProessUpdate;
        /// <summary>
        /// when this flag is set true, onInDepthProessUpdate event is activated
        /// for those users who wish to have a more closer look into what the engine is doing, this event will give several updates during process
        /// These updates include a text description and (not always) an image of latest operation performed
        /// </summary>
        public bool inDepthFeedBack { get { return inDFB; }set { inDFB = true; } }     
        /// <summary>
        /// This event can be configured in case an A sync Process can be called, to receive the process completion indication.
        /// </summary>
        public event AsyncCompletionEventHandler onAsyncCompletion;
        Exception busy = new Exception("An A sync process is in process");
        Exception notReady = new Exception("A cam image must be processed first for this operation");
        /// <summary>
        /// This determines if the image has been processed completely or not.
        /// </summary>
        public bool ResultReady { get { if (processingAsync) throw busy; return resRead; } }

        /// <summary>
        /// Index is only important when programming with multi-threading
        /// </summary>
        public int Index { get { return index_; } set { index_ = value; } }
        /// <summary>
        /// Gets or sets the cam Image property
        /// Note for multi-thread programming: This call takes a lot of processor due to scaling performed on image
        /// </summary>
        public Image CamImage
        {
            get
            {
                if (camImg != null)
                    return camImg;
                else
                {
                    nbs = new List<numberBlock>(); ebs = new List<emptyBlock>(); abs = new List<answerBlock>();
                    resRead = false; 

//                    Image value = Image.FromFile(rawImgAddress);
                    System.Drawing.Image value = System.Drawing.Image.FromFile(rawImgAddress);
                    if (value.Width * value.Height > 10000000)
                    {
                        int maxRes = (int)Math.Round(10D * 1000000D);
                        //lock (lockObject)
                        {
                            camImg = OMR.helpers.ImageUtilities.ResizeImage(value, (maxRes) / value.Height, (maxRes) / value.Width);
                        }
                    }
                    else
                        camImg = value;
                    return camImg;
                }
            }
            set
            {
                if (processingAsync) throw busy;
                nbs = new List<numberBlock>(); ebs = new List<emptyBlock>(); abs = new List<answerBlock>();
                resRead = false; camImg = value;
                if (value.Width * value.Height > 10000000)
                {
                    int maxRes = (int)Math.Round(10D * 1000000D);
                    lock (lockObject)
                    {
                        camImg = OMR.helpers.ImageUtilities.ResizeImage(value, (maxRes) / value.Height, (maxRes) / value.Width);
                    }
                }
            }
        }
        /// <summary>
        /// White level can be obtained using ImageUtils.getWhiteLevel() method. This level is only effective if OverrideWhite is set true. By default, it is false
        /// </summary>
        public int WhiteLevel
        {
            get
            {
                return whiteLevel_;
            }
            set
            {
                if (processingAsync) throw busy;
                whiteLevel_ = value;
            }
        }
        /// <summary>
        /// While an a sync process is being run, this will return an estimation of the process progress on a scale of 100
        /// </summary>
        public int Progress
        {
            get
            {
                return progress_;
            }
        }
        /// <summary>
        /// Setting this will override the automatic white leveling with the white level provided in WhiteLevel property of this class
        /// </summary>
        public bool OverrideWhite
        {
            get
            {
                lock (lockObject)
                {
                    return orWhite_;
                }
            }
            set
            {
                lock (lockObject)
                {
                    if (processingAsync) throw busy;
                    if (!resRead)
                        orWhite_ = value;
                    else
                        throw notReady;
                }
            }
        }
        /// <summary>
        /// Recognized image after processing
        /// </summary>
        public Image RecognizedImage
        {
            get
            {
                if (processingAsync) throw busy;
                if (resRead)
                    return recogImg;
                else
                    throw notReady;
            }
        }
        /// <summary>
        /// Sheet bounds of detected image
        /// </summary>
        public Size SheetBounds
        {
            get
            {
                if (processingAsync) throw busy;
                if (resRead)
                    return sbounds_;
                else
                    throw notReady;
            }
        }
        /// <summary>
        /// Once the image has been processed, this will contain all the Number blocks detected.
        /// </summary>
        public List<numberBlock> NumberBlocks { get { if (!resRead) throw notReady; if (processingAsync) throw busy; return nbs; } }
        /// <summary>
        /// Once the image has been processed, this will contain all the EmptyBlocks detected.
        /// </summary>
        public List<emptyBlock> EmptyBlocks { get { if (!resRead) throw notReady; if (processingAsync) throw busy; return ebs; } }
        /// <summary>
        /// Once the image has been processed, this will contain all the AnswerBlocks detected.
        /// </summary>
        public List<answerBlock> AnswerBlocks { get { if (!resRead) throw notReady; if (processingAsync) throw busy; return abs; } }
        /// <summary>
        /// Scaling which has been/will be performed on the original image to make it workable with this engine
        /// </summary>
        public double ImageProcessScale
        {
            get
            {
                int pixel = camImg.Width * camImg.Height;
                int minPixForP = 1500000;
                int maxPixForP = 10000000;
                if (pixel <= minPixForP)
                    return (double)minPixForP / (double)pixel;
                if (pixel >= maxPixForP)
                {
                    return (double)pixel / (double)maxPixForP;
                }
                int reqPix = minPixForP + (int)Math.Round((double)oq / 100D * (double)(pixel - minPixForP));
                return (double)reqPix / (double)pixel;
            }
        }
        /// <summary>
        /// Randomization ID of current sheet as read from the Number block specified by answer key
        /// </summary>
        public int AnswerKeyRandomizationID { get { if (!resRead) throw notReady; if (processingAsync) throw busy; return akRandID; } }
        /// <summary>
        /// Total number of random keys found in the data-sheet for this specific answer key
        /// </summary>
        public int TotalRandomizationKeys { get { if (!resRead) throw notReady;return  akTotalRandKeys; } }
        /// <summary>
        /// In case randomization is enabled in the answer key, this index gives the index of Number block used to take randomization index
        /// </summary>
        public int RandomizationBlockIndex { get { if (!resRead) throw notReady; return akRandBlockID; } }
        /// <summary>
        /// Name of answerkey which will be used to evaluate the score of this sheet
        /// </summary>
        public string AnswerKeyTitle { get { if (processingAsync) throw busy; return ansKeyTitle; } set { if (processingAsync) throw busy; ansKeyTitle = value; } }
        /// <summary>
        /// This flag determines which operation was used to calculate the scores. If AND was used, one must mark all of the options for a question having multiple answer options. If False, i.e. OR, One may mark any of the answer options to get the right answer
        /// </summary>
        public bool UseAndOpForMultipleAnswers { get { if (!resRead) throw notReady; return akUseAndForMOps; } }
        /// <summary>
        /// This value was used to score the right answers
        /// </summary>
        public int ScoreForRightQuestion { get { if (!resRead) throw notReady; return akPMark; } }
        /// <summary>
        /// This value was used to deduct score for wrong answers
        /// </summary>
        public int ScoreForWrongQuestion { get { if (!resRead) throw notReady; return akNMark; } }
        public int TotalScore
        {
            get
            {
                if (!resRead) throw notReady; if (processingAsync) throw busy;
                int totalScore = 0; foreach (answerBlock ab in abs) totalScore += ab.TotalScore; return totalScore;
            }
        }

        private object lockObject = new object();
        private List<AForge.IntPoint> wrapPoints = new List<AForge.IntPoint>();
        private List<numberBlock> nbs = new List<numberBlock>();
        private List<emptyBlock> ebs = new List<emptyBlock>();
        private List<answerBlock> abs = new List<answerBlock>();
        private bool resRead = false, processingAsync = false, orWhite_ = false, inDFB, akUseAndForMOps = false;
        private Image camImg, recogImg;
        private string dbPath, sheetName, rawImgAddress, ansKeyTitle = "", ruta;
        private Size sbounds_;
        private int whiteLevel_, progress_ = 0, oq = 80, index_, akRandID = -1, akRandBlockID = -1, akPMark = 0, akNMark = 0, akTotalRandKeys = 0;


        /// <summary>
        /// A constructor for this class. 
        /// </summary>
        /// <param name="camImage_">Full Path where the target image is located</param>
        /// <param name="dbPath_">Full path of access database containing sheetinfo</param>
        /// <param name="sheetName_">Name of sheet from the database</param>
        public OMREngine(string camImage_, string dbPath_, string sheetName_, string ruta_)
        {
            rawImgAddress = camImage_;
            dbPath = dbPath_;
            sheetName = sheetName_;
            ruta = ruta_;
        }
        /// <summary>
        /// A constructor for this class.
        /// </summary>
        /// <param name="camImage_">Full Path where the target image is located</param>
        /// <param name="dbPath_">Full path of access database containing sheetinfo</param>
        /// <param name="sheetName_">Name of sheet from the database</param>
        /// <param name="answerKeyTitle">Complete name of answer key from the database which will be used to calculate scores</param>
        public OMREngine(string camImage_, string dbPath_, string sheetName_, string answerKeyTitle_, string ruta_)
        {
            rawImgAddress = camImage_;
            dbPath = dbPath_;
            sheetName = sheetName_;
            ansKeyTitle = answerKeyTitle_;
            ruta = ruta_;
        }
        /// <summary>
        /// In case user has subscribed to the progress changed event, this little helper will reduce some lines of codes.
        /// </summary>
        /// <param name="inc"></param>
        private void updateProgress(int inc)
        {
            progress_ += inc;
            progress_ = Math.Min(100, progress_);
            if (onAsyncProgressUpdated != null) onAsyncProgressUpdated(new ProgressEventArgs(progress_, Index));
        }
        //This Method actually hands over the task to OMR reader and also performs the post processing to compile a programmer friendly result
        private void processThread()
        {
            //create an omr reader member AQUI EMPRIEZA
            OMR.OpticalReader or = new OpticalReader();
            //subscribe to reader updates as well if inDepthUpdates are requested
            if (inDepthFeedBack)
                 or.onReaderProgressUpdate += Or_ProgressUpdateEvent;
            //set progress to 0
            //updateProgress(-progress_);
            //WEB if (inDepthFeedBack)
            //WEB    onInDepthProessUpdate(new ProgressUpdateEventArgs("Proceso iniciado"));
            //confirm that image exists
            //only needed if this is the first call to this method since the current class member was created in the caller.
            //this will simply load the cam image from the file
            if (camImg == null)
            {
                if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Leyendo imagen del archivo"));
                int i = CamImage.Width;
            }
            //WEB if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Lectura de archivo completa", new Bitmap(camImg), true));
            //cam image may take some time to load. Add 20 to the progress :p
            //updateProgress(20);
            //create an instance of bitmap to store the extracted image
            Bitmap extd = new Bitmap(1, 1);
            //create another instance of object to store image for object detection and other processing. This image will be scaled to match some reasonable values for object detection
            Bitmap extForEB = new Bitmap(1, 1);
            if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Obteniendo parametros de la base de datos"));
            //create another instance of image which will be used for Empty block counts. User may want to see those in original resolution as got from the cam image without scaling
            sbounds_ = (Size)dbOps.getSheetUniqueProperty(Enums.sheetProperties.SheetBounds, sheetName, dbPath);
            //progress update a little to tell that the thread is alive
            //updateProgress(2);
            //this try statement will try make the overall process immune to exceptions. All exceptions will be caught and sent to the process failed event
            //try
            {
                //create an empty instance of wrap points which will be calculated using the reader
                wrapPoints = new List<AForge.IntPoint>();
                
                try
                {
                    //lock (lockObject)
                    {
                        //WEB if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Incio detección de esquinas"));
                        //call the omr sheet extractor method. "False" in last argument specifies that the method will return only the wrap points on CamImage Scale
                        wrapPoints = (List<AForge.IntPoint>)or.ExtractOMRSheet(new Bitmap(CamImage), dbPath, sheetName,inDFB, whiteLevel_, orWhite_, true, ruta);
                    }
                }
                catch
                {
                    if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Fallos en los bloques de extraccción"));
                    //this exception is mostly caused due to wrong usage of OMR reader. Almost all of the exception causes have been either eliminated or handled
                    throw new Exception("La extracción de la imagen ha fallado. Por favor asegúrese de que ha utilizado el nombre de la hoja y encuesta correcta. Tambien lea \"¿Cómo conseguir una detección precisa?\". Codigo de error: 02080315");
                }
                //this took a lot of time. lets update the progress a lot more. lets say, 19. No, 18. . .
                //updateProgress(18);
                lock (lockObject)
                {
                    //WEB if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Bordes de imagen detectados"));
                    //now is the time to initialize the bitmap object which matches the scale on which wrap points were calculated
                    extd = OMR.helpers.ImageUtilities.applyWrap(wrapPoints, new Bitmap(camImg), ImageProcessScale, sbounds_);
                }
                //WEB if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Imagen separada en bordes", new Bitmap(extd), true));
                //keep the end user posted
                //updateProgress(2);
                //Image object is needed for remaining processes. why didn't we create an image on the first place? because creating a bitmap allocates totally new space on RAM. For one reason it is bad; memory consumption. But not as bad as Image which gave more errors back then due to complex programming under System.Drawing
                recogImg = (Image)extd;
                //create instances for Lists of Answer Blocks, Emptyblocks and Number Blocks
                abs = new List<answerBlock>();
                ebs = new List<emptyBlock>();
                nbs = new List<numberBlock>();

                //WEB if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Leyendo las propiedades de la hoja"));

                //start collecting data from database and keep the end user posted of what we are up to
                DataRowCollection abrows = dbOps.getBlockPropIds(Enums.sheetProperties.AnswerBlock, sheetName, dbPath);
                //updateProgress(1);
                DataRowCollection rbrows = dbOps.getBlockPropIds(Enums.sheetProperties.NumberBlock, sheetName, dbPath);
                //updateProgress(1);
                DataRowCollection ebrows = dbOps.getBlockPropIds(Enums.sheetProperties.EmptyBlock, sheetName, dbPath);
                //update the progress. what? where were we? lets make it obvious. 50
                //updateProgress(50 - progress_);
                //only initialize the full scale image for extraction of empty blocks if there are positive number of empty blocks
                if (ebrows.Count > 0)
                    extForEB = OMR.helpers.ImageUtilities.applyWrap(wrapPoints, new Bitmap(camImg), 1, sbounds_);
                //a test requirement of multi-thread suggests to lock this region
                lock (lockObject)
                {
                    for (int i = 0; i < abrows.Count; i++)
                    {
                        //add new member to the list of answer blocks
                        abs.Add(new answerBlock());
                        //Populate information about the block from DB
                        abs[i].PopulateProperties(dbPath, sheetName, i);
                        //these lines are similar. Using the information collected from above to extract images and binary marks on them
                        abs[i].CroppedImage = OMR.OpticalReader.cutOutBlockImage(recogImg, sbounds_, abs[i].gRectangle);
                        abs[i].CroppedAnswerLines = OMR.OpticalReader.SliceOsMarkBlock(abs[i].CroppedImage, abs[i].NumberOfLines);
                        
                        //rate the marks according to the dimensions collected from database and image extracted/cropped
                        for (int j = 0; j < abs[i].NumberOfLines; j++)
                        {
                            //initialize a temp array for current line of current block. (see current depth of 2 for loops)
                            bool[] tempSlice = OMR.OpticalReader.getBinaryMaskedScore(new Bitmap(abs[i].CroppedAnswerLines[j]), abs[i].Options, whiteLevel_, false);
                            //rate current mark of current line of current block. (see current depth of 3 for loops)
                            for (int k = 0; k < tempSlice.Length; k++)
                            {
                                abs[i].BinaryMaskedOMs[j, k] = tempSlice[k];
                            }
                        }
                        //Binary Masked OMs have been evaluated. Its time we start rating the answers for scores
                        //Check if the user has supplied with an answer key title
//                        if (ansKeyTitle.Length > 0)
//                        {
//                            DataTable dt = dbOps.rawSelectCommand(string.Format(
//                                "SELECT answerKeys.positiveMarking, -1, true, 1, null FROM answerKeys WHERE(((answerKeys.title) = \"{0}\"));",
//                                ansKeyTitle), dbPath).Tables[0];
                        /*                            DataTable dt = new DataTable();
                                                    dt.Columns.Add("positiveMarking");
                                                    dt.Columns.Add("negativeMarking");
                                                    dt.Columns.Add("andOpForMultiple");
                                                    dt.Columns.Add("totalRandKeys");
                                                    dt.Columns.Add("randKeyBlockInd");

                                                    DataRow row = dt.NewRow();
                                                    row["positiveMarking"] = abs.;
                                                    row["negativeMarking"] = -1;
                                                    row["andOpForMultiple"] = true;
                                                    row["totalRandKeys"] = 1;
                                                    row["randKeyBlockInd"] = null;
                                                    dt.Rows.Add(row);

                                                    akPMark = (int)dt.Rows[0][0];
                                                    akNMark = (int)dt.Rows[0][1];
                                                    akUseAndForMOps = (bool)dt.Rows[0][2];
                                                    akTotalRandKeys = (int)dt.Rows[0][3];
                                                    if (akTotalRandKeys > 1) // multi
                                                        akRandBlockID = (int)dt.Rows[0][4];

                            
                                                    akPMark = abs[i].Options ;
                                                    akNMark = -1;
                                                    akUseAndForMOps = true;
                                                    akTotalRandKeys = 1;
                                                    if (akTotalRandKeys > 1) // multi
                                                        akRandBlockID = 1;


                                                    abs[i].RateScores(dbPath, ansKeyTitle, akRandID, akPMark, akNMark, akUseAndForMOps, sheetName);
                                                }
                                                //total of 20 will be added to process for processing all the blocks
                                                //updateProgress(20 / abrows.Count);

                                                if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Opción multiple detectada", new Bitmap(abs[i].CroppedImage), false));
                        */
                    }
                }
                //same again
                //updateProgress(70 - progress_);
                //this for loop is almost similar to above loop. Read comments there first
                for (int i = 0; i < rbrows.Count; i++)
                {
                    //see, similar!
                    nbs.Add(new numberBlock());
                    nbs[i].PopulateProperties(dbPath, sheetName,  i);
                    //And still similar
                    nbs[i].CroppedImage = OMR.OpticalReader.cutOutBlockImage(recogImg, sbounds_, nbs[i].gRectangle);
                    nbs[i].CroppedDigitLines = OMR.OpticalReader.SliceOsMarkBlock(nbs[i].CroppedImage, nbs[i].Digits);
                    
                    for (int j = 0; j < nbs[i].Digits; j++)
                    {
                        //and still similar
                        bool[] tempSlice = OMR.OpticalReader.getBinaryMaskedScore(new Bitmap(nbs[i].CroppedDigitLines[j]), 10, whiteLevel_, false);
                        for (int k = 0; k < tempSlice.Length; k++)
                        {
                            nbs[i].BinaryMaskedOMs[j, k] = tempSlice[k];
                        }
                    }
                    //even this is similar
                    //updateProgress(20 / abrows.Count);
                    if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Bloque numerico detectado", new Bitmap(nbs[i].CroppedImage), false));
                }
                //updateProgress(90 - progress_);
                //this  block is repetition as well. Only that in involves lesser lines. because an empty block doesn't contain so much details
                for (int i = 0; i < ebrows.Count; i++)
                {
                    ebs.Add(new emptyBlock());
                    ebs[i].gRectangle= (RectangleF)dbOps.getProperty(Enums.sheetProperties.EmptyBlock,sheetName, dbPath, i);
                    ebs[i].CroppedImage = OMR.OpticalReader.cutOutBlockImage((Image)extForEB, sbounds_, ebs[i].gRectangle);

                    //updateProgress(8 / abrows.Count);
                    if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Bloque de texto detectado", new Bitmap(ebs[i].CroppedImage), false));
                }
                if (inDepthFeedBack) onInDepthProessUpdate(new ProgressUpdateEventArgs("Proceso terminado"));
                //updateProgress(99 - progress_);
                //and that concludes the process!!
                processingAsync = false;
                //mark the result Ready flag
                resRead = true;
                //update the end user on the latest update!
                if (onAsyncCompletion != null)
                    onAsyncCompletion(new CompletionEvenArgs(Index));
                //Cleanup all the mess we have created on the drive
                duplicateFile.cleanUpFiles(ruta);
                //updateProgress(100 - progress_);
            }
            //catch (Exception ex)
            //{
            //    //in case things didn't go well, we must inform the user about our hard luck. All the possible exceptions which could be handled, didn't cause this catch. Only those which are caused due to fault of end user or those which still remain in the engine even after much deliberations
            //    OnExtractionFailed(new ExtractionFailedArgs(ex, Index));
            //    //mark the result ready flag to be incomplete.
            //    resRead = false;
            //    //mark Sync process on going flag to be false.
            //    processingAsync = false;
            //    //keep the progress. This may let the tester have some clue where did the process snap. "MAY LET"!!
            //    //updateProgress(-progress_);

            //    //Cleanup all the mess we have created on the drive
            //    duplicateFile.cleanUpFiles();
            //}
            camImg.Dispose();
        }
        /// <summary>
        /// This event is used to extract in depth progress updates when the process is in OMR reader's part
        /// </summary>
        /// <param name="e"></param>
        private void Or_ProgressUpdateEvent(ProgressUpdateEventArgs e)
        {
            //we just need to forward these updates to the next level.
            onInDepthProessUpdate(e);
        }

        /// <summary>
        /// Once the class has been initialized and are editable properties are set, this method may be called to start a synchronous process
        /// </summary>
        /// <returns>Returns a bool indicating if the process completed successfully or not</returns>
        public bool StartProcess()
        {
            //need no explanation
            processThread();
            return ResultReady;
        }
        /// <summary>
        /// Similar to method StartPrcess()
        /// Once the class has been initialized and are editable properties are set, this method may be called to start an ASynchronous process
        /// </summary>
        public void StartProcessAsync()
        {
            if (!processingAsync)
            {
                processingAsync = false;
                int nil = CamImage.Width;
                processingAsync = true;
                resRead = false;
                System.Threading.Thread pt = new System.Threading.Thread(new System.Threading.ThreadStart(processThread));
                pt.Start();
            }
            else
                throw new Exception("Esta procesando una imagen");
        }
    }
}
