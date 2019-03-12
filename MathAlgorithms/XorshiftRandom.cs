using nobnak.Gist.Extensions.Int;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms {

	public class XorshiftRandom : IEnumerable<ulong> {
		protected ulong seed;

		public XorshiftRandom(ulong seed) {
			Seed = seed;
		}
		public XorshiftRandom(XorshiftRandom org) {
			Seed = org.seed;
		}
		public XorshiftRandom() : this(NowTicks()) { }

		#region public
		public ulong Seed {
			get { return seed; }
			set {
				if (value <= 0)
					throw new System.Exception("Seed must be bigger than 0");
				seed = value;
			}
		}
		public ulong Next() {
			return (seed = Xor64(seed));
		}
		public ulong RejectionSampleIn(ulong width) {
			if (width <= 0)
				throw new System.Exception(string.Format("{0} <= 0", width));

			var uwidth = (ulong)width;
			var lowerBitsMask = (uwidth.Po2() - 1);
			ulong v = 0;
			while ((v = (Next() & lowerBitsMask)) >= uwidth) ;
			return v;
		}
		public long NextRange(long start, long end) {
			return (long)RejectionSampleIn((ulong)(end - start)) + start;
		}
		public double NextNormalized() {
			return (double)(Next() - 1) / ulong.MaxValue;
		}
		#endregion

		#region static
		public static long MIN_TICKS = System.DateTime.MinValue.Ticks;
		public static ulong Xor64(ulong y) {
			y ^= (y << 13);
			y ^= (y >> 17);
			return (y ^= (y << 5));
		}
		public static ulong TickeFromMin(long ticks) {
			return (ulong)(ticks - MIN_TICKS);
		}
		public static ulong NowTicks() {
			return TickeFromMin(System.DateTime.Now.Ticks);
		}
		#endregion

		#region IEnumerable
		public IEnumerator<ulong> GetEnumerator() {
			while (true)
				yield return Next();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			throw new System.NotImplementedException();
		}
		#endregion
	}
}
