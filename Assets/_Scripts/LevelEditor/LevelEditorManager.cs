using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class LevelEditorManager : MonoBehaviour
    {
        [UnityMessage]
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
                
        }
    }
}