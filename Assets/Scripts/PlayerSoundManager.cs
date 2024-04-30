using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private SoundCue footstepsSoundCue;
    [SerializeField] private AudioSource wallRunSound;

    public void PlayFootstepSound()
    {
        footstepsSoundCue.PlayRandomSound();
    }

    public void StopWallRunSound()
    {
        wallRunSound.mute = true;
    }

    public void PlayWallRunSound()
    {
        wallRunSound.mute = false;
    }
}