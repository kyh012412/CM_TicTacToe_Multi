[How to make Simple Multiplayer Game! (FREE Course Unity Tutorial Netcode for Game Objects) TicTacToe](https://www.youtube.com/watch?v=YmUnXsOp_t0)

Universal 2D 템플릿에서 진행

### Getting Started, INstalling Packages

1. 패키지 설치
   1. 가급적 동일버전의 패키지를 설치를 권유
   2. Netcode for GameObjects
   3. Multiplayer Play mode
   4. Mulitplayer tools
2. NetworkManager 객체 설치와 cs붙여주기
   1. Network Transport 설정해주기
      1. 현재는 Unity Transport
      2. 이하에 Untiry Transport 컴포넌트가 자동으로 추가가 될것임
3. 테스트를 해보면
   1. NetworkManger는 DontDestoryOnLoad에 들어가는것을 확인
   2. inspector에서 Start Host, Start Server, Start Client 가 활성화 되는 것을 확인
4. Second 플레이어를 위해서 Build 할수도있지만
   1. Multiplayer play mode를 사용 (패키지)
   2. Player1은 Start Host를 해주고
   3. Player2에서 우상단의 layout에서 inspector와 hierarchy를 추가하여
   4. Play Mode일때 Start Client로 접속을 시도하면된다.
5. RuntimeNetworkStatsMonitor 객체를 만들어준다.
   1. cs도 붙여준다. (Multiplayer Tools를 받았다면 존재할것)
   2. 저장 후 테스트
6. 4번에서 실행한 상황을 재현
   1. 주고받은 무언가가 그래프로 보이면 성공

### ~~Companion Walkthrough~~

### Grid Setup

1. Background 객체를 만들어주고
   1. Sprite Renderer에 background를 넣어준다.
   2. Scale 13.7:16:1
   3. additional Settings 내에있는
      1. Order in layer의 숫자로 순서 정렬(숫자가 낮으면 배경화면)
2. Background 객체 안에 빈객체를 넣어준다.(Line)
   1. Sprite Renderer에 Line을 넣어준다.
   2. Draw Mode - Sliced
3. Open Sprite Editor를 눌러서 편집기를 열어주고
   1. 확인 후 다시 돌아온다
4. Line의 스케일 6 1 1
   1. Draw Mode - width 1.65, height 0.7
5. 9개의 섹션이 되도록 Line을 복사 배치
   1. y 1.5, y -1.5
   2. y 0 && x 1.5 , -1.5 && rotation z 90
6. 섹션을 감지하기 위하여별개의 square를 만들어준다.(GridPosition)
   1. scale 2.6 2.6 1
7. 클릭으로 감지할 수 있게 box collider 2d를 추가해준다.
8. Assets/Prefabs 에 Prefab 화 해준다.
9. 각 섹션별로 해당 GridPosition를 복사해준다. (편도 거리 3.1)
   1. 좌하단을 0,0좌표로 우상단을 2,2좌표로
   2. GridPosition_0_0 이런식으로 이름을 명명해준다.
10. GridPosition Prefab내의 Sprite Renderer컴포넌트만 비활성화 해준다.
11. Assets/Scripts/GridPosition를 만들어준다.
    1. `OnMouseDown`을 사용 대소문자 주의
    2. 다른방법으로는 `IPointerDownHandler` 또는 `IPointerClickHandler` 구현하는것이다.
       1. 이 방법을 사용하려면 이벤트시스템과 카메라의 Physiscs 2d Raycaster가 필요하다.
12. 카메라는 Projection - Orthographic을 사용한다.

### GameManager, Singleton Pattern

1. GameManager.cs 를 만들어 준다.
2. 싱글톤으로 만들어준뒤
3. 버튼이 눌렸을때 작동할 메서드를 만들어준뒤 GridPosition에서 호출을 해본다.

### Game Visual Manager, Spawn Objects

1. 새로운 빈객체 Cross를 만들어준다.
2. 논리와 형상을 분리해준다.
3. 부모객체가 논리를갖고 형상을 자식이 갖도록해서 분리해준다.
4. Cross 이하에 자식 객체를 추가한다.(Sprite)
   1. Sprite의 Sprite Renderer를 조정해준다.
   2. Sprite의 Scale을 2.6 2.6 1로 조정해준다.
5. Cross를 Prefab화 해준다.
   1. 복사하여 Cricle도 만들어준다.
6. GameVisualManager 를 만들어준다.
   1. Prefab들을 연결
7. GameManger에서 이벤트 정의
   1. invoke를 감싸는 메서드도 정의
   2. 타 클래스에서 해당 메서드 호출
   3. 해당 이벤트에 GameVisualManager클래스 내에서도 기능 연결
      1. 좌표를 받아와 올바른 Prefabs를 instantiate 해준다.
8. 테스트
   1. 로컬 정상
   2. 네트워크 동기화 x
9. Cross와 Circle에 NetworkObject 컴포넌트를 추가해준다.
10. 코드상에서 네트워크에 소환하는 코드
    ```cs
        private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e) {
            Transform spawnedCrossTransform = Instantiate(crossPrefab);
            spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
            spawnedCrossTransform.position = GetGridWorldPosition(e.x, e.y);
        }
    ```
11. 다만 client는 spawn을 할 수 없으므로 rpc를 이용한다.

### RPCs

1. `[Rpc(SendTo.Server)]`를써서 서버로 rpc를 보내려고하고
2. `MonoBehaviour` 대신에 `NetworkBehaviour`를써줘야 rpc를 쓸수있다.
3. `NetworkBehaviour`를 사용한 객체에서는 `NetworkObject` 컴포넌트도 넣어줘야한다.
4. 테스트
   1. Spawn은 정상적이였지만
   2. 위치가 동기화가 되지 않았다.

### Network Transform

1. 위치를 동기화하고싶다면 객체내에 Network Transform이 있어야한다.
2. Prefab Cross와 Circle에 Network Transform컴포넌트를 추가해준다.
   1. 컴포넌트 내부에 보면 동기화하고싶은것을 설정해줄수있는 체크박스들이있는데
   2. 이 이유는 네트워크는 대역폭의 문제라서 필요한만큼만 동기화할수있도록 설정할수있게 만들어준것이다.
   3. 현재는 비용의 최소를 위해 Position x,y만 체크해주고 전부 해제해준다.
3. 코드상에서 Spawn위치를 바로 지정해준다. (코드변경)
4. `Transform spawnedCrossTransform = Instantiate(crossPrefab, GetGridWorldPosition(x, y), Quaternion.identity);`

### Player Types, LocalClientId

1. GameManager내에 PlayerType인 enum을 정의
2. PlayerType자료형의 값을 가질 변수 localPlayerType을정의
3. GameManager를 NetworkBehaviour로 변경
   1. Network Object 추가
   2. OnNetworkSpawn메서드를 Override해준다.
4. NetworkManager내의 singleton에 접근해서
   1. 그 내부에 있는 LocalClientId를 사용한다.
   ```cs
       public override void OnNetworkSpawn() {
           Debug.Log("OnNetworkSpawn - NetworkManager.Singleton.LocalClientId : " + NetworkManager.Singleton.LocalClientId);
       }
   ```
5. 여기 까지 만든 후 테스트.
   1. 호스트는 호스트 개설시 0번으로
   2. 클라이언트는 start client시 1번으로 동작하는것을 확인
   3. 해당 메서드는 serverRpc이기때문에 client가 요청해도 Server에서 serverRpc가 실행되어 Server의 localPlayerType을갖기에 원하는 문양을 얻을 수 없다.
   4. 매개변수로 Prefab를 주면될수도 있지만 ServerRpc에 매개변수로
      1. Object나 Transform에 해당하는 자료형들을 보낼수없다.
      2. NetworkObject도 불가
      3. 꼭 필요하다면 NetworkObjectReference로 가능하다.
         1. 태생이 Struct이기에 가능하다.
      4. 대신에 Enum은 가능하다.
6. 테스트 -
   1. 구현한 만큼은 정상
   2. 한 명이 무제한의 턴을 가질수있음
   3. 턴의 처리에 해당하는 구현이 필요
7. enum의 첫값은 관용적으로 None이나 Null을 쓴다.
8. 현재 플레이할 사람을 담을 변수와
   1. `PlayerType currentPlayablePlayerType`
   2. 이 변수를 연결되었을때 서버에서만기준으로 초기화
   3. 조건부 분기처리

### Player UI

9. UI를 위해 캔버스 생성
10. 캔버스 이하에 빈객체 생성 PlayerUI
    1. 전체 크기
11. PlayerUI 이하에 Image 생성 (CrossImage)
    1. 위치 12시 pos x 50 y -5
    2. 크기 100 100
    3. 스프라이트 Cross
12. 복사해서 대칭위치해 Circle을 만들어주기(CircleImage)
13. PlayerUI이하에 새로운 Image추가
    1. Image컴포넌트내에 sprite none
    2. background color black
       1. 크기 480 150
14. (하이라키내에) 배치순서에 따른 렌더링 순서가 생기니 올바른 위치에 잘 놓아준다.
15. 기존 Main Camera
    1. position y를 0.6 해준다.
    2. Projection size 6
16. 기존 Background (캔버스내부가 아닌)의 Scale을 17 22로 변경
17. PlayerUI내에 Text 추가 ( Mesh Pro) (CrossYouTextMesh)
    1. 라벨 YOU
    2. 가로 세로 0 0
    3. 중앙정렬 중앙정렬
    4. NO WRAP
    5. pos x 50 y -120
    6. 대칭으로 하나 더 만들어준다. (pos x -50자리에) (CircleYouTextMesh)
18. PlayerUI내에 Image 추가 (CircleArrowImage)
    1. sprite arrow
    2. Pos X -150
19. 대칭으로 하나더 만들어 준다. (CrossArrowImage)
    1. Pos X 150
    2. rotation z 180
20. PlayerUI Script를 만들어 준다.
21. GameManger에 이벤트를 만들어준다.
    1. `public event EventHandler OnGameStarted;`
    2. `public event EventHandler OnCurrentPlayablePlayerTypeChanged;`
22. 서버에서만 실행되지않고 모두에게 실행되어야하는 부분을 다른 메서드로 뽑아주며
    1. `[Rpc(SendTo.ClientsAndHost)]`를 붙여서 사용해준다.
    2. Rpc속성을 사용한 메서드는 Rpc로 끝나야한다. (명명규칙)

### Network Variable

1. `currentPlayablePlayerType`을 네트워크 변수로 만들어 효율적으로 모두 동일한 값을 가지게 한다.
   1. 효율적이지않게 값을 변경할때마다 rpc를 보내서 같은 값을 가지게 할수도있기는 하다.
   2. Network Variable 자동으로 동기화되므로 사용하기 편하다.
2. EX)
   1. `PlayerType`인 자료형을 Network Variable로 사용하려면 `NetworkVariable<PlayerType>`로 사용해주면 된다.
