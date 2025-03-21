using System;
using System.Collections.Generic;
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
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs {
        public Line line;
        public PlayerType winPlayerType;
    }
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    public event EventHandler OnRematch;

    public enum PlayerType {
        None,
        Cross,
        Circle,
    };

    public enum Orientation {
        Horizontal,
        Vertical,
        DiagonalA, // 좌하단 to 우상단
        DiagonalB, // 좌상단 to 우하단
    }

    public struct Line {
        public List<Vector2Int> gridVector2IntList; // 라인을 구성하는 승리 좌표 3개
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] playerTypesArray;
    private List<Line> lineList;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More thane One GameManager instance!");
        }
        Instance = this;

        playerTypesArray = new PlayerType[3, 3];
        lineList = new List<Line> {
            // Horizontal
            new Line{ // 바닥 라인
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0),
                },
                centerGridPosition = new Vector2Int(1,0),
                orientation = Orientation.Horizontal,
            },
            new Line{ // 중간 라인 (가로)
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1),
                },
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.Horizontal,
            },
            new Line{ // 상단 라인
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2),
                },
                centerGridPosition = new Vector2Int(1,2),
                orientation = Orientation.Horizontal,
            },

            // Vertical
            new Line{ // 좌측 라인
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(0,2),
                },
                centerGridPosition = new Vector2Int(0,1),
                orientation = Orientation.Vertical,
            },
            new Line{ // 중간 라인 (세로)
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(1,0),new Vector2Int(1,1),new Vector2Int(1,2),
                },
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.Vertical,
            },
            new Line{ // 우측 라인
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(2,2),
                },
                centerGridPosition = new Vector2Int(2,1),
                orientation = Orientation.Vertical,
            },

            // Diagonals
            new Line{ // 좌하단 to 우상단
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,0),new Vector2Int(1,1),new Vector2Int(2,2),
                },
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.DiagonalA,
            },
            new Line{ // 좌상단 to 우하단
                gridVector2IntList = new List<Vector2Int>{
                    new Vector2Int(0,2),new Vector2Int(1,1),new Vector2Int(2,0),
                },
                centerGridPosition = new Vector2Int(1,1),
                orientation = Orientation.DiagonalB,
            },
        };
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

        if (playerTypesArray[x, y] != PlayerType.None) {
            // Already occupied
            return;
        }

        playerTypesArray[x, y] = playerType;

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

        TestWinner(); // 해당 메서드가 Server 내에서 호출되기 때문에 서버에서만 작동됨
    }

    private bool TestWinnerLine(Line line) {
        return TestWinnerLine(
            playerTypesArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypesArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypesArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
            );
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType) {
        return aPlayerType != PlayerType.None &&
        aPlayerType == bPlayerType &&
        bPlayerType == cPlayerType;
    }

    private void TestWinner() {
        for (int i = 0; i < lineList.Count; i++) {
            Line line = lineList[i];
            if (TestWinnerLine(line)) {
                // Win !
                Debug.Log("Winner!");
                currentPlayablePlayerType.Value = PlayerType.None;

                TriggerOnGameWinRpc(i, playerTypesArray[line.centerGridPosition.x, line.centerGridPosition.y]);
                break;
            }
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType) {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs {
            line = line,
            winPlayerType = winPlayerType,
        });
    }

    public PlayerType GetLocalPlayerType() {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType() {
        return currentPlayablePlayerType.Value;
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc() {
        for (int x = 0; x < playerTypesArray.GetLength(0); x++) {
            for (int y = 0; y < playerTypesArray.GetLength(1); y++) {
                playerTypesArray[x, y] = PlayerType.None;
            }
        }

        currentPlayablePlayerType.Value = PlayerType.Cross; //랜덤으로하거나 패배자 선으로 할수있다.

        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerOnRematchRpc() {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }
}
