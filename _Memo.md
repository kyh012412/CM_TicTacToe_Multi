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
