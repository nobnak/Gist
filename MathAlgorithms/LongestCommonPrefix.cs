using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms {

	public static class LongestCommonPrefix {

		public static readonly int[] BYTE_TO_COUNT = ByteToLength(new int[256]);

		public static int Length(byte b) {
			return BYTE_TO_COUNT[b];
		}
		public static int Length(uint v) {
#if UNITY_EDITOR
			if (!System.BitConverter.IsLittleEndian)
				throw new System.Exception("Doesn't support Big Endian");
#endif

			var len = 0;
			BytesOfUint bytes = v;
			var l = Length(bytes.b3);
			len += l;
			if (l < 8)
				return len;

			l = Length(bytes.b2);
			len += l;
			if (l < 8)
				return len;

			l = Length(bytes.b1);
			len += l;
			if (l < 8)
				return len;

			l = Length(bytes.b0);
			len += l;
			return len;
		}

		public static int ByteToLength(byte b) {
			byte i = 0;
			for (; i < 8; ++i)
				if ((b & (1 << (7 - i))) == 0)
					break;
			return i;
		}
		public static int[] ByteToLength(int[] output) {
			for (uint i = 0; i < output.Length; i++)
				output[i] = ByteToLength((byte)i);
			return output;
		}

		public static int LengthOfLCP(this uint a, uint b) {
			return Length(~(a ^ b));
		}

		public static uint PrefixByLength(this uint a, int length) {
			var mask = (~0u >> length) << length;
			return a & mask;
		}
		public static uint Prefix(this uint a, uint b) {
			var len = a.LengthOfLCP(b);
			return a.PrefixByLength(len);
		}

		#region class
		[StructLayout(LayoutKind.Explicit)]
		public struct BytesOfUint {
			[FieldOffset(0)]
			public uint v;
			[FieldOffset(0)]
			public byte b0;
			[FieldOffset(1)]
			public byte b1;
			[FieldOffset(2)]
			public byte b2;
			[FieldOffset(3)]
			public byte b3;

			public static implicit operator BytesOfUint(uint v) {
				return new BytesOfUint() { v = v };
			}
		}
		#endregion
	}
}
