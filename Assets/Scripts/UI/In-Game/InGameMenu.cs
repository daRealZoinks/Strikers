using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private ConnectedMenu connectedMenu;

    private void Awake()
    {
        var connectMenu = GetComponent<ConnectMenu>();
        connectMenu.OnGameCreated += OnGameCreated;
        connectMenu.OnGameJoined += OnGameJoined;

        connectedMenu.OnGameStarted += OnGameStarted;

        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    private void OnGameCreated()
    {
        HideConnectMenu();
        ShowConnectedMenu();
    }

    private void OnGameJoined()
    {
        HideConnectMenu();
        ShowConnectedMenu();
    }

    private void HideConnectMenu()
    {
        var connectMenu = GetComponent<UIDocument>();
        connectMenu.enabled = false;
    }

    private void ShowConnectedMenu()
    {
        connectedMenu.gameObject.SetActive(true);
        connectedMenu.InitializeUi();
        connectedMenu.InitializeValues();
        connectedMenu.IsPaused = false;
    }

    private void OnClientStopped(bool wasHost)
    {
        connectedMenu.IsPaused = true;
        connectedMenu.gameObject.SetActive(false);

        var connectMenuUIDocument = GetComponent<UIDocument>();
        connectMenuUIDocument.enabled = true;

        var connectMenuScript = GetComponent<ConnectMenu>();
        connectMenuScript.Initialize();
    }

    private void OnGameStarted()
    {
    }
}