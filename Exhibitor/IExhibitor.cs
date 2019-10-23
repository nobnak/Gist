using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {
    
    public interface IExhibitor {

        void Invalidate();
        string SerializeToJson();
        void DeserializeFromJson(string json);
		object RawData();
		void Draw();
	}
}
