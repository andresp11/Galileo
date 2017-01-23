using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using OMR;
using System.Linq;
using System.Text;


namespace OMR.helpers
{
    /// <summary>
    /// Provides various image untilities, such as high quality resizing and the ability to save a JPEG.
    /// </summary>
    public static class ImageUtilities
    {
        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        private static Dictionary<string, ImageCodecInfo> encoders = null;

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand
            get
            {
                //if the quick lookup isn't initialised, initialise it
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                //if there are no codecs, try loading them
                if (encoders.Count == 0)
                {
                    //get all the codecs
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        //add each codec to the quick lookup
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                //return the lookup
                return encoders;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid value was entered for image quality.
        /// </exception>
        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, jpegCodec, encoderParams);
        }

        /// <summary>
        /// returns histogram of approx. grey Image
        /// </summary>
        /// <param name="img">raw image</param>
        /// <returns>histogram of image</returns>
        public static int[] histogram(System.Drawing.Image img)
        {
            
            int[] ans = new int[256];
            
            AForge.Imaging.UnmanagedImage umimg = AForge.Imaging.UnmanagedImage.FromManagedImage(new Bitmap(img));

            for (int y = 0; y < umimg.Height; y++)
                for (int x = 0; x < umimg.Width; x++)
                {
                    Color c = umimg.GetPixel(x, y);
                    ans[(c.R + c.G + c.B) / 3]++;
                }
            int max = ans.Max();
            for (int i = 0; i < 256; i++)
            {
                ans[i] = (int)Math.Round((double)ans[i] / (double)max * 255D, 0);
            }
            return ans;
        }
        public static int[] smoothHistogram(int[] histogram)
        {
            int[] ans = new int[256];
            for (int i = 0; i < 256; i++)
            {
                ans[i] = histogram[i];
            }
            for (int r = 0; r < 50; r++)
            {
                for (int i = 1; i < 255; i++)
                {
                    ans[i] = (histogram[i + 1] + histogram[i - 1] + histogram[i]) / 3;
                }
                ans[0] = Math.Min(Math.Max(histogram[1] - (histogram[2] - histogram[1]), 0), 255);
                ans[255] = Math.Min(Math.Max(histogram[254] - (histogram[253] - histogram[254]), 0), 255);
                for (int i = 0; i < 256; i++)
                {
                    histogram[i] = ans[i];
                }
            }
            return ans;
        }
        /// <summary>
        /// approximates the white level from histogram
        /// </summary>
        /// <param name="histogram"></param>
        /// <returns>returns white level to be used with fill method</returns>
        public static int getWhiteLevel(int[] histog)
        {
            int[] histo = smoothHistogram(histog);
            int[] maximas = maximasOfHistoGram(histo);
            return maximas[maximas.Length - 1];
        }
        public static int avgColor(Image img, int forceZeroBelow)
        {
            long total = 0;
            AForge.Imaging.UnmanagedImage umimg = AForge.Imaging.UnmanagedImage.FromManagedImage(new Bitmap(img));
            for (int y = 0; y < umimg.Height; y++)
                for (int x = 0; x < umimg.Width; x++)
                {
                    Color c = umimg.GetPixel(x, y);
                    total += forceZeroBelow > ((c.R + c.G + c.B) / 3) ? 0 : 255;
                }
            return (int)((long)total / ((long)umimg.Width * (long)umimg.Height));
        }
        public static int getDarkLevel(Bitmap bmp)
        {
            int[] histo = smoothHistogram(histogram(bmp));
            int[] maximas = maximasOfHistoGram(histo);
            try
            {
                return maximas[0];
            }
            catch { }
            if (maximas.Length == 0)
                return 125;
            else if (maximas.Length == 1)
                return maximas[0];
            else if (maximas.Length == 2)
                return (int)(maximas[0] * 0.7 + maximas[1] * 0.3);

            else
                return (int)(maximas[maximas.Length - 1] * 0.3 + maximas[0] * 0.7);
        }
        public static int getWhiteLevel(Bitmap bmp)
        {
            int[] histo = smoothHistogram(histogram(bmp));
            int[] maximas = maximasOfHistoGram(histo);
            try
            {
                return Math.Min((int)(maximas[maximas.Length - 1] * 0.8 + maximas[maximas.Length - 2] * 0.2), 255);
            }
            catch { }
            if (maximas.Length == 0)
                return 127;
            else if (maximas.Length == 1)
                return maximas[maximas.Length - 1];
            throw new Exception("There is some unexpected bug in the algorithm. Kindly contact the developer with the error code: 070315");
        }
        public static Bitmap applyWrap(List<AForge.IntPoint> quad, Bitmap originalIage, double scale, Size sheetBounds)
        {
            double ratio = Math.Max((double)originalIage.Width / (double)sheetBounds.Width, (double)originalIage.Height / (double)sheetBounds.Height);
            ratio *= scale;
            AForge.Imaging.Filters.QuadrilateralTransformation wrap = new AForge.Imaging.Filters.QuadrilateralTransformation(quad);
            wrap.UseInterpolation = false; //perspective wrap only, no binary.
            Size sr = sheetBounds;
            sr = new Size(
                (int)Math.Round((double)sr.Width * ratio),
                (int)Math.Round((double)sr.Height * ratio)
                );
            wrap.AutomaticSizeCalculaton = false;
            wrap.NewWidth = sr.Width;
            wrap.NewHeight = sr.Height;
            return wrap.Apply(originalIage); // wrap 
        }
        static public Image drawHistogram(int[] histogram)
        {
            Bitmap bmp = new Bitmap(256, 256);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    bmp.SetPixel(i, 255 - j, (j <= histogram[i]) ? Color.Black:Color.White);
                }
            }
            return (Image)bmp;
        }
        static int[] maximasOfHistoGram(int[] histo)
        {
            List<int> maximas = new List<int>();
            int[] dOfF = new int[256], d2OfF = new int[256];

            for (int i = 0; i < 255; i++)
            {
                if (histo[i + 1] < histo[i])
                    dOfF[i] = -1;
                else if (histo[i + 1] > histo[i])
                    dOfF[i] = 1;
                else
                    dOfF[i] = 0;
            }
            dOfF[255] = dOfF[254];

            for (int i = 0; i < 255; i++)
            {
                if (dOfF[i + 1] < dOfF[i])
                    d2OfF[i] = -1;
                else if (dOfF[i + 1] > dOfF[i])
                    d2OfF[i] = 1;
                else
                    d2OfF[i] = 0;
            }
            d2OfF[255] = d2OfF[254];

            bool lookingForDown = true;
            int startInd = 0;
            for (int i = 0; i < 256; i++)
            {
                if (dOfF[i] == -1) // downhill start
                {
                    if (lookingForDown)
                    {
                        maximas.Add(((i + 1) + startInd) / 2);
                        lookingForDown = false;
                    }
                }
                if (dOfF[i] == 1)
                {
                    lookingForDown = true;
                    startInd = i;
                }
                if (i == 255)
                {
                    if (lookingForDown)
                    {
                        maximas.Add((i + startInd) / 2);
                    }
                    else
                    {
                        if (dOfF[i] == 1)
                            maximas.Add((i + startInd) / 2);
                    }
                }
            }
            return maximas.ToArray();

        }
        static public Image drawSmoothHistogramWithMaximas(Image img)
        {
            int[] histo = smoothHistogram(histogram(img));
            List<int> maximas = maximasOfHistoGram(histo).ToList();
            

            Bitmap bmp = new Bitmap(256,256);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    if (maximas.Contains(i))
                        bmp.SetPixel(i, 255 - j, (j <= histo[i]) ? Color.Red : Color.White);
                    else
                        bmp.SetPixel(i, 255 - j, (j <= histo[i]) ? Color.Black : Color.White);
                }
            }
            return bmp;
        }
        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type
            string lookupKey = mimeType.ToLower();

            //the codec to return, default to null
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return
            if (Encoders.ContainsKey(lookupKey))
            {
                //pull the codec from the lookup
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }
    }
}
