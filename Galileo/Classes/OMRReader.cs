using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Web;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using AForge.Math.Geometry;
using System.IO;
using System.Xml;
using OMR;
using OMR.helpers;

namespace OMR
{
    /// <summary>
    /// Event Args may contain a test message, an image or both for processing the message given by the event.
    /// </summary>
    public class ProgressUpdateEventArgs : EventArgs
    {
        string updateText = "";
        System.Drawing.Image img;
        bool isMainImage = false;

        /// <summary>
        /// Tells if the image contained within this event arg (if not NULL) represents the main image given for OMR reading at a particular processing state or a cropped smaller part of the original image.
        /// </summary>
        public bool IsMainImage { get { return isMainImage; } }
        /// <summary>
        /// The text explaining the update which caused this event to occur.
        /// </summary>
        public string UpdateText { get { return updateText; } }
        /// <summary>
        /// The Image (if not NULL) which is associated with the update which caused this event.
        /// </summary>
        public System.Drawing.Image LatestImage { get { return img; } }

        /// <summary>
        /// A onstructor for the event arg.
        /// </summary>
        /// <param name="updateText_">The text explaining the update which caused this event to occur.</param>
        public ProgressUpdateEventArgs(string updateText_)
        { updateText = updateText_; }
        /// <summary>
        /// A onstructor for the event arg.
        /// </summary>
        /// <param name="updateText_">The text explaining the update which caused this event to occur.</param>
        /// <param name="img_">The Image which is associated with the update which caused this event. If null, use the other constructor</param>
        /// <param name="isMainImage_"></param>
        public ProgressUpdateEventArgs(string updateText_, Bitmap img_, bool isMainImage_)
        { updateText = updateText_;  img = img_; isMainImage = isMainImage_; }
        /// <summary>
        /// A onstructor for the event arg.
        /// </summary>
        /// <param name="img_">The Image which is associated with the update which caused this event. If null, don't use this constructor</param></param>
        public ProgressUpdateEventArgs(Bitmap img_)
        { img = img_; }
    }

    public delegate void ProcessUpdatedHandler(ProgressUpdateEventArgs e);
    public class OpticalReader
    {
        /// <summary>
        /// Occurs on several steps during the processing of image. Event args should be processed in the events to see the message contained. The 
        /// </summary>
        public event ProcessUpdatedHandler onReaderProgressUpdate;
        /// <summary>
        /// this is used to keep track of time during the process. May only be used for debug pupose.
        /// </summary>
        TimeSpan ts = new TimeSpan();
        /// <summary>
        /// Extracts Image/extraction points from camera image (maximum 10MP). Type of output can be specified in "onlyExtractionPoints"
        /// if extraction points are required, they are mapped on the coordinate system of cameraImage
        /// </summary>
        /// <param name="cameraImage">Camera image, (not greater than 10MP) to be used for OMR detection</param>
        /// <param name="OMRSpecsSheetAddress">Address of database containing sheet information</param>
        /// <param name="sheet">Name of sheet to be used from database</param>
        /// <param name="whiteLevel">If white level is set to override the system value, this value will be used</param>
        /// <param name="overrideWhite">White level used for extraction can be overridden.</param>
        /// <param name="onlyExtractionPoints">Specifies if only extraction points are needed or the wrapped image.</param>
        /// <returns>Detected camera image or extraction points as specified in the arguments</returns>
        public object ExtractOMRSheet(Bitmap cameraImage, string OMRSpecsSheetAddress, string sheet, int whiteLevel, bool overrideWhite, bool onlyExtractionPoints, string ruta)
        {
            return ExtractOMRSheet(cameraImage, OMRSpecsSheetAddress, sheet, false, whiteLevel, overrideWhite, onlyExtractionPoints, ruta);
        }
     
        /// <summary>
        /// Extracts Image/extraction points from camera image (maximum 10MP). Type of output can be specified in "onlyExtractionPoints"
        /// if extraction points are required, they are mapped on the coordinate system of cameraImage
        /// </summary>
        /// <param name="cameraImage">Camera image, (not greater than 10MP) to be used for OMR detection</param>
        /// <param name="OMRSpecsSheetAddress">Address of database containing sheet information</param>
        /// <param name="sheet">Name of sheet to be used from database</param>
        /// <param name="onlyExtractionPoints">Specifies if only extraction points are needed or the whole wrapped image.</param>
        /// <returns>Detected camera image or extraction points as specified in the arguments</returns>
        public object ExtractOMRSheet(Bitmap cameraImage, string OMRSpecsSheetAddress, string sheet, bool onlyExtractionPoints, string ruta)
        {
            return ExtractOMRSheet(cameraImage, OMRSpecsSheetAddress, sheet, false,  0, false, onlyExtractionPoints, ruta);
        }