3. _`NetworkVariable`는 선언할때 값을 주어야만한다._ (의무,중요)
4. `private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>(초기값,읽을수있는대상,쓰기가능한대상);`
   1. 으로 써준다.
   2. `private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>(PlayerType.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);`
   3. 기본값이 있으므로 `private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();`까지만 써주어도 된다.
5. `currentPlayablePlayerType`의 값을 사용할때는 `currentPlayablePlayerType.Value`를 사용한다.
6. `currentPlayablePlayerType`값이 서버에서 바뀌면 자동으로 값을 동기화함
7. Host또는 서버에서만 값을 바꿔주면됨
8. `OnNetworkSpawn` 내에서 `currentPlayablePlayerType.OnValueChanged`에 해당하는 메서드 추가
   1. 기존의 `OnCurrentPlayablePlayerTypeChanged`를 invoke시켜주는것을 연결
9. 테스트 - 정상
   1. 서버측에서 값을바꾸면 자동으로 동기화가되고
   2. 각자의 로컬에서 `OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);`가 적용되어 다음 상황까지 동일하게 연결

### Network Manager UI

1. Network Manager UI를 canvas이하에 만들어준다.
   1. 앵커 전체
2. NetworkManagerUI이하에 Image추가 (StartHostButton)
   1. 텍스트 라벨 HOST
   2. 배경과 폰트색 조정
   3. 버튼 크기 조정
   4. 폰트크기 65
