using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private SoundCue footstepsSoundCue;

    public void PlayFootstepSound()
    {
        footstepsSoundCue.PlayRandomSound();
    }
}