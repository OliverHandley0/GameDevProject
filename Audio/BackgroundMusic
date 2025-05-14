using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PersistentAudio : MonoBehaviour
{
    public AudioClip audioClipToPlay;            // Audio clip to play, assign in Inspector
    public bool persistAcrossScenes = true;      // Keep playing across scene loads if true

    private AudioSource audioSource;             // Cached reference to AudioSource component
    private static PersistentAudio instance = null;  // Singleton instance to prevent duplicates

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Ensure only one PersistentAudio exists when persisting across scenes
        if (persistAcrossScenes)
        {
            if (instance == null)
            {
                instance = this;                // First instance becomes the singleton
                DontDestroyOnLoad(gameObject); // Prevent this GameObject from being destroyed
            }
            else if (instance != this)
            {
                Destroy(gameObject);           // Destroy any extra instances
                return;                        // Stop further initialization
            }
        }

        // Initialize the audio source component
        audioSource = GetComponent<AudioSource>();

        // Assign the clip if set; if none, warn and abort
        if (audioClipToPlay != null)
        {
            audioSource.clip = audioClipToPlay;
        }
        else if (audioSource.clip == null)
        {
            Debug.LogWarning("No AudioClip assigned; audio will not play.");
            return;
        }

        // Configure looping and auto-play
        audioSource.loop = true;        
        audioSource.playOnAwake = true;

        // Start playback if not already playing
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
