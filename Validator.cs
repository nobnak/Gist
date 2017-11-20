using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {

    public class Validator {

        public event System.Action Validation;

        protected bool initialValidity;
        protected bool internalValidity;
        protected System.Func<bool> extraValidity;

        public Validator(bool initialValidity) {
            this.initialValidity = initialValidity;
            this.internalValidity = initialValidity;
        }
        public Validator() : this(false) { }

        public void Reset() {
            internalValidity = initialValidity;
            Validation = null;
            extraValidity = null;
        }
        public void SetExtraValidityChecker(System.Func<bool> extraValidity) {
            this.extraValidity = extraValidity;
        }
        public bool Valid {
            get { return internalValidity && (extraValidity == null || extraValidity()); }
        }
        public void Invalidate() {
            internalValidity = false;
        }
        public void CheckValidation() {
            if (Valid)
                return;
            internalValidity = true;

            if (Validation != null)
                Validation();
        }
    }
}