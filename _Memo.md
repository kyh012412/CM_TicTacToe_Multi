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
