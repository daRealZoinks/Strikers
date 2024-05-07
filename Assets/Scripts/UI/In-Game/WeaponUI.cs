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

    private VisualElement _crosshair;
    private Label _ammoLeft;
    private Label _maxAmmo;

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

        _crosshair = root.Q<VisualElement>("Crosshair");

        _ammoLeft = root.Q<Label>("AmmoLeft");
        _maxAmmo = root.Q<Label>("MaxAmmo");
    }

    private void Update()
    {
        if (_gunManager != null)
        {
            _ammoLeft.text = _gunManager.currentWeapon.CurrentAmmo.ToString();
            _maxAmmo.text = _gunManager.currentWeapon.MaxAmmo.ToString();
        }

        _characterMovementController = NetworkManager.Singleton.LocalClient.PlayerObject
            .GetComponentInChildren<CharacterMovementController>();

        var lookInputMagnitude = _characterMovementController.LookInput.magnitude;
        var velocityMagnitude = _characterMovementController.Rigidbody.velocity.magnitude;

        var crosshairScale = (lookInputMagnitude * lookingInfluence) + (velocityMagnitude * walkingInfluence);

        float targetWidth = baseScale + crosshairScale;
        float targetHeight = baseScale + crosshairScale;

        _crosshair.style.width =
            Mathf.Lerp(_crosshair.style.width.value.value, targetWidth, Time.deltaTime * smoothingFactor);
        _crosshair.style.height = Mathf.Lerp(_crosshair.style.height.value.value, targetHeight,
            Time.deltaTime * smoothingFactor);
    }
}