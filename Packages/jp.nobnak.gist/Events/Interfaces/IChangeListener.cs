using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events.Interfaces {

    public interface IChangeListener<Target> {

        void TargetOnChange(Target target);
    }
}
