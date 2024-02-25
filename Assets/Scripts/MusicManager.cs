using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    private AudioSource _audioSource;
    private int _currentSongIndex;

    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = songs[0];
        _audioSource.Play();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Next Song"))
        {
            GoToRandomSong();
        }
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            GoToRandomSong();
        }
    }

    public static void GoToRandomSong()
    {
        Instance._currentSongIndex = Random.Range(0, Instance.songs.Length);
        Instance._audioSource.clip = Instance.songs[Instance._currentSongIndex];
        Instance._audioSource.Play();

        Debug.LogFormat("Playing song {0}", Instance._currentSongIndex);
    }
}