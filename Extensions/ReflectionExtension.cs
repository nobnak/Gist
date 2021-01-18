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

		public static IEnumerable<FieldInfo> GetInstancePublicFields(this System.Type t) {
			return t.GetFields(BF_PUBLIC_INSTANCE);
		}
		public static IEnumerable<PropertyInfo> GetInstancePublicProperties(this System.Type t) {
			return t.GetProperties(BF_PUBLIC_INSTANCE);
		}

		public static object GetValue(this MemberInfo m, object target) {
            if (TryGetValue(m, target, out object v))
                return v;
            return null;
        }
        public static bool TryGetValue<OUT>(this MemberInfo m, object target, out OUT v) {
            switch (m.MemberType) {
                case MemberTypes.Field:
                    var fi = (FieldInfo)m;
                    if (typeof(OUT).IsAssignableFrom(fi.FieldType)) {
                        v = (OUT)fi.GetValue(target);
                        return true;
                    }
                    break;
                case MemberTypes.Property:
                    var pi = (PropertyInfo)m;
                    if (typeof(OUT).IsAssignableFrom(pi.PropertyType)) {
                        v = (OUT)pi.GetValue(target);
                        return true;
                    }
                    break;
            }
            v = default;
            return false;
        }
        public static bool TrySetValue<IN>(this MemberInfo m, object target, IN v) {
            switch (m.MemberType) {
                case MemberTypes.Field:
                    var fi = (FieldInfo)m;
                    if (fi.FieldType.IsAssignableFrom(typeof(IN))) { 
                        fi.SetValue(target, v);
                        return true;
                    }
                    break;
                case MemberTypes.Property:
                    var pi = (PropertyInfo)m;
                    if (pi.PropertyType.IsAssignableFrom(typeof(IN))) {
                        pi.SetValue(target, v);
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}