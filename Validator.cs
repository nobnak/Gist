using System.Linq;
using UnityEngine;

namespace nobnak.Gist {

	public class Validator : IValidator {

		public event System.Action Validation;
		public event System.Action Validated;
		public event System.Action Invalidated;

		protected bool useCache;
		protected int validatedFrameCount = -1;

		protected bool isUnderValidation;
		protected bool initialValidity;
		protected bool validity;
		protected System.Func<bool>[] checker;

		public Validator(bool initialValidity, bool useCache = true) {
			this.initialValidity = initialValidity;
			this.validity = initialValidity;
			this.useCache = useCache;
		}
		public Validator() : this(false) { }

		public void Reset() {
			validity = initialValidity;
			Validation = null;
			checker = null;
		}
		public void SetCheckers(params System.Func<bool>[] checkers) {
			this.checker = checkers;
		}
		public bool IsValid {
			get {
				return validity && Check(true);
			}
		}
		public void Invalidate() {
			if (validity) {
				validity = false;
				NotifyInvalidated();
			}
		}

		public bool Validate(bool force = false) {
			if (isUnderValidation)
				return false;
			if (!force && IsValid)
				return true;

			Invalidate();
			validity = true;
			_Validate();

			var result = Check(false);
			if (result && validity) {
				NotifyValidated();
			}

			return result;
		}

		protected void _Validate() {
			try {
				isUnderValidation = true;
				if (Validation != null)
					Validation();
			} finally {
				isUnderValidation = false;
			}
		}
		protected bool Check(bool useChache) {
			if (useCache && validatedFrameCount == Time.frameCount)
				return true;

			var result = _Check();
			if (result)
				validatedFrameCount = Time.frameCount;
			return result;
		}
		protected bool _Check() {
			if (checker == null)
				return true;

			foreach (var c in checker)
				if (!c())
					return false;
			return true;
		}

		protected void NotifyInvalidated() {
			if (Invalidated != null)
				Invalidated();
		}
		protected void NotifyValidated() {
			if (Validated != null)
				Validated();
		}

	}
}