using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartLevel()
    {
        // Load the level 1 scene
        SceneManager.LoadScene("Level 1");
    }
}