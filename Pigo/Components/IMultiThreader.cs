using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pigo.Components
{
    public interface IMultiThreader
    {
        /// <summary>
        /// Maximum number of threads
        /// </summary>
        int MaxThread { get; }

        /// <summary>
        /// Perform the action asynchrously on a individual thread
        /// </summary>
        void Action(Action action);

        /// <summary>
        /// is there any threads are running or not
        /// </summary>
        bool HasRunningThreads();
    }
}
