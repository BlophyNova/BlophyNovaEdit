using UnityEngine;
namespace UtilityCode.Singleton
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = GetComponent<T>();
            }
            else Destroy(this);
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}