using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

    public class OnClickedOnGridPositionEventArgs : EventArgs {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;

    public enum PlayerType {
        None,
        Cross,
        Circle,
    };

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More thane One GameManager instance!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn() { // 네트워크에 연결됬을때 자동으로 0,1,2,3의 값을 차례로 받음
        // Debug.Log("OnNetworkSpawn - NetworkManager.Singleton.LocalClientId : " + NetworkManager.Singleton.LocalClientId);

        if (NetworkManager.Singleton.LocalClientId == 0) {
            localPlayerType = PlayerType.Cross;
        } else {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayablePlayerType.OnValueChanged += currentPlayablePlayerType_OnValueChanged;
    }

    private void currentPlayablePlayerType_OnValueChanged(PlayerType previousValue, PlayerType newValue) {
        OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj) { // obj는 클라이언트식별 아이디에 해당하는 값
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            // Start Game
            currentPlayablePlayerType.Value = PlayerType.Cross; // 크로스가 선
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc() {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType) {
        Debug.Log("ClickedOnGridPosition " + x + ", " + y);
        if (playerType != currentPlayablePlayerType.Value) {
            return;
        }

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs {
            x = x,
            y = y,
            playerType = playerType,
        });

        switch (currentPlayablePlayerType.Value) {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
    }

    public PlayerType GetLocalPlayerType() {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType() {
        return currentPlayablePlayerType.Value;
    }
}
