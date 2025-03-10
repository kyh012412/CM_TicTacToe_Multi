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