        /// <summary>
        /// Extracts Image/extraction points from camera image (maximum 10MP). Type of output can be specified in "onlyExtractionPoints"
        /// if extraction points are required, they are mapped on the coordinate system of cameraImage
        /// </summary>
        /// <param name="cameraImage">Camera image, (not greater than 10MP) to be used for OMR detection</param>
        /// <param name="OMRSpecsSheetAddress">Address of database containing sheet information</param>
        /// <param name="sheet">Name of sheet to be used from database</param>
        /// <param name="giveFB">specifies if feedback is needed. if true, feedback can be received by subscribing to ProgressUpdateEvent. Enabling feedback can hinder the performance</param>
        /// <param name="onlyExtractionPoints">Specifies if only extraction points are needed or the whole wrapped image.</param>
        /// <returns>Detected camera image or extraction points as specified in the arguments</returns>
        public object ExtractOMRSheet(Bitmap cameraImage, string OMRSpecsSheetAddress, string sheet, bool giveFB, bool onlyExtractionPoints, string ruta)
        {
            return ExtractOMRSheet(cameraImage, OMRSpecsSheetAddress, sheet, giveFB ,  0, false, onlyExtractionPoints, ruta);
        }
        /// <summary>
        /// Very raw usage override. Should be used with care.
        /// Extracts image and gives "In-Process" feebback on ProgressUpdateEvent
        /// </summary>
        /// <param name="originalImage">Image directly from cam, no processing, cropping or skewing should have been done</param>
        /// <param name="OMRSpecsSheetAddress">sheet database addrss</param>
        /// <param name="sheet">string name of sheet from database</param>
        /// <param name="giveFB">specifies if feedback is needed. if true, feedback can be received by subscribing to ProgressUpdateEvent. Enabling feedback can hinder the performance</param>
        /// <param name="whiteLevel">If white level is set to override the system value, this value will be used</param>
        /// <param name="overrideWhite">White level used for extraction can be overridden.</param>
        /// <param name="onlyExtractionPoints">Specifies if only extraction points are needed or the wrapped image.</param>
        /// <returns>Detected camera image or extraction points as specified in the arguments.</returns>
        public object ExtractOMRSheet(Bitmap originalImage, string OMRSpecsSheetAddress,  string sheet, bool giveFB, int whiteLevel, bool overrideWhite, bool onlyExtractionPoints, string ruta)
        {
            ts = new TimeSpan(DateTime.Now.Ticks);
            if (giveFB)
                onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Incia extracción")));
            System.Drawing.Image flattened = (System.Drawing.Image)prepareForObjectDetection(originalImage, whiteLevel, overrideWhite);
            if (giveFB)
            {
                onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Incia con la lectura "), new Bitmap(flattened), true));
            }
            return ExtractPaperFromPrepapred(new Bitmap(flattened), originalImage, OMRSpecsSheetAddress, giveFB, sheet, onlyExtractionPoints, ruta);
        }

        /// <summary>
        /// Very very raw usage override. Should be used with care.
        /// Extracts image and gives "In-Process" feebback on ProgressUpdateEvent
        /// </summary>
        /// <param name="PreparedImage">Color corrected Image.</param>
        /// <param name="originalImage">Image directly from cam, no processing, cropping or skewing should have been done</param>
        /// <param name="OMRSpecsSheetAddress">sheet database addrss</param>
        /// <param name="giveFB">specifies if feedback is needed. if true, feedback can be received by subscribing to ProgressUpdateEvent. Enabling feedback can hinder the performance.</param>
        /// <param name="sheet">string name of sheet from database</param>
        /// <param name="onlyExtractionPoints">Specifies if only extraction points are needed or the wrapped image.</param>
        /// <returns>Detected camera image or extraction points as specified in the arguments.</returns>
        private object ExtractPaperFromPrepapred(Bitmap PreparedImage, Bitmap originalImage, string OMRSpecsSheetAddress, bool giveFB, string sheet, bool onlyExtractionPoints, string ruta)
        {
            //The image here will be high contrast, inverted. Flatten Image does this for us. We are hunting for markings and ink on a white paper. White, in logic, means 1. means presence of an object. Which, in our case is background paper, which is converse of an object
            //which also is supposed to be logical 1. means white, means brighter. So, we have inverted the image.
                     