3. 복사해서 클라이언트관련쪽도만들어주기
4. NetworkManagerUI 이하에 Image 만들기 (Background)
   1. Background를 첫번째 자식으로 만들어주기
   2. 앵커 최대
   3. 검정색 + 알파값 조금낮추기 (200)
5. NetworkManagerUI 생성 및 부착
   1. 버튼연결
   2. 버튼에 기능연결
   3. 버튼 눌릴시 UI 비활성화
6. 테스트 - 정상
   1. 새로운 문제
      1. 이미 OX가 결정된 곳에 중복으로 다른 XO를 넣을 수 있다.

### Player Type Array

1. OX 보드에 해당하는 값들을 저장할 예정
   1. GameManger내에 PlayerType이 자료형인 이중배열 3,3을만들어준뒤
   2. OX invoke 발생전 배열내의 좌표값이 None이 아니면 return을 발생
   3. 정상처리시 해당 배열에 값 부여코드를 추가해준다.
2. 테스트

### Test Winner Function

1. GameManger 내에 TestWinner funciton을 만들어준다.
2. 정상적인 OX처리후 TestWinner를 호출
3. 하이라키에 LineComplete객체를 만들어주고 LineGreen 소스를 사용한다.
   1. order의 값을 올려줘서 다른객체를 덮을수있게 해준다.
   2. 네트워크 오브젝트 컴포넌트를 추가해준다.
   3. 위치도 동기화하기 위해서 Network Transform도 추가해준다.
      1. 싱크는 Position x,y / Rotation z만 필요하다.
      2. (나머지는) 체크 해제
   4. 이것을 Prefab화 해준다.
