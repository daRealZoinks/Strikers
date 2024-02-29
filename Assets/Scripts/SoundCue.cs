using UnityEngine;

public class SoundCue : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        var randomIndex = Random.Range(0, audioClips.Length);
        _audioSource.PlayOneShot(audioClips[randomIndex]);
    }
}
