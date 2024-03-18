using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private float walkingInfluence = 1f;
    [SerializeField] private float lookingInfluence = 1f;
    [SerializeField] private float smoothingFactor = 10f;
    [SerializeField] private float baseScale = 30f;

    private GunManager _gunManager;
    private CharacterMovementController _characterMovementController;

    private VisualElement crosshair;
    private Label ammoLeft;
    private Label maxAmmo;

    private void Awake()
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        _gunManager = player.GetComponentInChildren<GunManager>();
        _characterMovementController = player.GetComponent<CharacterMovementController>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        crosshair = root.Q<VisualElement>("Crosshair");

        ammoLeft = root.Q<Label>("AmmoLeft");
        maxAmmo = root.Q<Label>("MaxAmmo");
    }

    private void Update()
    {
        if (_gunManager != null)
        {
            ammoLeft.text = _gunManager.currentWeapon.CurrentAmmo.ToString();
            maxAmmo.text = _gunManager.currentWeapon.MaxAmmo.ToString();
        }

        _characterMovementController = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<CharacterMovementController>();

        var lookInputMagnitude = _characterMovementController.LookInput.magnitude;
        var velocityMagnitude = _characterMovementController.Rigidbody.velocity.magnitude;

        var crosshairScale = (lookInputMagnitude * lookingInfluence) + (velocityMagnitude * walkingInfluence);

        float targetWidth = baseScale + crosshairScale;
        float targetHeight = baseScale + crosshairScale;

        crosshair.style.width = Mathf.Lerp(crosshair.style.width.value.value, targetWidth, Time.deltaTime * smoothingFactor);
        crosshair.style.height = Mathf.Lerp(crosshair.style.height.value.value, targetHeight, Time.deltaTime * smoothingFactor);
    }
}
