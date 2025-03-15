using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    // Audio source component reference
    private AudioSource audioSource;
    
    // Optional: Expose these parameters in the inspector
    [Header("Music Settings")]
    [Tooltip("The audio clip to play as background music")]
    public AudioClip musicClip;
    
    [Range(0f, 1f)]
    [Tooltip("Volume level of the background music")]
    public float volume = 0.5f;
    
    [Tooltip("Whether the music should loop")]
    public bool loopMusic = true;
    
    [Tooltip("Whether music should persist between scenes")]
    public bool dontDestroyOnLoad = true;
    
    // Static instance to ensure we only have one music player
    private static BackgroundMusic instance;
    
    void Awake()
    {
        // Singleton pattern to prevent multiple music players
        if (instance == null)
        {
            // This is the first instance - make it the singleton
            instance = this;
            
            // Don't destroy this object when loading a new scene
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
                
            // Set up the audio source
            SetupAudioSource();
        }
        else
        {
            // Another instance already exists - destroy this one
            Destroy(gameObject);
        }
    }
    
    void SetupAudioSource()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Configure the audio source
        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = loopMusic;
        audioSource.playOnAwake = true;
        
        // Start playing if we have a clip
        if (musicClip != null)
            audioSource.Play();
        else
            Debug.LogWarning("No music clip assigned to BackgroundMusic component");
    }
    
    // Public methods to control music
    
    public void SetVolume(float newVolume)
    {
        if (audioSource != null)
            audioSource.volume = Mathf.Clamp01(newVolume);
    }
    
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }
    
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }
    
    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
    
    // Optional: respond to scene changes
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // You can adjust music based on scene if desired
        // For example, if (scene.name == "Game") { SetVolume(0.8f); }
    }
}