            if (giveFB) // means if the raw feedback is required                 
            {
                //tell the caller that process has started.
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Imagen preparada para la extraccción"), PreparedImage, true));
            }
            //Step 1
            // lock the image. Otherwise, Bitmap itself takes much time to be processed
            BitmapData bitmapData = PreparedImage.LockBits(
                new Rectangle(0, 0, PreparedImage.Width, PreparedImage.Height),
                ImageLockMode.ReadWrite, PreparedImage.PixelFormat);

            // step 2 - locating objects
            //The only things we Need to find in the image are the four corner marks. Once the marks have been identified, and their centres calculated, we will Wrap the polygon to a rectangle.
            //this rectangle will be transformed to the aspect ratio of original sheet as specified during the creation of OMR sheet used in this image.
            //
            //PLEASE NOTE:!!! No detection will be performed for any other object or marks on the sheet. We won't need to! Once the cam image has been transformed into similar aspect Ratio as that of
            //the original OMR sheet, the objects and OMR marks block will also become equilent to the original sheet. We would only need to BLINDLY, crop the cam image at the pre-specified locations and we will have our block cut out
            //
            //So, lets get started
            //
            //Initialize a blob counter object. this object will be used to find objects on the bitmap
            BlobCounter blobCounter = new BlobCounter();
            
            //As we are hunting for small corner marks, we already can estimate a lot of things about them even without knowing what kind of a sheet was used.
            //Like,
            //specify the first safe estimate of minimum size possible of the corner block we are searching for. OMR sheet (hopefully) will never be bigger than an A4 Sized paper.
            //this is just the first estimate, we will make it more precise in the coming steps.
            int minBlobWidHei = (int)(0.001 * PreparedImage.Width);
            //we need the blobs filtered with these parametrs
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = minBlobWidHei;  // both these variables have to be given when calling the
            blobCounter.MinWidth = minBlobWidHei;   // method

            if (giveFB)
            {
                //Give a little update on the progress
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob inicia extracción Started. Blob Tamaño Ratio filtro en altura, ancho= " + minBlobWidHei.ToString()));
            }
            //lets call very low lights a line threshhold. this makes it possible to work will low resolution images which have anti-alising turned on.
            //The image here will already be high contrast, remember.
            blobCounter.BackgroundThreshold = Color.FromArgb(10, 10, 10);
            //let the counter start the job synchronously
            blobCounter.ProcessImage(bitmapData);
            //We are done with the image. Lets unlock it.
            PreparedImage.UnlockBits(bitmapData);

            //lets get the coordinates and sizes of the detected blobs
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            //this will transfer the images to the blob objects. Its a requirement of the library
            Blob[] blobs = blobCounter.GetObjects(PreparedImage, false);

            //ass we decided earlier, we need to be more precceise about our estimates about the sizes of target objects
            // this helps filtering out too small and too larger blobs depending upon the size of image.
            //We are blind about what ratio does the target have with the original sheet. This has to be asked from the creator of this sheet. Lets check the user manual i.e. The Access DB created with the sheet
            //Minimum blob to sheet ratio. contains tolerance of upto 40%
            double minbr = (double)OMR.helpers.dbOps.getSheetUniqueProperty(OMR.Enums.sheetProperties.minBlobSizeRatio, sheet, OMRSpecsSheetAddress);
            //maximum blob to sheet ratio. contains tolerance of upto 40%
            double maxbr = (double)OMR.helpers.dbOps.getSheetUniqueProperty(OMR.Enums.sheetProperties.maxBlobSizeRatio, sheet, OMRSpecsSheetAddress);

            // Store sheet corner locations in this list (in any order) . . . (if anyone is detected )
            List<IntPoint> quad = new List<IntPoint>(); 
            
