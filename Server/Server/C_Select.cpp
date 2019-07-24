////#include "C_Select.h"
////#include "SessionManager.h"
//
//void C_Select::Select_Run()
//{
//
//	int result = 0;
//
//	// 클라이언트 목록얻어옴
//	while (1)
//	{
//		FDReset();
//		FDSet(listenSock, READ_SET);
//
//		// 만약 클라이언트 리스트가 검색중이 아니라면, 리스트를 순회하며 각 set에 셋팅을 한다.
//		if (SessionManager::GetInstance()->SearchCheck() == false)
//		{
//			C_ClientInfo* info = nullptr;	// 순회할 때 클라의 정보를 하나씩 담아서 활용할 변수
//
//			SessionManager::GetInstance()->SearchStart();
//			while (1)
//			{
//				info = SessionManager::GetInstance()->SearchData();	// 클라이언트를 하나씩 받아온다.
//
//				// 끝까지 다 확인했다면 종료한다.
//				if (info == nullptr)
//					break;
//
//				FDSet(info->GetMySocket(), READ_SET);	// 읽기셋에 셋팅한다.
//
//				// 클라의 상태를 봐서
//				switch (info->GetState())
//				{
//					// 쓰기셋에 넣어야되는 상태면 쓰기셋에 넣는다.
//				case STATE::JOIN_STATE:
//				case STATE::LOGIN_STATE:
//				case STATE::LOGOUT_STATE:
//					FDSet(info->GetMySocket(), WRITE_SET);
//					break;
//				}
//			}
//			SessionManager::GetInstance()->SearchEnd();
//		}
//
//		Select(true, true, false, nullptr);	// select 함수를 호출하여 입출력을 대기한다.
//
//		// Accept가 되었다면 ptr이 nullptr이 아닌 값이 들어온다. 이를 리스트에 추가한다.
//		C_ClientInfo* ptr = FDAccept(listenSock);
//		if (ptr != nullptr)
//			SessionManager::GetInstance()->Insert(ptr);
//
//		// 만약 클라이언트 리스트가 검색중이 아니라면, Select에 대한 결과를 처리한다.
//		if (SessionManager::GetInstance()->SearchCheck() == false)
//		{
//			C_ClientInfo* info = nullptr;	// 순회할 때 클라의 정보를 하나씩 담아서 활용할 변수
//
//			SessionManager::GetInstance()->SearchStart();
//			while (1)
//			{
//				info = SessionManager::GetInstance()->SearchData();	// 클라이언트를 하나씩 받아온다.
//
//				// 끝까지 다 확인했다면 종료한다.
//				if (info == nullptr)
//					break;
//
//				//////////// 그게 아니면 이 밑에서 Select의 결과를 처리를 한다 /////////////////
//
//				// read를 다 못했다면 다시 받도록 continue;
//				result = FDRead(info);
//				if (result == SOC_FALSE)
//					continue;
//
//				// write를 다 못했다면 다시 보내도록 continue;
//				result = FDWrite(info);
//				if (result == SOC_FALSE)
//					continue;
//
//				// ptr의 상태가 끊겼다면 클라정보 찍고 지워준다.
//				if (info->GetState() == STATE::DISCONNECTED_STATE)
//				{
//					printf("\n[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n",
//						inet_ntoa(info->GetAddress().sin_addr), ntohs(info->GetAddress().sin_port));
//
//					// 퇴장 로그에 기록해줌
//					LogManager::GetInstance()->ConnectFileWrite("[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n",
//						inet_ntoa(info->GetAddress().sin_addr), ntohs(info->GetAddress().sin_port));
//
//					SessionManager::GetInstance()->Delete(info);
//					continue;
//				}
//			}
//			SessionManager::GetInstance()->SearchEnd();
//		}
//	}
//}
