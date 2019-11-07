using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace nobnak.Gist.Extensions.CustomAttrExt {

    public static class CustomAttributeExtension {
        
        public static bool TryGetAttribute<T>(this MemberInfo info, out T attr, bool inherit = true)
            where T : System.Attribute {
            attr = info.GetCustomAttribute<T>(inherit);
            return attr != null;
        }
    }
}
