using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace nobnak.Gist {

	public static class Parallel {

		public static void For<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, T arg = default(T)) {
            var numThreads = 2 * System.Environment.ProcessorCount;
            For(fromInclusive, toExclusive, body, numThreads, arg);
        }
        public static void For<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, int numThreads, T arg = default(T)) {
            var resets = ForWithoutWait (fromInclusive, toExclusive, body, numThreads, arg);
            for (var i = 0; i < numThreads; i++)
                resets [i].WaitOne ();
		}

        public static IEnumerator ForAsync<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, T arg = default(T)) {
            var numThreads = 2 * System.Environment.ProcessorCount;
            return ForAsync(fromInclusive, toExclusive, body, numThreads);
        }
        public static IEnumerator ForAsync<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, int numThreads, T arg = default(T)) {
            var resets = ForWithoutWait (fromInclusive, toExclusive, body, numThreads, arg);
            for (var i = 0; i < numThreads; i++)
                while (!resets [i].WaitOne (0))
                    yield return null;
        }

        public static void SerialFor<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, T arg = default(T)) {
            for (var i = fromInclusive; i < toExclusive; i++)
                body (i, arg);
        }

    
        static AutoResetEvent[] ForWithoutWait<T>(int fromInclusive, int toExclusive, System.Action<int, T> body, int numThreads, T arg = default(T)) {
            var resets = new AutoResetEvent[numThreads];
            for (var i = 0; i < numThreads; i++)
                resets [i] = new AutoResetEvent (false);
            var work = new WaitCallback (i =>  {
                var ii = (int)i;
                var j = ii + fromInclusive;
                try {
                    for (var k = j; k < toExclusive; k += numThreads)
                        body (k, arg);
                } catch (System.Exception e) {
                    UnityEngine.Debug.LogWarning(e);
                } finally {
                    resets [ii].Set ();
                }
            });
            for (var i = 0; i < numThreads; i++)
                ThreadPool.QueueUserWorkItem (work, i);
            return resets;
        }
    }
}