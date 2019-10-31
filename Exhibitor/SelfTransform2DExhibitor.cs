using ModelDrivenGUISystem;
using ModelDrivenGUISystem.Factory;
using ModelDrivenGUISystem.ValueWrapper;
using ModelDrivenGUISystem.View;
using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    [ExecuteAlways]
    public class SelfTransform2DExhibitor : AbstractExhibitor {

        [SerializeField]
        protected Converter converter = new Converter();
        [SerializeField]
        protected Data data = new Data();

        protected Validator dataValidator = new Validator();
        protected BaseView view;

        #region unity
        private void OnEnable() {
            dataValidator.Reset();
            dataValidator.Validation += () => {
                ResetViewModelFromModel();
                ClearView();
            };
        }
        private void OnValidate() {
            //dataValidator.Invalidate();
        }
        private void Update() {
            dataValidator.Validate();
        }
        #endregion

        #region interface
        #region Exhibitor
        public override void DeserializeFromJson(string json) {
            Debug.Log($"DeserializeFromJson");
            JsonUtility.FromJsonOverwrite(json, data);
            ApplyViewModelToModel();
        }
        public override object RawData() {
            dataValidator.Validate();
            return data;
        }
        public override string SerializeToJson() {
            dataValidator.Validate();
            return JsonUtility.ToJson(data);
        }
        public override void ResetViewModelFromModel() {
            Debug.Log($"ResetDataFromModel");
            data.name = gameObject.name;
            var n = data.node;
            n.position = converter.EncodePosition(transform.localPosition);
            n.rotation = converter.EncodeRotation(transform.localRotation);
            n.scale = converter.EncodePosition(transform.localScale);
            data.node = n;
        }
        public override void ApplyViewModelToModel() {
            Debug.Log($"ApplyDataToModel:");
            gameObject.name = data.name;
            var n = data.node;
            transform.localPosition = converter.DecodePosition(n.position, 0f);
            transform.localRotation = converter.DecodeRotation(n.rotation);
            transform.localScale = converter.DecodePosition(n.scale, 1f);
        }
        public override void Draw() {
            GetView().Draw();
        }
        #endregion

        #endregion

        #region member
        protected BaseView GetView() {
            dataValidator.Validate();
            if (view == null) {
                var factory = new SimpleViewFactory();
                view = ClassConfigurator.GenerateClassView(new BaseValue<object>(data), factory);
            }
            return view;
        }
        protected void ClearView() {
            if (view != null) {
                view.Dispose();
                view = null;
            }
        }
        #endregion

        #region Classes
        [System.Serializable]
        public class Data {
            public string name = "No name";
			public TransformData node = new TransformData();
        }
        [System.Serializable]
        public class TransformData {
            public Vector2 position;
            public float rotation;
            public Vector2 scale;
        }

        [System.Serializable]
        public class Converter {
            public Vector2Int indices = new Vector2Int(0, 1);

            public Vector2 EncodePosition(Vector3 v3, out float z) {
                z = v3[IndexOfZ];
                return new Vector2(v3[indices.x], v3[indices.y]);
            }
            public Vector2 EncodePosition(Vector3 v3) {
                float z;
                return EncodePosition(v3, out z);
            }
            public float EncodeRotation(Quaternion q) {
                return q.eulerAngles[IndexOfZ];
            }

            public Vector3 DecodePosition(Vector2 v2, float z = 0f) {
                var v3 = Vector3.zero;
                v3[indices.x] = v2.x;
                v3[indices.y] = v2.y;
                v3[IndexOfZ] = z;
                return v3;
            }
            public Quaternion DecodeRotation(float r) {
                var v3 = Vector3.zero;
                v3[IndexOfZ] = r;
                return Quaternion.Euler(v3);
            }

            public int IndexOfZ {
                get { return 3 - (indices.x + indices.y); }
            }
        }
        #endregion
    }
}