            //separate those which are the most suspected.
            List<Blob> suspectBlobs = new List<Blob>();
            if (giveFB)
            {
                //Give a little update on the progress
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob extraido. Tamaño = " + blobs.Length.ToString()));
            }
            //now, check each blob and apply a more precise size and location filter
            for (int i = 0; i < blobs.Length; i++)
            {
                //area of blob
                int a = blobs[i].Image.Width * blobs[i].Image.Height;
                //in VS 2012 and below, image debugger worked. so this line was usefull
                System.Drawing.Image imgt = blobs[i].Image.ToManagedImage();
                if (
                    //only if it doesn't have insanely wrong size
                    ((double)a) / ((double)PreparedImage.Width * PreparedImage.Height) > minbr &&
                        ((double)a) / ((double)PreparedImage.Width * PreparedImage.Height) < maxbr
                        &&
                    //And it doesn't have a total inappropriate ASpect Ratio
                        ((double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height < 1.3 &&
                            (double)(double)blobs[i].Rectangle.Width / (double)blobs[i].Rectangle.Height > .7)
                    )// filters out blobs having insanely wrong aspect size and )
                {
                    suspectBlobs.Add(blobs[i]);
                    //in VS 2012 and below, image debugger worked. so this line was usefull
                    //System.Drawing.Image imgt2 = blobs[i].Image.ToManagedImage();
                }
                imgt.Dispose();
            }
            if (giveFB)
            {
                //a little more update
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob filtrado por tamaño incorrecto, posición y relación de aspecto. Tamaño supuesto = " + suspectBlobs.Count.ToString()));
            }
            //forget about the old blobs. do the remaing filteration only on the remaining
            blobs = suspectBlobs.ToArray();

            //Detection of paper lies within the presence of crossmark printed on the corneres of printed sheet.
            //edge marks are detected by comparison of detected blobs to a original sample
            // First, detect left edge.
            //Create duplicate is used for mutlithreading. 
            //compImg = Comparison Image
            System.Drawing.Image compImg = System.Drawing.Image.FromFile(duplicateFile.createDuplicate("lc.jpg", ruta));
            

            // lc.jpg = Mirrored image sample as located on the corner of printed sheet
            UnmanagedImage compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);

            //lets create a graphics object in case the process fails to extract in future and we would want to draw some diagnosis marks on the image to be shown to the caller.
            Graphics g = Graphics.FromImage(PreparedImage);
            //A yellow Pen. How gentle!
            Pen yellowPen = new Pen(Color.Yellow, 2);
            //A red one. Red means danger here.
            Pen redPen = new Pen(Color.Red, 2);

            if (giveFB)
            {
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs("Inicia detección bordes izquierdos"));
            }

            int crsr = 0;
            foreach (Blob blob in blobs)
            {
                if (blob.Rectangle.X < (PreparedImage.Width) / 2) // filters out blobs located at very different position
                {
                    //compUMImg = Comparison Unmanaged Image
                    //resize the comparison image to match the size of detected suspect
                    compUMImg = UnmanagedImage.FromManagedImage(ImageUtilities.ResizeImage(compImg, blob.Rectangle.Width, blob.Rectangle.Height));
                    //this method just does a pixel to to pixel comparison of two same sized images. If the images are similar, they must also have very good pixel match, regardless of the shape.
                    //this may not apply to some images, but the type of our suspect blob is compatible with this hypothsis
                    if (isSame(blob.Image, compUMImg))
                    {
                        // to give the feedback, draw a rectangle around the blob. Don't send the main object back to the user. It won't be as usable as sending the blob image itself.
                        if (giveFB)
                        {
                            g.DrawRectangle(yellowPen, blob.Rectangle);
                            Bitmap debugBMP = blob.Image.ToManagedImage();
                            if (onReaderProgressUpdate != null)
                                onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob detectada en la esquina", debugBMP, false)); //aqui uno
                        }
                        //add the corner mark to the array. Arranement is not significant. We will deal with that later. Aqui dos
                        quad.Add(new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y));
                    }
                    else if (giveFB)                        
                    {
                        //in case a blob didn't match the image
                        // to give the feedback, draw a rectangle around the blob. Don't send the main object back to the user. It won't be as usable as sending the blob image itself.
                        g.DrawRectangle(redPen, blob.Rectangle);
                        Bitmap debugBMP = blob.Image.ToManagedImage();
                        if (onReaderProgressUpdate != null)
                            onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob detectada en la esquina", debugBMP, false));
                    }
                }
                crsr++;
            }


            //It is when we start to arrange the blobs in right sequence to conform with the rest of the algorithm
            // Sort out the list in right sequence, UpperLeft,LowerLeft,LowerRight,upperRight

