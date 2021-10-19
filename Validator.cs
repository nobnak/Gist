using System.Linq;
using UnityEngine;

namespace nobnak.Gist {

	public struct Validator : IValidator {

		public event System.Action Validation;
		public event System.Action Validated;
		public event System.Action Invalidated;

		private bool useCache;
		private int validatedFrameCount;

		private bool isUnderValidation;
		private bool initialValidity;
		private bool validity;
		private System.Func<bool>[] checker;

		public Validator(bool initialValidity = false, bool useCache = true) {
			this.Validation = null;
			this.Validated = null;
			this.Invalidated = null;

			this.validatedFrameCount = -1;
			this.isUnderValidation = false;
			this.checker = null;

			this.initialValidity = initialValidity;
			this.validity = initialValidity;
			this.useCache = useCache;
		}

		public void Reset() {
			validity = initialValidity;
			checker = null;
			ResetEvents();
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

		private void ResetEvents() {
			Validation = null;
			Validated = null;
			Invalidated = null;
		}
		private void _Validate() {
			try {
				isUnderValidation = true;
				Validation?.Invoke();
			} finally {
				isUnderValidation = false;
			}
		}
		private bool Check(bool useChache) {
			if (useCache && validatedFrameCount == Time.frameCount)
				return true;

			var result = _Check();
			if (result)
				validatedFrameCount = Time.frameCount;
			return result;
		}
		private bool _Check() {
			if (checker == null)
				return true;

			foreach (var c in checker)
				if (!c())
					return false;
			return true;
		}

		private void NotifyInvalidated()  => Invalidated?.Invoke();
		private void NotifyValidated()  => Validated?.Invoke();

	}
}