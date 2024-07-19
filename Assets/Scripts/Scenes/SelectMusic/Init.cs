using UnityEngine;
using UnityEngine.UI;
namespace Scenes.SelectMusic
{
    public class Init : MonoBehaviour
    {
        public Scrollbar scrollbar;
        // Start is called before the first frame update
        private void Start()
        {
            scrollbar.value = 1;
        }

    }
}
