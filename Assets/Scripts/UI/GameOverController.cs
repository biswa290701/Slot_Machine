using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Placed in the GameOver scene. Plays the game-over sound on load and
/// provides the RestartGame callback wired to the Restart button.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [SerializeField] private AudioClip gameOverClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;

        if (gameOverClip != null)
        {
            audioSource.clip = gameOverClip;
            audioSource.Play();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