            //we no assume that only 4 blobs will be added to the quad. Because there is no filteration we can perform now. to detect the left edge.
            //if this assumption is wrong and there are lesser or more blobs, we can  do anything but to declare an extraction failure.
            //2ndly, with this assumption, first two blobs must have bee detected as a left edge corner mark. there X component is not significant, only Y is
            //to  conform with the sequence we decided, do the following check
            //check count only to avoid an indexOutOfRangeException
            if (quad.Count > 1)
            {

                if (giveFB)
                {
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Detección de esquina izquierda finalizada")));
                }
                if (quad[0].Y > quad[1].Y)
                {
                    IntPoint tp = quad[0];
                    quad[0] = quad[1];
                    quad[1] = tp;
                }
            }
            else
            {
                if (giveFB)
                {
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Fallo en detección izquierda. Tamaño supuesto = " + quad.Count.ToString())));
                }
            }
            
            //do the same routine for right side corner marks
            //compUMImg = comparison Unmannaged Image, remember?
            compImg = System.Drawing.Image.FromFile(duplicateFile.createDuplicate("rc.jpg", ruta));
            compUMImg = UnmanagedImage.FromManagedImage((Bitmap)compImg);
            if (giveFB)
            {
                if (onReaderProgressUpdate != null)
                    onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Inicia detección bordes derechos")));// jusst like left edge detection
            }

            //this is same as what we did for left edge
            foreach (Blob blob in blobs)
            {
                if (
                    blob.Rectangle.X > (PreparedImage.Width * 3) / 4)
                {

                    compUMImg = UnmanagedImage.FromManagedImage(ImageUtilities.ResizeImage(compImg, blob.Rectangle.Width, blob.Rectangle.Height));
                    if (isSame(blob.Image, compUMImg))
                    {
                        if (giveFB)
                        {
                            g.DrawRectangle(yellowPen, blob.Rectangle);
                            Bitmap debugBMP = blob.Image.ToManagedImage();
                            onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob detectada en la esquina", debugBMP, false));
                        }
                        quad.Add(new IntPoint((int)blob.CenterOfGravity.X, (int)blob.CenterOfGravity.Y));
                    }
                    else if (giveFB)
                    {
                        g.DrawRectangle(redPen, blob.Rectangle);
                        Bitmap debugBMP = blob.Image.ToManagedImage();
                        onReaderProgressUpdate(new ProgressUpdateEventArgs("Blob detectada en la esquina", debugBMP, false));
                    }
                }
            }

            if (giveFB)
            {
                if (onReaderProgressUpdate != null)
                onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Detección esquina derecha finalizada")));
            }
            //2 more have been added to quad (persumably)
            //here, the lower blob needs to be first
            if (quad.Count > 3)
            {
                if (giveFB)
                {
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Detección esquina derecha finalizada")));
                }
                if (quad[2].Y < quad[3].Y)
                {
                    IntPoint tp = quad[2];
                    quad[2] = quad[3];
                    quad[3] = tp;
                }
            }
            else
            {
                if (giveFB)
                {
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Fallo en detección derecha. Tamaño supuesto = " + quad.Count.ToString())));
                }
            }
            //close the graphics object
            redPen.Dispose();
            yellowPen.Dispose();
            g.Flush();
            g.Dispose();
             compImg.Dispose();
             compUMImg.Dispose();
            //For the last time, check if wrong blobs pretended to our corner marks...
            
            
            if (quad.Count == 4)// means, //if our assumption of 4 blobs was true
            {
                if (((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) < .75 ||
                    ((double)quad[1].Y - (double)quad[0].Y) / ((double)quad[2].Y - (double)quad[3].Y) > 1.25)
                    quad.Clear(); // clear if, both edges have insanely wrong lengths
                else if (quad[0].X > PreparedImage.Width / 2 || quad[1].X > PreparedImage.Width / 2 || quad[2].X < PreparedImage.Width / 2 || quad[3].X < PreparedImage.Width / 2)
                    quad.Clear(); // clear if, sides appear to be "wrong sided"
            }
            if (quad.Count != 4)// sheet not detected..
            {
                if (giveFB)
                {
                    //prepared image has been marked with rectangles
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Falla en la deteccón de la hoja"), PreparedImage, true));
                }
                //confirm disposal
                else { PreparedImage = null; }
                throw new Exception("Sheet Extraction Failed.");
            }
            else // sheet found
            {
                if (giveFB)
                {
                    if (onReaderProgressUpdate != null)
                        onReaderProgressUpdate(new ProgressUpdateEventArgs(makeTimeStamp("Hoja extraida"), PreparedImage, true));
                }
                //sort the edges for wrap operation. Its required by the Aforge Wrap Operation
                IntPoint tp2 = quad[3];
                quad[3] = quad[1];
                quad[1] = tp2;

                //if the caller onyl required these points, return, else, do the warp as well.
                if (onlyExtractionPoints)
                    return quad;

                //from Afroge.
                QuadrilateralTransformation wrap = new QuadrilateralTransformation(quad);
                wrap.UseInterpolation = false; //perspective wrap only, no binary.
                //The new size can be inquired from the DB
                Size sr = (Size)OMR.helpers.dbOps.getSheetUniqueProperty(Enums.sheetProperties.SheetBounds, sheet, OMRSpecsSheetAddress);
                sr = new Size(sr.Width * 2, sr.Height * 2);
                wrap.AutomaticSizeCalculaton = false;
                wrap.NewWidth = sr.Width;
                wrap.NewHeight = sr.Height;
                return wrap.Apply(originalImage); // wrap and return
            }
        }

