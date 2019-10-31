using nobnak.Gist.InputDevice;
using nobnak.Gist.Loader;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public class ExhibitorGroup : AbstractExhibitor {

        [SerializeField]
        protected AbstractExhibitor[] exhibitors = new AbstractExhibitor[0];
        [SerializeField]
        protected Data data = new Data();

        protected Validator dataValidator = new Validator();

        protected int selectedTab = 0;
        protected string[] tabNames = new string[0];

        #region Unity
        private void OnEnable() {
            dataValidator.Reset();
            dataValidator.Validation += () => {
                ResetDataFromModel();
                tabNames = exhibitors.Select(v => v.name).ToArray();
            };
        }
        #endregion

        #region interface
        public override void DeserializeFromJson(string json) {
            JsonUtility.FromJsonOverwrite(json, data);
            ApplyDataToModel();
        }
        public override object RawData() {
            dataValidator.Validate();
            return data;
        }
        public override string SerializeToJson() {
            dataValidator.Validate();
            return JsonUtility.ToJson(data, true);
        }
        public override void ApplyDataToModel() {
            data.ApplyTo(exhibitors);
        }
        public override void ResetDataFromModel() {
            data.UpdateFrom(exhibitors);
        }
        public override void Draw() {
            GUILayout.BeginVertical();
            GUILayout.Label("", GUI.skin.horizontalSlider);

            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            if (0 <= selectedTab && selectedTab < exhibitors.Length) {
                var ex = exhibitors[selectedTab];
                ex.Draw();
            }

            GUILayout.EndVertical();
        }
        #endregion

        #region classes
        public class Data {
            public string[] exhibitorData = new string[0];

            public static Data CreateFrom(AbstractExhibitor[] exhibitors) {
                return new Data().UpdateFrom(exhibitors);
            }

            public Data UpdateFrom(AbstractExhibitor[] exhibitors) {
                exhibitorData = exhibitors.Select(v => v.SerializeToJson()).ToArray();
                return this;
            }
            public Data ApplyTo(AbstractExhibitor[] exhibitors) {
                for (var i = 0; i < exhibitors.Length && i < exhibitorData.Length; i++) {
                    var j = exhibitorData[i];
                    var e = exhibitors[i];
                    e.DeserializeFromJson(j);
                }
                return this;
			}
		}
        #endregion
    }
}
