using UnityEngine;

namespace Scenes.PublicScripts
{
    public class PassThisRect : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool IsPass;

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return IsPass;
        }
    }
}