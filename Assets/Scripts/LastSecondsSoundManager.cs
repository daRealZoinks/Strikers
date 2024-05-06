using UnityEngine;

public class LastSecondsSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource lastSecondsAudioSource;

    private void Awake()
    {
        Timer.Instance.OnTimeChanged += PlayLastSecondsSound;
    }

    private void PlayLastSecondsSound(int time)
    {
        if (time <= 10)
        {
            lastSecondsAudioSource.Play();
        }
    }
}
