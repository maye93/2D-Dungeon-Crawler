using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelTrigger : MonoBehaviour
{
    public float fadeDuration = 1f; // Duration of the fade effect
    public AudioSource transitionAudio; // Reference to the audio source for transition sound

    private bool isTransitioning = false; // Flag to prevent multiple transitions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionToNextLevel());
        }
    }

    IEnumerator TransitionToNextLevel()
    {
        // Stop all activities in the scene
        Time.timeScale = 0f;

        // Get the UI canvas
        GameObject uiCanvas = GameObject.FindGameObjectWithTag("UI");
        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas not found.");
            yield break;
        }

        // Create a black image for fading
        GameObject fadeImageObj = new GameObject("FadeImage");
        fadeImageObj.transform.SetParent(uiCanvas.transform, false);
        Image fadeImage = fadeImageObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        // Set the size of the image to match the screen size
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        // Fade to black
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // Use unscaled time for the fade
            fadeImage.color = Color.Lerp(Color.clear, Color.black, timer / fadeDuration);
            yield return null;
        }

        // Play transition audio
        if (transitionAudio != null)
            transitionAudio.Play();

        // Wait for the audio to finish playing
        yield return new WaitForSecondsRealtime(transitionAudio.clip.length);

        // Delete the black image
        Destroy(fadeImageObj);

        // Resume time scale
        Time.timeScale = 1f;

        // Load the next scene
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}