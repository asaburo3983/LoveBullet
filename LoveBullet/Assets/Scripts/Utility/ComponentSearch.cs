using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ComponentSearch : MonoBehaviour
    {
        public static T GetParentComponent<T>(Transform _parent)
            where T : MonoBehaviour
        {
            if (_parent.gameObject.TryGetComponent<T>(out var t)) {
                return t;
            }
            else if (_parent.parent == null) {
                Debug.LogWarning("コンポーネントがありませんでした"); 
                return default(T);
            }
            else {
                return GetParentComponent<T>(_parent.parent);
            }
        }

        public static T GetChildComponent<T>(Transform _parent)
           where T : MonoBehaviour
        {
            if (_parent.gameObject.TryGetComponent<T>(out var t)) {
                return t;
            }
            else if (_parent.childCount == 0) {
                Debug.LogWarning("コンポーネントがありませんでした");
                return default(T);
            }
            else {

                foreach (Transform _child in _parent) {
                    T _t =   GetParentComponent<T>(_child);
                    if(_t != default(T)) {
                        return _t;
                    }
                }

                return default(T);
            }
        }
    }

}