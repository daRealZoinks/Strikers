using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private SoundCue footstepsSoundCue;

    [SerializeField] private AudioSource wallRunAudioSource;
    [SerializeField] private AudioSource inAirAudioSource;

    [SerializeField] private float wallRunSoundEffectVolumeSmoothTime = 0.8f;
    [SerializeField] private float inAirSoundEffectVolumeSmoothTime = 0.8f;

    [SerializeField] private CharacterMovementController characterMovementController;

    private void Update()
    {
        var targetWallRunSoundEffectVolume = characterMovementController.IsWallRunning ? 1 : 0;

        wallRunAudioSource.volume = Mathf.Lerp(wallRunAudioSource.volume,
            targetWallRunSoundEffectVolume,
            wallRunSoundEffectVolumeSmoothTime * Time.deltaTime);

        var playerCurrentSpeed = characterMovementController.Rigidbody.velocity.magnitude;
        var playerMaxSpeed = characterMovementController.MaxSpeed;

        var targetInAirSoundEffectVolume = playerCurrentSpeed > playerMaxSpeed ? 0 : 1;

        inAirAudioSource.volume = Mathf.Lerp(inAirAudioSource.volume,
            targetInAirSoundEffectVolume,
            inAirSoundEffectVolumeSmoothTime * Time.deltaTime);
    }

    public void PlayFootstepSound()
    {
        footstepsSoundCue.PlayRandomSound();
    }
}