using System.Reflection;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public enum MVVMComponent { Model = 0, ViewModel, View }

    public abstract class AbstractExhibitor : MonoBehaviour {

        public static readonly BindingFlags B_INST_F 
            = BindingFlags.Instance 
            | BindingFlags.Public 
            | BindingFlags.NonPublic;

        [SerializeField]
        protected string uiName;

        #region interface

        #region IExhibitor
        public abstract string SerializeToJson();
		public abstract void DeserializeFromJson(string json);
		public abstract object RawData();

		public virtual void Draw() { }
        public virtual void ResetView() { }
        public virtual void ApplyViewModelToModel() { }
        public virtual void ResetViewModelFromModel() { }

        public virtual void ReflectChangeOf(MVVMComponent latestOne) {
            switch (latestOne) {
				case MVVMComponent.Model:
					NotifyModelChanged();
					break;
				case MVVMComponent.ViewModel:
					NotifyViewModelChanged();
					break;
				case MVVMComponent.View:
					NotifyViewChanged();
					break;
			}
        }

		public virtual void NotifyModelChanged() {
			ResetViewModelFromModel();
			ResetView();
		}
		public virtual void NotifyViewModelChanged() {
			ApplyViewModelToModel();
			ResetView();
		}
		public virtual void NotifyViewChanged() {
			ApplyViewModelToModel();
		}
        #endregion

        [ReplacementField("uiName")]
        public string Name {
            get {
                var returnName = name;
                if (TryGetField(nameof(Name), typeof(string), out var valueField)) {
                    var altName = (string)valueField.GetValue(this);
                    if (!string.IsNullOrWhiteSpace(altName))
                        returnName = altName;
                }
                return returnName;
            }
            set {
                if (TryGetField(nameof(Name), typeof(string), out var valueField)) {
                    valueField.SetValue(this, value);
                    return;
                }
                name = value;
            }
        }
        #endregion

        #region member
        protected bool TryGetField(string name, System.Type returnType, out FieldInfo value) {
            try {
                var tt = GetType();
                PropertyInfo nameProp;
                ReplacementFieldAttribute attr;

                if (((nameProp = tt.GetProperty(name, returnType)) != null)
                    && ((attr = nameProp.GetCustomAttribute<ReplacementFieldAttribute>()) != null)
                    && ((value = tt.GetField(attr.nameField, B_INST_F)) != null))
                    return true;
            } catch(System.Exception e) {
                Debug.LogWarning(e);
            }
            value = default;
            return false;
        }
        #endregion
    }
}
