using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

	public class ExhibitorGroup : AbstractExhibitor {

        [SerializeField]
        protected AbstractExhibitor[] exhibitors = new AbstractExhibitor[0];
        [SerializeField]
        protected Data data = new Data();

        protected Validator validator = new Validator();

        protected int selectedTab = 0;
        protected string[] tabNames = new string[0];

        #region Unity
        private void OnEnable() {
            validator.Reset();
            validator.Validation += () => {
                data.UpdateFrom(exhibitors);
                tabNames = exhibitors.Select(v => v.Name).ToArray();
            };
        }
        #endregion

        #region interface

        #region AbstractExhibitor
        public override void DeserializeFromJson(string json) {
            JsonUtility.FromJsonOverwrite(json, data);
			//data.ApplyTo(exhibitors, (e, i) => Debug.LogWarning($"Exception on load at {i} in {name}\n{e}"));
			data.ApplyTo(exhibitors);
			ReflectChangeOf(MVVMComponent.ViewModel);
        }
        public override object RawData() {
            validator.Validate();
            return data;
        }
        public override string SerializeToJson() {
            validator.Validate(true);
            return JsonUtility.ToJson(data, true);
        }
        public override void ApplyViewModelToModel() {
            foreach (var ex in exhibitors)
                ex.ApplyViewModelToModel();
        }
        public override void ResetViewModelFromModel() {
            foreach(var ex in exhibitors)
                ex.ResetViewModelFromModel();
        }
        public override void Draw() {
            validator.Validate();

            GUILayout.BeginVertical();
            GUILayout.Label("", UnityEngine.GUI.skin.horizontalSlider);

            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            var ex = SelectedExhibitor;
            if (ex != null){
                ex.Draw();
            }

            GUILayout.EndVertical();
        }
        public override void ReflectChangeOf(MVVMComponent latestOne) {
            SelectedExhibitor?.ReflectChangeOf(latestOne);
        }
        #endregion

        public AbstractExhibitor SelectedExhibitor {
            get {
                return (0 <= selectedTab && selectedTab < exhibitors.Length)
                    ? exhibitors[selectedTab] 
                    : null;
            }
        }
		public IEnumerable<AbstractExhibitor> Exhibitors {
			get => exhibitors;
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
            public Data ApplyTo(AbstractExhibitor[] exhibitors, System.Action<System.Exception, int> onError = null) {
                for (var i = 0; i < exhibitors.Length && i < exhibitorData.Length; i++) {
					try {
						var j = exhibitorData[i];
						var e = exhibitors[i];
						e.DeserializeFromJson(j);
					} catch(System.Exception e) {
						if (onError == null) throw e; else onError(e, i);
					}
                }
                return this;
			}
		}
        #endregion
    }
}
