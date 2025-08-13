using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace nobnak.Gist.DocSys {

	public static class DocGen {

		public static bool IsList(this System.Type t) {
			return t.GetInterfaces()
				.Any(v => v.IsGenericType
					&& (v.GetGenericTypeDefinition() == typeof(IList<>)));
		}

		public static string GetTooltip(this MemberInfo mi) {
			return string.Join(", ",
				mi.GetCustomAttributes<TooltipAttribute>(true)
				.Cast<TooltipAttribute>()
				.Select(attr => attr.tooltip));
		}
		public static string GenerateListElement(this MemberInfo info, int i, string name) {
			return $"{new string('\t', i)}- {name} : "
			 + $"{GetTooltip(info)}";
		}
		public static IEnumerable<string> GenerateDoc(this FieldInfo info, int i) {

			yield return GenerateListElement(info, i, info.Name);

			var ft = info.FieldType;
			if (ft.IsValueType
				|| ft == typeof(string)
				|| ft.IsArray
				|| IsList(ft))
				yield break;

			foreach (var fchild in ft.GetFields())
				foreach (var line in GenerateDoc(fchild, i + 1))
					yield return line;
		}
	}
}
