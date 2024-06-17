using UnityEngine;
using UnityEngine.VFX;

public class AnimeLinesController : MonoBehaviour
{
    [SerializeField]
    private CharacterMovementController characterMovementController;

    [SerializeField]
    private VisualEffect animeLines;

    [SerializeField]
    private float maxSpeed = 30f;

    private void Start()
    {
        animeLines.enabled = true;
        animeLines.Stop();
    }

    private void Update()
    {
        var velocityMagnitude = characterMovementController.Rigidbody.velocity.magnitude;

        var isOverMaxSpeed = velocityMagnitude > maxSpeed;

        if (isOverMaxSpeed)
        {
            animeLines.Play();
        }
        else
        {
            animeLines.Stop();
        }
    }
}
