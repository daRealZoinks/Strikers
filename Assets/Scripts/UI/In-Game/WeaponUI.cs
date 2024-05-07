using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private float walkingInfluence = 3f;
    [SerializeField] private float lookingInfluence = 4f;
    [SerializeField] private float smoothingFactor = 10f;
    [SerializeField] private float baseScale = 30f;

    private GunManager _gunManager;
    private CharacterMovementController _characterMovementController;

    private VisualElement _crosshair;
    private Label _ammoLeft;
    private Label _maxAmmo;

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _crosshair = root.Q<VisualElement>("Crosshair");

        _ammoLeft = root.Q<Label>("AmmoLeft");
        _maxAmmo = root.Q<Label>("MaxAmmo");
    }

    private void Update()
    {
        var localClientPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (!localClientPlayerObject) return;

        _gunManager = localClientPlayerObject.GetComponentInChildren<GunManager>();

        if (_gunManager)
        {
            UpdateAmmoCountDisplay();
        }

        _characterMovementController = localClientPlayerObject.GetComponent<CharacterMovementController>();

        if (_characterMovementController)
        {
            AdjustCrosshairSize();
        }
    }

    private void UpdateAmmoCountDisplay()
    {
        _ammoLeft.text = _gunManager.currentWeapon.CurrentAmmo.ToString();
        _maxAmmo.text = _gunManager.currentWeapon.MaxAmmo.ToString();
    }

    private void AdjustCrosshairSize()
    {
        var lookInputMagnitude = _characterMovementController.LookInput.magnitude;
        var velocityMagnitude = _characterMovementController.Rigidbody.velocity.magnitude;

        var crosshairScale = lookInputMagnitude * lookingInfluence + velocityMagnitude * walkingInfluence;

        var targetWidth = baseScale + crosshairScale;
        var targetHeight = baseScale + crosshairScale;

        var factor = Time.deltaTime * smoothingFactor;

        _crosshair.style.width = Mathf.Lerp(_crosshair.style.width.value.value, targetWidth, factor);
        _crosshair.style.height = Mathf.Lerp(_crosshair.style.height.value.value, targetHeight, factor);
    }
}