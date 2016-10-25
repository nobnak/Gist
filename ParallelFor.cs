using System.Collections.Generic;
using System.Threading;

namespace Gist {

	public static class Parallel {
        static AutoResetEvent[] _resets;

		public static void For(int fromInclusive, int toExclusive, System.Action<int> body) {
			var numThreads = 2 * System.Environment.ProcessorCount;
            if (_resets == null || _resets.Length != numThreads) {
                _resets = new AutoResetEvent[numThreads];
                for (var i = 0; i < numThreads; i++)
                    _resets [i] = new AutoResetEvent (false);
            }

            var work = new WaitCallback ((i) => {
                var ii = (int)i;
                var j = ii + fromInclusive;
                for (var k = (int)j; k < toExclusive; k += numThreads)
                    body ((int)k);
                _resets [ii].Set ();
            });

            lock (_resets) {
                for (var i = 0; i < numThreads; i++)
                    ThreadPool.QueueUserWorkItem (work, i);
            
                for (var i = 0; i < numThreads; i++)
                    _resets [i].WaitOne ();
            }
		}
        public static void SerialFor(int fromInclusive, int toExclusive, System.Action<int> body) {
            for (var i = fromInclusive; i < toExclusive; i++)
                body (i);
        }
	}
}