# HELLO_MY_WORLD
마인크래프트 모작

본 프로젝트는 마인크래프트 모작입니다. 따라서, 복셀 렌더링 방식을 이용해

게임내 블록들을 생성하여 월드를 구축합니다. 해당 알고리즘은 아래 링크를 참고하였습니다.

Reference link : http://studentgamedev.blogspot.kr/2013/08/unity-voxel-tutorial-part-1-generating.html

위 링크는, 외국의 한 블로거가 튜토리얼을 작성한겁니다.

1. 개발엔진 : Unity3D
2. 개발언어 : C#
3. 사용 Lib : Itween, NGUI 3.x, JSONObject, Sqlite3
4. Unity3d 에서 제공하는 Collider 및 충돌 기능을 쓰지 않고, 직접 만든 AABB, RayCasting, Octree를 이용해 지형 충돌을 하고 있습니다.