4. 위 Prefab을 GameVisualManager에 연결해준다.
5. GameManager에 새로운 이벤트 핸들러 추가
   1. OnGameWin
      1. 누군가 이겼을때 invoke
      2. 매개변수로 누가 이겼는지 알수있게 매개변수 추가
         1. 중앙 좌표 `public Vector2Int centerGridPosition;`
6. GameVisualManager에서 OnGameWin에 액션 추가
   1. 좌표를 받아서 월드 포지션으로 변경후 그곳에 Instantiate한후
   2. NetworkObject를 가져와서 한번더 Spawn해준다.
7. GameManager 단에서 새로운 것을 정의
8. 읽기전용인 데이터 컨테이너에서는 주로 Strcut를 사용한다.(Class 보다)
   1. `public struct Line`을 만든다.
      1. 멤버 변수로는 `List<Vector2Int>`로 좌표들을 갖는 변수와
      2. 라인의 중점을 나타내는 `Vector2Int` 자료형인 변수 이렇게 2개이다.
9. 일부 코드를 Line으로 리팩토링해준다.
10. 가능한 Line들을 모아놓은 변수를 GameManager단에서 만들어준다.
11. 테스트
    1. 선의 기울기 조정이 필요
12. 새로운 enum 생성 Orientation
    1. LIne 내에도 변수추가
    2. Visual Manager단에서 Switch를 통한 분기처리

### Game Over UI

1. Canvas이하에 빈 객체 생성(Game Over UI)
   1. 앵커 전체
2. Game Over UI 이하에 Text추가
   1. 라벨 YOU WIN!
   2. 올바른 background 추가해준후
   3. 우상단에 기울여서 위치
3. GameOverUI cs 제작
   1. result Text Mesh를 연결
   2. win color와 lose color 변수 생성
   3. GameManger에 OnGameWin에 이어서 액션 구현
   4. 조건 분기처리후 Show()
4. 테스트 -
   1. 클라이언트에서 정상적인 UI가 보여지지않음
   2. 이유 -
      1. Rpc가 서버에서만 작동되어 있기 때문에
   3. 해결 ClientandHost로 Rpc를 보내어서 해결
   4. 추가 문제
      1. Rpc로 Line 자료형을 매개변수로 보낼수없다.
