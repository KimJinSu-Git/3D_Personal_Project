# ⚔️ Unity 3D RPG 시스템 구현 중심의 학습 프로젝트
🛠 개발 도구: Unity(C#), JetBrains Rider <br/>
📆 개발 기간: 25.04.14 ~ 25.04.28 (약 2주) <br/>
___
Unity 기반으로 RPG의 기본 시스템을 직접 구현하며 기초를 다진 개인 프로젝트입니다. <br/>
캐릭터의 상태 전환, NPC 대화, 퀘스트 진행, 상점 시스템 등을 포함하여 RPG 장르의 핵심 기능을 학습하고 구현했습니다. <br/>
___
🔑 주요 구현 요소
* FSM 기반 Player·Monster 전투 시스템
* 인벤토리 시스템 👉 [InventoryManager.cs](https://github.com/KimJinSu-Git/3D_Personal_Project/blob/main/3D_Project_RPG/Assets/Scripts/Item/Inventory/InventoryManager.cs)
  * List 기반의 20칸짜리 인벤토리 슬롯 구현 
  * 아이템을 드래그해서 다른 슬롯으로 이동
* 퀵슬롯 시스템 👉 [QuickSlotManager](https://github.com/KimJinSu-Git/3D_Personal_Project/blob/main/3D_Project_RPG/Assets/Scripts/Item/Inventory/QuickSlotManager.cs)
  * 인벤토리에서 드래그를 통해 큇슬롯 등록
  * 해당 키 입력을 통해 아이템 사용
* 상점 시스템
  * 가진 Gold를 소모하여 포션 구매
  * 아이템 구매 시 인벤토리를 체크하여 중복된 아이템이 존재하는 지 확인 👉 [InventoryManager.cs](https://github.com/KimJinSu-Git/3D_Personal_Project/blob/main/3D_Project_RPG/Assets/Scripts/Item/Inventory/InventoryManager.cs) => AddItem()
  * 인벤토리에서 아이템을 우클릭하여 아이템 판매
* NPC 대화 시스템
  * G 키를 통해 NPC와 대화
  * dialogue ID를 통해 CSV에서 NPC의 대사를 호출
* 퀘스트 시스템
  * 퀘스트 NPC와 대화를 통해 퀘스트 연계
  * Event 호출을 통한 퀘스트 진행 현황 동기화 👉 [QuestManager.cs](https://github.com/KimJinSu-Git/3D_Personal_Project/blob/main/3D_Project_RPG/Assets/Scripts/Quest/QuestManager.cs)
* Json, CSV 기반 저장 및 로드
___
* 영상 바로가기 [RPG.avi](https://drive.google.com/file/d/1ft5Vmcbp2HLU-rUg7bm2hKi7oNjKIkEu/view?usp=drive_link)
* 문서 바로가기 [RPG.pdf](https://drive.google.com/file/d/1HnQlISH36RC1Cdsj5P0eS_aId1hCV_Jg/view?usp=drive_link)
