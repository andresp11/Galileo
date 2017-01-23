using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OMR.helpers
{
    /// <summary>
    /// This class is under construction and doesn't work as intended YET. Some serious work has to be done by someone who understands multi threading.
    /// </summary>
    public class MultiThreading
    {
        public event AsyncProgressEventHandler onProgressChanged;
        public event AsyncCompletionEventHandler onThreadCompleted;
        public event AsyncCompletionEventHandler onAllCompleted;
        public OMREngine[] Engines { get { return engines; } }
        OMREngine[] engines;

        public int MaximumThreads { get { return maximumThreads; } set { maximumThreads = Math.Min(Math.Max(value, 1), 32); } }
        public int CompletedThreads { get { int count = 0; for (int i = 0; i < engines.Length; i++) count += engines[i].ResultReady ? 1 : 0; return count; } }
        public int InProcessThreads { get {return inP; } }
        public int InQue { get { return inQue; } }
        public int Progress { get { return (int)Math.Round((double)progress_ / (double)engines.Length); } }
        int maximumThreads = 4, inP, inQue, progress_, state = 0;
        long startTempFile = 0;
        string tFile;
        
        public MultiThreading(int maximumThreads_, string [] imageAddresses, string sheetInfo, string sheetName, string ruta)
        {
            throw new Exception("This class is still under construction. Due to my poor grip over multithreading, and lack of understanding on how the System.Drawing manages resourcecs, multi threads face problems like memoryoverflow, Wrong memory access. Keep tight while I get this working. Or you could do some tweaks and share it with me. :)");
            tFile = sheetInfo;
            duplicateFile.cleanUpFiles(ruta);
            maximumThreads = maximumThreads_;
            startTempFile = DateTime.Now.Ticks;
            long rand = startTempFile;
            engines = new OMREngine[imageAddresses.Length];
            for (int i = 0; i < imageAddresses.Length; i++)
            {
                engines[i] = new OMREngine(imageAddresses[i], duplicateFile.createDuplicate(sheetInfo, ruta), sheetName, ruta);
                engines[i].onAsyncProgressUpdated += multitasking_onAsyncProgressUpdated;
                engines[i].onAsyncCompletion += multitasking_onAsyncCompletion;
                engines[i].Index = i;
                engines[i].OverrideWhite = false;
                state = 1;
                rand++;
            }
        }
        
        public void Pause()
        { if (state != 1) throw new Exception("Nothing is in process"); else state = -1; }

        public void Resume()
        {
            if (state == -1)
            {
                engines[engines.Length - inQue].StartProcessAsync();

                inP++;
                inQue--;
            }
            else
                throw new Exception("Nothing is in process");
        }
        public void StartProcess()
        {
            inP = 0;
            inQue = engines.Length;

            for (int i = 0; i < maximumThreads && i < engines.Length; i++)
            {
                engines[i].StartProcessAsync();
                inP++;
                inQue--;
            }
        }

        void multitasking_onAsyncCompletion(CompletionEvenArgs e)
        {
            inP--;

            if (inQue > 0)
            {
                onThreadCompleted(new CompletionEvenArgs(e.EngineIndex));

                if (state != -1)
                {
                    engines[engines.Length - inQue].StartProcessAsync();

                    inP++;
                    inQue--;
                }
            }
            else
            {
                onThreadCompleted(new CompletionEvenArgs(e.EngineIndex));

                if (inP == 0)
                {
                    onAllCompleted(new CompletionEvenArgs(e.EngineIndex));
                    //duplicateFile.cleanUpFiles(ruta);
                }
            }
        }

        void multitasking_onAsyncProgressUpdated(ProgressEventArgs e)
        {
            int p = 0;
            for (int i = 0; i < engines.Length; i++)
            {
                p += engines[i].Progress;
            }
            progress_ = p;
            onProgressChanged(new ProgressEventArgs(Progress, e.EngineIndex));
        }
    }
}
