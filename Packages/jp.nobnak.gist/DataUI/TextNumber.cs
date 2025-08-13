using UnityEngine;
using System.Collections;

namespace nobnak.Gist.DataUI {
	public class TextFloat : Reactive<float> {
		string _strValue = "0";
		
		public TextFloat(float data) : base(data) {
			SetStrData(data);
		}
		#region Reactive
		public string StrValue {
			get {
				return _strValue;
			}
			set {
				if (value != null && value != _strValue) {
					_strValue = value;

					float numValue;
					if (float.TryParse(_strValue, out numValue))
						base.SetData(numValue);
				}
			}
		}
		protected override void SetData(float value) {
			SetStrData(value);
			base.SetData(value);
		}
		#endregion

		#region member
		protected virtual void SetStrData(float data) {
			_strValue = data.ToString();
		}
		#endregion
	}

	public class TextInt : Reactive<int> {
		string _strValue = "0";
		
		public TextInt(int init) :base(init) {
			SetStrDataInternal(init);
		}

		public string StrValue {
			get {
				return _strValue;
			}
			set {
				if (value != null && value != _strValue) {
					_strValue = value;

					int numValue;
					if (int.TryParse(_strValue, out numValue)) {
						base.SetData(numValue);
					}
				}				
			}
		}

		#region Reactive
		protected override void SetData(int value) {
			SetStrDataInternal(value);
			base.SetData(value);
		}
		#endregion

		#region member
		protected virtual void SetStrDataInternal(int init) {
			_strValue = init.ToString();
		}
		#endregion
	}

	public class TextVector {
		TextFloat[] _texts = new TextFloat[]{ new TextFloat(0), new TextFloat(0), new TextFloat(0), new TextFloat(0) };
		Vector4 _value = Vector4.zero;
		
		public TextVector(Vector4 existing) {
			Value = existing;
		}
		
		public string this[int index] {
			get { return _texts[index].StrValue; }
			set { _texts[index].StrValue = value; }
		}
		
		public Vector4 Value {
			get {
				_value.Set(_texts[0].Value, _texts[1].Value, _texts[2].Value, _texts[3].Value);
				return _value;
			}
			set {
				_value = value;
				_texts[0].Value = _value.x;
				_texts[1].Value = _value.y;
				_texts[2].Value = _value.z;
				_texts[3].Value = _value.w;
			}
		}
    }  

    public class TextMatrix {
        TextFloat[] _texts = new TextFloat[16];
        Matrix4x4 _value = Matrix4x4.zero;

        public TextMatrix(Matrix4x4 existing) {
            for (var i = 0; i < _texts.Length; i++)
                _texts[i] = new TextFloat(0);
            Value = existing;
        }

        public string this[int index] {
            get { return _texts[index].StrValue; }
            set { _texts[index].StrValue = value; }
        }

        public Matrix4x4 Value {
            get {
                for (var y = 0; y < 4; y++)
                    for (var x = 0; x < 4; x++)
                        _value [x + y * 4] = _texts [y + x * 4].Value;
                return _value;
            }
            set {
                _value = value;
                for (var y = 0; y < 4; y++)
                    for (var x = 0; x < 4; x++)
                        _texts [x + y * 4].Value = _texts [y + x * 4].Value;
            }
        }
    }
}