5. Line 구조체에 INetworkSerializable을 상속 구현해준다.

   ```cs
       public struct Line : INetworkSerializable {
           public List<Vector2Int> gridVector2IntList; // 라인을 구성하는 승리 좌표 3개
           public Vector2Int centerGridPosition;
           public Orientation orientation;

           public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
   			serializer.SerializeValue(ref gridVector2IntList); // 이부분에서 에러가 발생함 이유는 Nullable은 지원하지 않기때문에
               serializer.SerializeValue(ref centerGridPosition);
               serializer.SerializeValue(ref orientation);
           }
       }
   ```

6. 다른 방법을 사용해서 공유하는 리스트가 있으므로 index (int)를 매개변수로 처리해준다.
   1. 이렇게 하면 대역폭과 많은 돈을 아낄 수있다.
   ```cs
   	[Rpc(SendTo.ClientsAndHost)]
       private void TriggerOnGameWinRpc(int lineIndex) {
           Line line = lineList[lineIndex];
           OnGameWin?.Invoke(this, new OnGameWinEventArgs {
               line = line,
               winPlayerType = playerTypesArray[line.centerGridPosition.x, line.centerGridPosition.y],
           });
       }
   ```
7. 위 부분에서 playerTypesArray에 올바른 값은 가진건 서버뿐이다.
   1. 나머지Client들은 PlayerType.None을 가지고있으므로
   2. server단에서 조회후 반환된 값을 Rpc 호출 매개변수로 넘겨준다.
8. VisualManager에서
   1. GameManager_OnGameWin가 호출될때
      1. 서버가아니면 return으로 종료해준다.
      2. (서버에서 필요한 Network Object를 spawn후 동기화되어서 생기게된다.)

### Rematch

1. GameOverUI 이하에 RematchButton 을 만들어 준다.
2. 스크립트에서 연결
3. GameManager에서
   1. Rematch 메서드 생성
      1. 데이터 배열을 초기화 &
      2. 다음 플레이어 초기화
      3. 그리고 Rpc 서버를 붙여준다.
      4. 이름도 RematchRpc로 변경해준다.
   2. OnRematch 이벤트를 만들어준다.
4. GameVisualManager에서
   1. Rematch 제거할 요소들을 담을
      1. 변수 visualGameObjectList를 선언후
      2. 다른 Object를 생성할때마다 visualGameObjectList에 추가해준다.
5. OnRematch이벤트가 발생시 VisualManager단에서
   1. visualGameObjectList 내부 요소들을 파괴와 리스트를 Clear해준다.
6. Rematch시
   1. gameoverui가 hide되도록설정
   2. 서버에서는 목록을 삭제해주기

### Score

1. 동기화된 스코어점수를 위해서 변수
   1. 변수를 선언해준다. `NetworkVariable<int>`
   2. 네트워크 변수 선언시 선언과 동시에 초기화 필요
2. 각 유저가 승리시 적절한 변수 증가
3. PlayerUI객체 내부로가서 Text를 만들어준다. ( PlayerCrossScoreTextMesh )
4. PlayerUI.cs에서 연결
5. PlayerUI에서 승리 수변수 값을 가져와서 UI에 반영한다.
6. 테스트
   1. 네트워크 변수 딜레이로 인하여 정상적인 반영이 안되는상태
7. 새로운 이벤트 생성
   1. OnScoreChanged
8. 변수가 OnChanged 되었을때 OnScoreChanged를 invoke 시켜준다.
9. PlayerUI에서 OnScoreChanged를 가 되었을때 갱신하도록 코드를 넣어준다.

### Sound Effects and Music

1. PlaceSfx객체를 만들어준다.(하이라키에)
   1. Audio Source 컴포넌트 추가
      1. Audio Resource는 PlacingObject로
      2. Spatial Blend는 2d로
2. 해당 객체를 Prefab화 해준다.
3. SoundManager를 만들어 준다.
   1. prefab을 연결
4. 상황 발생시 Clientandhost로 Rpc 발생
   1. 모든 클라이언트가 로컬로 조건 이벤트 발동
5. 조건 이벤트 감지시
   1. 필요한 오브젝트를 생성 후 5초뒤 파괴
6. 승/패도 OnGameWin으로부터 정보를 받아서 분기처리
   1. 동일한 방법으로 Prefab을 만들어서 연결
7. 뮤직을 위한 객체와 오디오 소스 컴포넌트를 추가해준다.
   1. 루프 체크
