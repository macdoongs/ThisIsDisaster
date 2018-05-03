using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Collections;

using NetworkComponents;

public class NetworkSetupControl : MonoBehaviour
{
    public enum Step {
        None = -1,

        Wait = 0,
        ServerStart,
        ServerConnect,
        ClientConnect,
        Prepare,
        Started,
        Error,
        Restart,

        Dummy,//for indexing
    }

    public InputField AddressField;
    private bool _isHost = false;

    private Step _currentStep = Step.None;
    private Step _nextStep = Step.None;
    private float _waitTime = 1f;

    Timer _waitTimer = new Timer();
    
    // Use this for initialization
    void Start()
    {
        Initialize();

    }

    public void Initialize()
    {
        AddressField.text = GetLocalHost();

        _currentStep = Step.None;
        _nextStep = Step.Wait;

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_waitTimer.started)
        {
            _waitTimer.RunTimer();
        }

        while (_nextStep != Step.None) {
            _currentStep = _nextStep;
            _nextStep = Step.None;

            ExecuteStep();
            _waitTimer.StartTimer(_waitTime);
        }

        SetNextStep();
    }

    void ExecuteStep() {
        switch (_currentStep) {
            case Step.ServerStart:
                ServerStart();
                break;
            case Step.ServerConnect:
                ServerConnect();
                break;
            case Step.ClientConnect:
                ClientConnect();
                break;
            case Step.Restart:
                ResetNetwork();
                break;
        }
        
    }

    void SetNextStep() {
        switch (_currentStep) {
            case Step.ServerStart:
                if (_waitTimer.started) return;
                _nextStep = Step.ServerConnect;
                break;
            case Step.ServerConnect:
                _nextStep = Step.ClientConnect;
                break;
            case Step.ClientConnect:
                _nextStep = Step.Prepare;
                break;
            case Step.Prepare:
                //check sync
                if (CheckGameSyncState()) {
                    _nextStep = Step.Started;
                }
                break;
            case Step.Error:
                _nextStep = Step.Restart;
                break;
            case Step.Wait:
            case Step.Restart:
                //do nothing
                break;
        }
    }

    public void ServerStart()
    {
        int port = _isHost ? NetConfig.GAME_PORT : NetConfig.GAME_PORT + 1;
        bool state = StartServer(port);
        if (_isHost) {
            NetworkModule.Instance.StartGameServer();
        }
        LogStepState(state, Step.ServerStart);
        if (!state) {
            _currentStep = Step.Error;
        }
    }

    public void ServerConnect() {
        string serverAddress = GetHostAddress();
        if (_isHost) {
            serverAddress = GetLocalHost();
        }
        bool state = Connect(serverAddress, NetConfig.SERVER_PORT, NetworkModule.ConnectionType.TCP);

        LogStepState(state, Step.ServerConnect);
        if (!state) {
            _currentStep = Step.Error;
        }
    }

    public void ClientConnect() {
        int port = _isHost ? NetConfig.GAME_PORT + 1 : NetConfig.GAME_PORT;
        bool state = Connect(GetHostAddress(), port, NetworkModule.ConnectionType.UDP);
        LogStepState(state, Step.ClientConnect);
        if (!state) {
            _currentStep = Step.Error;
        }
    }

    public void OnClickHost() {
        _isHost = true;
        _nextStep = Step.ServerStart;
    }

    public void OnClickConnect()
    {
        _isHost = false;
        _nextStep = Step.ServerStart;
    }

    bool StartServer(int port) {
        return NetworkModule.Instance.StartServer(port, NetworkModule.ConnectionType.UDP);
    }

    bool Connect(string addr, int port, NetworkModule.ConnectionType type) {
        return NetworkModule.Instance.Connect(addr, port, type);
    }

    string GetHostAddress() {
        ///TODO : add some check ip structure
        return AddressField.text;
    }

    string GetLocalHost() {
        string hostAddress = "";
        string hostName = Dns.GetHostName();
        IPAddress[] addrList = Dns.GetHostAddresses(hostName);
        hostAddress = addrList[0].ToString();
        return hostAddress;
    }

    void ResetNetwork() {
        var net = NetworkModule.Instance;
        net.Disconnect();
        net.StopGameServer();
        net.StopServer();
    }

    public bool CheckGameSyncState() {
        return true;
    }

    void LogStepState(bool state, Step step) {
        if (state)
        {
            Debug.Log("Success to " + step.ToString());
        }
        else {
            Debug.LogError("Failed to " + step.ToString());
        }
    }
}
