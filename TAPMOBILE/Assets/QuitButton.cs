using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButtonScript : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        // In the Unity Editor, stop the play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // In a standalone build, quit the application
        Application.Quit();
#endif
    }
}
