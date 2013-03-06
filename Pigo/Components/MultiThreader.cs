using System;
using System.Threading;

namespace Pigo.Components
{
    public class MultiThreader : IMultiThreader
    {
        Thread[] _threads = new Thread[5];
        object _lock = new object();

        public MultiThreader(int maxThreads)
        {
            _threads = new Thread[maxThreads];
        }
        
        /// <summary>
        /// Max number of threads
        /// </summary>
        public int MaxThread
        {
            get
            {
                return _threads.Length;
            }
        }

        /// <summary>
        /// perform the action asynchrously on a seperate thread
        /// </summary>
        public void Action(Action action)
        {
            lock (_lock)
            {
                int freeThreadIndex = GetFreeThreadIndex();
                while (freeThreadIndex < 0)
                {                    
                    System.Threading.Thread.Sleep(100);
                    freeThreadIndex = GetFreeThreadIndex();
                }

                if (MaxThread > 1)
                {
                    _threads[freeThreadIndex] = new Thread(new ThreadStart(action));
                    _threads[freeThreadIndex].Start();
                }
                else
                {
                    action.Invoke();
                }
            }
        }

        /// <summary>
        /// Whether there are running threads
        /// </summary>
        public bool HasRunningThreads()
        {
            lock (_lock)
            {
                for (int i = 0; i < _threads.Length; i++)
                {
                    if (_threads[i] == null)
                    {
                     
                    }
                    
                    else if (_threads[i].IsAlive)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int GetFreeThreadIndex()
        {
            int freeThreadIndex = -1;
            int currentIndex = 0;
            lock (_lock)
            {
                foreach (Thread thread in _threads)
                {
                    if ((thread == null) || !thread.IsAlive)
                    {
                        freeThreadIndex = currentIndex;
                        break;
                    }

                    currentIndex++;
                }
            }
            return freeThreadIndex; ;
        }
    }
}