        /// <summary>
        /// This method is required to pre-process images for extraction.
        /// </summary>
        /// <param name="bmpOriginalImage">Image to use for processing</param>
        /// <param name="whiteLevel">[0,255] if white override is forced, White level, can be got by using methods in OMR.helpers.ImageUtils</param>
        /// <param name="overrideWhite">if true, whiteLevel will be used otherwise, algorithm will try and find a white level itself.</param>
        /// <returns>Returns a pre-processed image which can be used directly for sheet extraction</returns>
        public Bitmap prepareForObjectDetection(Bitmap bmpOriginalImage, int whiteLevel, bool overrideWhite)
        {
            //The image wehich is required for processing should be be in high contrast and inverted. We will be hunting for markings and ink on a white paper. White, in logic, means 1. means presence of an object. Which, in our case is background paper, which is converse of an object
            //which also is supposed to be logical 1. means white, means brighter. So, we need to return the inverted image

            //step 0 - Prepaer a clone. Don't ruin the original
            Bitmap bmp = (Bitmap)bmpOriginalImage.Clone();
            // step 1 - turn background to White

            // Color filter Colors all pixels which lie in a color range. It can also color the outside region with black
            ColorFiltering colorFilter = new ColorFiltering();

            //white level should be obtained to represent the most of paper color present in the cam image
            //ImageUtilities.getWhiteLevel can do this for us.
            int white = whiteLevel;
            if (!overrideWhite)
                white = ImageUtilities.getWhiteLevel(bmp);
            //white/3 should compensate for a pixel as less bright as 1/3 of original white level
            colorFilter.Red = new IntRange(0, white/3);
            colorFilter.Green = new IntRange(0, white/3);
            colorFilter.Blue = new IntRange(0, white/3);
            colorFilter.FillOutsideRange = true;
            //we need to fill it with white
            colorFilter.FillColor = new RGB(Color.White);
            colorFilter.ApplyInPlace(bmp);
            
            //we didn't specifically needed to do this. but apparanlty, the invert filter we need, cannot be performed to the type of image we currently have.
            //Its something Aforge has dragged us into.
            //Invert Method cannot be applied to indexed image. extract channel converts it to monochromatic.
            ExtractChannel extract_channel = new ExtractChannel(0);
            bmp = extract_channel.Apply(bmp);

            Invert invert = new Invert();
            invert.ApplyInPlace(bmp);

            Threshold threshholdFilter = new Threshold(10);
            threshholdFilter.ApplyInPlace(bmp);
            
            bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);

