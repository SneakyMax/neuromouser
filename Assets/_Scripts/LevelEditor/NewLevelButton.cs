using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class NewLevelButton : MonoBehaviour
    {
        [CalledFromUnity]
        public void NewLevel()
        {
            WorkingLevel.Instance.Reset();
        }
    }
}