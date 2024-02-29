using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    private AudioSource _audioSource;
    private List<AudioClip> _availableSongs = new();

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
        GoToRandomSong();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Next Song"))
        {
            GoToRandomSong();
        }
    }
#endif

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            GoToRandomSong();
        }
    }

    public static void GoToRandomSong()
    {
        if (Instance._availableSongs.Count == 0)
        {
            Instance._availableSongs = Instance.songs.ToList();
        }

        var currentSongIndex = Random.Range(0, Instance._availableSongs.Count);
        Instance._audioSource.clip = Instance._availableSongs[currentSongIndex];
        Instance._audioSource.Play();

        Instance._availableSongs.RemoveAt(currentSongIndex);
    }
}