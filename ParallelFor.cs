using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace Gist {

	public static class Parallel {

		public static void For(int fromInclusive, int toExclusive, System.Action<int> body) {
            var numThreads = 2 * System.Environment.ProcessorCount;
            For(fromInclusive, toExclusive, body, numThreads);
        }
        public static void For(int fromInclusive, int toExclusive, System.Action<int> body, int numThreads) {
            var resets = ForWithoutWait (fromInclusive, toExclusive, body, numThreads);
            for (var i = 0; i < numThreads; i++)
                resets [i].WaitOne ();
		}

        public static IEnumerator ForAsync(int fromInclusive, int toExclusive, System.Action<int> body) {
            var numThreads = 2 * System.Environment.ProcessorCount;
            return ForAsync(fromInclusive, toExclusive, body, numThreads);
        }
        public static IEnumerator ForAsync(int fromInclusive, int toExclusive, System.Action<int> body, int numThreads) {
            var resets = ForWithoutWait (fromInclusive, toExclusive, body, numThreads);
            for (var i = 0; i < numThreads; i++)
                while (!resets [i].WaitOne (0))
                    yield return null;
        }

        public static void SerialFor(int fromInclusive, int toExclusive, System.Action<int> body) {
            for (var i = fromInclusive; i < toExclusive; i++)
                body (i);
        }

    
        static AutoResetEvent[] ForWithoutWait (int fromInclusive, int toExclusive, System.Action<int> body, int numThreads) {
            var resets = new AutoResetEvent[numThreads];
            for (var i = 0; i < numThreads; i++)
                resets [i] = new AutoResetEvent (false);
            var work = new WaitCallback (i =>  {
                var ii = (int)i;
                var j = ii + fromInclusive;
                try {
                    for (var k = j; k < toExclusive; k += numThreads)
                        body (k);
                }
                finally {
                    resets [ii].Set ();
                }
            });
            for (var i = 0; i < numThreads; i++)
                ThreadPool.QueueUserWorkItem (work, i);
            return resets;
        }
    }
}