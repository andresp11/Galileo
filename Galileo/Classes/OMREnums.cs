using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMR;

namespace OMR.Enums
{
    public static class enumHelper
    {
        public static Type structureTypeOfvariable(sheetProperties sv)
        {
            if (sv == sheetProperties.NumberBlock)
                return typeof(sheetObjectTypes.numberBlock);
            else if (sv == sheetProperties.EmptyBlock)
                return typeof(sheetObjectTypes.emptyBlock);
            else if (sv == sheetProperties.AnswerBlock)
                return typeof(sheetObjectTypes.answerBlock);
            if ((int)sv <= 10)
                return typeof(double);
            else if ((int)sv <= 20)
                return typeof(System.Drawing.Rectangle);
            else if ((int)sv <= 30)
                return typeof(int);
            else if ((int)sv <= 40)
                return typeof(System.Drawing.Size);
            else
                throw new Exception("Unknown object type requested");
        }

        public static Type dataTypeOfvariable(sheetProperties sv)
        {
            if ((int)sv <= 10)
                return typeof(double);
            else if ((int)sv <= 20)
                return typeof(System.Drawing.RectangleF);
            else if ((int)sv <= 30)
                return typeof(int);
            else if ((int)sv <= 40)
                return typeof(System.Drawing.Size);
            else
                throw new Exception("Unknown object type requested");
        }
    }
    public enum sheetProperties: int
    {
        //double
        minBlobSizeRatio = 1,
        maxBlobSizeRatio = 2,

        //Rectangle
        AnswerBlock = 11,
        NumberBlock = 12,
        EmptyBlock = 13,

        //int
        NumberOfBubbleBlocks = 21,
        NumberOfNumberBlocks = 22,

        //Size
        SheetBounds =31,

    }
}
