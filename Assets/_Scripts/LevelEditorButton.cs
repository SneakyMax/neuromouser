using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Scripts
{
    [UnityComponent]
    public class LevelEditorButton : MonoBehaviour
    {
        public void GoToLevelEditor()
        {
            SceneManager.LoadScene(2);
        } 
    }
}