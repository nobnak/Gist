using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Extensions.ProjectExt {

	public static class ProjectExtension {

		public static string ProjectPath {
			get {
				return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
			}
		}

		public static string PathFromProjectFolder(this string filename) {
			return $"{ProjectPath}/{filename}";
		}
	}
}
