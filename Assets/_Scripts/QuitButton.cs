using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class QuitButton : MonoBehaviour
    {
        [CalledFromUnity]
        public void Quit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}