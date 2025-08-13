using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace nobnak.Gist.Extensions.SpecialFolder {

    public static class SpecialFolderExtension {
        
        public static string DataPath(this System.Environment.SpecialFolder folder, string filename) {
            var dir = System.Environment.GetFolderPath(folder);
            return Path.Combine(dir, filename);
        }
        public static bool Load<S>(this System.Environment.SpecialFolder folder, string filename, ref S data) {
            try {
                var path = folder.DataPath(filename);
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), data);
                return true;
            } catch (System.Exception e) {
                Debug.Log(e);
            }
            return false;
        }
        public static bool Save<S>(this System.Environment.SpecialFolder folder, string filename, ref S data) {
            try {
                var path = folder.DataPath(filename);
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
                return true;
            } catch (System.Exception e) {
                Debug.Log(e);
            }
            return false;
        }
    }
}