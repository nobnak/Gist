using nobnak.Gist;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

    public class MouseTracker {
        [System.Flags]
        public enum ButtonFlag { None = 0, Left = 1 << 0, Right = 1 << 1, Middle = 1 << 2 }

        public const float LESS_THAN_ONE = 0.999f;

        public event System.Action<MouseTracker> OnUpdate;
        public event System.Action<MouseTracker, ButtonFlag> OnSelectionDown;
        public event System.Action<MouseTracker, ButtonFlag> OnSelection;
        public event System.Action<MouseTracker, ButtonFlag> OnSelectionUp;

        public ButtonFlag PrevSelection { get; protected set; }
        public ButtonFlag CurrSelection { get; protected set; }

        public ButtonFlag SelectionDiff { get; protected set; }
        public ButtonFlag SelectionDown { get; protected set; }
        public ButtonFlag SelectionUp { get; protected set; }

        public Vector2 PrevPosition { get; protected set; }
        public Vector2 CurrPosition { get; protected set; }
        public Vector2 PositionDiff { get; protected set; }

        #region Static
        public static ButtonFlag GetSelection() {
            return (Input.GetMouseButton(0) ? ButtonFlag.Left : 0)
                            | (Input.GetMouseButton(1) ? ButtonFlag.Right : 0)
                            | (Input.GetMouseButton(2) ? ButtonFlag.Middle : 0);
        }
        public static bool Any(ButtonFlag selection, ButtonFlag search) {
            return (selection & search) != MouseTracker.ButtonFlag.None;
        }
        public static bool All(ButtonFlag selection, ButtonFlag search) {
            return (selection & search) == search;
        }
        #endregion

        public virtual void Clear() {
            OnUpdate = null;
            OnSelectionDown = null;
            OnSelection = null;
            OnSelectionUp = null;

            PrevSelection = CurrSelection = ButtonFlag.None;
        }
        public virtual void Update() {
            UpdateSelection();
            UpdatePosition();
            Notify();
        }

        protected virtual void UpdatePosition() {
            PrevPosition = CurrPosition;
            CurrPosition = Input.mousePosition;
            if (PrevSelection == ButtonFlag.None)
                PrevPosition = CurrPosition;
            PositionDiff = CurrPosition - PrevPosition;
        }

        protected virtual void UpdateSelection() {
            PrevSelection = CurrSelection;
            CurrSelection = GetSelection();
            SelectionDiff = PrevSelection ^ CurrSelection;
            SelectionDown = SelectionDiff & CurrSelection;
            SelectionUp = SelectionDiff & ~CurrSelection;
        }

        protected virtual void Notify() {
            if (OnUpdate != null)
                OnUpdate(this);
            if (SelectionDown != ButtonFlag.None && OnSelectionDown != null)
                OnSelectionDown(this, SelectionDown);
            if (CurrSelection != ButtonFlag.None && OnSelection != null)
                OnSelection(this, CurrSelection);
            if (SelectionUp != ButtonFlag.None && OnSelectionUp != null)
                OnSelectionUp(this, SelectionUp);
        }

    }

    public static class ButtonFlagExtension {
        public static bool Any(this MouseTracker.ButtonFlag selection, MouseTracker.ButtonFlag search) {
            return (selection & search) != MouseTracker.ButtonFlag.None;
        }
        public static bool All(this MouseTracker.ButtonFlag selection, MouseTracker.ButtonFlag search) {
            return (selection & search) == search;
        }
    }

}
