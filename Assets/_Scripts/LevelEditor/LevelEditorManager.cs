using UnityEngine;
using UnityEngine.SceneManagement;

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
                SceneManager.LoadScene(0);
            }    
        }
    }
}