            return bmp;
        }
        
        /// <summary>
        /// Roughly compares two images on pixel matching method. Both images need to be of same dimensions
        /// Please note, this method doesn't work correctly if sequence of images in parameters is reversed.
        /// </summary>
        /// <param name="imageToBeCompared">As the name suggests</param>
        /// <param name="referenceImage">As the name suggests</param>
        /// <returns>returns true if more than 50 percent of pixels match otherwise, false.</returns>
        private bool isSame(UnmanagedImage imageToBeCompared, UnmanagedImage referenceImage)
        {
            AForge.Imaging.Filters.Grayscale gsf = new Grayscale(0.2989, 0.5870, 0.1140);
            imageToBeCompared = gsf.Apply(imageToBeCompared);
            referenceImage = gsf.Apply(referenceImage);

/*  Para detectar las esquinas
  Bitmap bmp1 = imageToBeCompared.ToManagedImage(), bmp2 = referenceImage.ToManagedImage();

            Bitmap bmpO = imageToBeCompared.ToManagedImage();
            bmp1.Save(".\\cero.jpg", System.Drawing.Imaging.ImageFormat.Jpeg); ;
            Bitmap bmpOz = referenceImage.ToManagedImage();
            bmp1.Save(".\\zeta.jpg", System.Drawing.Imaging.ImageFormat.Jpeg); ;
*/

            //var count  means the count of those which succeeded the match
            //var tCount means the count of those which were considered worthy of counting
            int count = 0, tcount = referenceImage.Width * referenceImage.Height;
            //run a simple bi-dimensional loop with variables x and y.
            for (int y = 0; y < imageToBeCompared.Height; y++)
                for (int x = 0; x < imageToBeCompared.Width; x++)
                {
                    Color c1 = imageToBeCompared.GetPixel(x, y), c2 = referenceImage.GetPixel(x, y);
                    
                    int a1 = (c1.R + c1.G + c1.B) / 3; // take Arithmetic mean for no reason
                    int a2 = (c2.R + c2.G + c2.B) / 3; // take Arithmetic mean for no reason
                    
                    if ((a1 < 127) == (a2 < 127)) // if both look dark or both look bright
                    {
                        if (a2 > 127) // if both look bright (a1 will also be > 127)
                            count++;    //both match. consider it a match
                        else // both a1, a2 will be <= 127. . .don't consider there as a competition pair
                            tcount--;
                    }
                    else
                        //if doesn't match, don't leave it here, deduct score for mismatch
                        count--;
                }
            //both count and tCount and count should be almost equal for a perfect match
            count += tcount; // means count should have been doubled by adding it
            count /= 2;     //means, count should have been brought back to first value. 
            // with the above assumption, in which count and tcount were the same,
            bool returnValue = (count * 100) / tcount >= 75; //ARPLH originalmente 50
            
           
            return returnValue;
        }

        /// <summary>
        /// private method, kindly figure out the usage by yourself
        /// </summary>
        /// <param name="str"></param>
        /// <param name="textBox1"></param>
        private string makeTimeStamp(string str)
        {
//            string ans = (str + ": " + (new TimeSpan(DateTime.Now.Ticks) - ts).TotalSeconds + "\r\n");
//            ts = new TimeSpan(DateTime.Now.Ticks);
            string ans = "";
            return ans;
        }

        /// <summary>
        /// Crops out a block image specified by block rectangle from image.
        /// Coordinate system is created by manipulating sBounds and block Rectangle
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="sBounds">bounds of sheet according to block Rectangle</param>
        /// <param name="block">block rectangle for cutting out image</param>
        /// <returns>Cutout Block</returns>
        public static System.Drawing.Image cutOutBlockImage(System.Drawing.Image image, Size sBounds, RectangleF block)
        {
            return cutOutBlockImage(image, sBounds, new Rectangle(
                (int)Math.Round(block.X),
                (int)Math.Round(block.Y),
                (int)Math.Round(block.Width),
                (int)Math.Round(block.Height)));
        }

        /// <summary>
        /// Crops out a block image specified by block rectangle from image.
        /// Coordinate system is created by manipulating sBounds and block Rectangle
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="sBounds">bounds of sheet according to block Rectangle</param>
        /// <param name="block">block rectangle for cutting out image</param>
        /// <returns>Cutout Block</returns>
        public static System.Drawing.Image cutOutBlockImage(System.Drawing.Image image, Size sBounds, Rectangle block)
        {
//            Bitmap bmp1 = new Bitmap(image);
//           bmp1.Save(HttpContext.Current.Server.MapPath("cortada.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg); 

            double xScale = (double)image.Width / sBounds.Width;
            double yScale = (double)image.Height / sBounds.Height;

            sBounds = new Size(
                (int)Math.Round((double)sBounds.Width * xScale),
                (int)Math.Round((double)sBounds.Height * yScale));
            block = new Rectangle(
                (int)Math.Round((double)block.X * xScale ),
                (int)Math.Round((double)block.Y * yScale * 0.915),
                (int)Math.Round((double)block.Width * xScale),
                (int)Math.Round((double)block.Height * yScale * 0.915)); //* (1/ 0.915) en block.y  y en block.height

            List<IntPoint> quad = new List<IntPoint>();
            quad.Add(new IntPoint(block.X, block.Y));
            quad.Add(new IntPoint(block.X + block.Width, block.Y));
            quad.Add(new IntPoint(block.X + block.Width, block.Y + block.Height));
            quad.Add(new IntPoint(block.X, block.Y + block.Height));
            var sr = block.Size;
            QuadrilateralTransformation wrap = new QuadrilateralTransformation(quad);
            wrap.UseInterpolation = false; //perspective wrap only, no binary.
            wrap.AutomaticSizeCalculaton = false;
            wrap.NewWidth = sr.Width;
            wrap.NewHeight = sr.Height;
            System.Drawing.Image img = (System.Drawing.Image)wrap.Apply(new Bitmap(image)); // wrap
//           img.Save(HttpContext.Current.Server.MapPath("recortada"+(block.X+block.Y).ToString()+".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg); 
            return img;
        }

        /// <summary>
        /// Analysis marking on a Sliced block specified by the parameters.
        /// Slice must be a single horizontal row of OMs
        /// </summary>
        /// <param name="slice">Slice which contains markings</param>
        /// <param name="OMCount">Number of marks possible horizontally</param>
        /// <param name="whiteLevel">If white level is set to override the system value, this value will be used</param>
        /// <param name="overrideWhite">White level used for extraction can be overridden.</param>
        /// <returns>bool array containg spefying if a blank is marked (true) or not (false)</returns>
        public static bool[] getBinaryMaskedScore(Bitmap slice, int OMCount, int sheetWhite, bool overrideWhite)
        {
            //make grayscale  //hasta aqui
            Grayscale gsf = new Grayscale(0.2989, 0.5870, 0.1140);
            slice = gsf.Apply(slice);
            //initialize members
            Rectangle[] cropRects = new Rectangle[OMCount];
            Bitmap[] marks = new Bitmap[OMCount];
            bool[] answers = new bool[OMCount];
            //sub-devide line into options count (horizontal only)
            for (int i = 0; i < OMCount; i++)
            {
                cropRects[i] = new Rectangle(i * slice.Width / OMCount, 0, slice.Width / OMCount, slice.Height);
            }
            //the user specified that the line is actually multiple o Marks. lets crop them
            for (int i = 0; i < OMCount; i++)
            {
                marks[i] = new Bitmap(cropRects[i].Width, cropRects[i].Height);
                using (Graphics g = Graphics.FromImage(marks[i]))
                {
                    g.DrawImage(slice, new Rectangle(0, 0, marks[i].Width, marks[i].Height),
                                     cropRects[i],
                                     GraphicsUnit.Pixel);
                }
//                marks[i].Save(i.ToString()+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg); //esta rutina permite ver las marcas
            }
            if (!overrideWhite)
                sheetWhite = OMR.helpers.ImageUtilities.getWhiteLevel(new Bitmap(slice));
            for (int i = 0; i < OMCount; i++)
            {
                int temp = 0;
                temp = Math.Max(OMR.helpers.ImageUtilities.avgColor((System.Drawing.Image)marks[i], sheetWhite / 2), 0);
                temp = (int)Math.Round(100 - (double)temp / (double)255 * 100D);
                //the lesser the temp var, the more sensative the detection becomes. This Value can be calibrated with further experimentation
                answers[i] = temp > 20;
            }
            return answers;
        }
       
        /// <summary>
        /// Simply slices the given block horizonttally in specified number of blocks
        /// </summary>
        /// <param name="croppedBlock">Image which needs to be sliced</param>
        /// <param name="slices">Number of horizontal slices</param>
        /// <returns>Bitmap array which contains individual slices</returns>
        public static Bitmap[] SliceOsMarkBlock(System.Drawing.Image croppedBlock, int slices)
        {
            List<Rectangle> cropRects = new List<Rectangle>();
            for (int i = 0; i < slices; i++)
            {
                cropRects.Add(new Rectangle(
                    0,
                    (int)Math.Round((double)i * (double)croppedBlock.Height / (double)slices), 
                    croppedBlock.Width, (int)Math.Round((double)croppedBlock.Height/(double)slices)));
            }
            Bitmap[] bmps = new Bitmap[slices];
            Bitmap src = (Bitmap)croppedBlock;
            for (int i = 0; i < cropRects.Count; i++)
            {
                Rectangle cropRect = cropRects[i];
                bmps[i] = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(bmps[i]))
                {
                    g.DrawImage(src, new Rectangle(0, 0, bmps[i].Width, bmps[i].Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }
            }
            return bmps;
        }
    }
}
