using UnityEngine;

namespace Generic {
    public abstract class SingletonBehaviour<T> : DkpMonoBehaviour where T : MonoBehaviour {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance {
            get {
                lock (_lock) {
                    if (_instance != null) return _instance;

                    _instance = (T) FindObjectOfType(typeof(T));

                    if (_instance != null) return _instance;

                    var singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T);

                    DontDestroyOnLoad(singleton);

                    return _instance;
                }
            }
        }
    }
}