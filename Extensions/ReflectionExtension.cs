using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace nobnak.Gist.Extensions.ReflectionExt {

	public static class ReflectionExtension {
		public static readonly BindingFlags BF_PUBLIC_INSTANCE = BindingFlags.Instance | BindingFlags.Public;

		public static IEnumerable<MemberInfo> GetMembers(this System.Type t, BindingFlags f) {
			return t.GetFields(f)
				.Cast<MemberInfo>()
				.Concat(t.GetProperties(f));
		}
		public static IEnumerable<MemberInfo> GetInstancePublicMembers(this System.Type t) {
			return t.GetMembers(BF_PUBLIC_INSTANCE);
		}

		public static IEnumerable<MemberInfo> GetInstancePublicFields(this System.Type t) {
			return t.GetFields(BF_PUBLIC_INSTANCE);
		}
		public static IEnumerable<MemberInfo> GetInstancePublicProperties(this System.Type t) {
			return t.GetProperties(BF_PUBLIC_INSTANCE);
		}

		public static object GetValue(this MemberInfo m, object target) {
			switch (m.MemberType) {
				case MemberTypes.Field:
					return ((FieldInfo)m).GetValue(target);
				case MemberTypes.Property:
					return ((PropertyInfo)m).GetValue(target);
				default:
					return null;
			}
		}
	}
}