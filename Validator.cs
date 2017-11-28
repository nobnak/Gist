using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Gist {

    public class Validator {

        public event System.Action Validation;
        public event System.Action Validated;
        public event System.Action Invalidated;

        protected bool initialValidity;
        protected bool validity;
        protected System.Func<bool>[] checker;

        public Validator(bool initialValidity) {
            this.initialValidity = initialValidity;
            this.validity = initialValidity;
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
        public bool IsValid { get { return validity; } }
        public void Invalidate() {
            validity = false;
            NotifyInvalidated();
        }

        public bool CheckValidation() {
            if (validity)
                return true;

            Validate();

            var result = Check();
            if (result) {
                validity = true;
                NotifyValidated();
            }

            return result;
        }

        protected void Validate() {
            if (Validation != null)
                Validation();
        }
        protected bool Check() {
            return checker == null || checker.All(c => c());
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