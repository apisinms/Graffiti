////#include "C_Select.h"
////#include "SessionManager.h"
//
//void C_Select::Select_Run()
//{
//
//	int result = 0;
//
//	// Ŭ���̾�Ʈ ��Ͼ���
//	while (1)
//	{
//		FDReset();
//		FDSet(listenSock, READ_SET);
//
//		// ���� Ŭ���̾�Ʈ ����Ʈ�� �˻����� �ƴ϶��, ����Ʈ�� ��ȸ�ϸ� �� set�� ������ �Ѵ�.
//		if (SessionManager::GetInstance()->SearchCheck() == false)
//		{
//			C_ClientInfo* info = nullptr;	// ��ȸ�� �� Ŭ���� ������ �ϳ��� ��Ƽ� Ȱ���� ����
//
//			SessionManager::GetInstance()->SearchStart();
//			while (1)
//			{
//				info = SessionManager::GetInstance()->SearchData();	// Ŭ���̾�Ʈ�� �ϳ��� �޾ƿ´�.
//
//				// ������ �� Ȯ���ߴٸ� �����Ѵ�.
//				if (info == nullptr)
//					break;
//
//				FDSet(info->GetMySocket(), READ_SET);	// �б�¿� �����Ѵ�.
//
//				// Ŭ���� ���¸� ����
//				switch (info->GetState())
//				{
//					// ����¿� �־�ߵǴ� ���¸� ����¿� �ִ´�.
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
//		Select(true, true, false, nullptr);	// select �Լ��� ȣ���Ͽ� ������� ����Ѵ�.
//
//		// Accept�� �Ǿ��ٸ� ptr�� nullptr�� �ƴ� ���� ���´�. �̸� ����Ʈ�� �߰��Ѵ�.
//		C_ClientInfo* ptr = FDAccept(listenSock);
//		if (ptr != nullptr)
//			SessionManager::GetInstance()->Insert(ptr);
//
//		// ���� Ŭ���̾�Ʈ ����Ʈ�� �˻����� �ƴ϶��, Select�� ���� ����� ó���Ѵ�.
//		if (SessionManager::GetInstance()->SearchCheck() == false)
//		{
//			C_ClientInfo* info = nullptr;	// ��ȸ�� �� Ŭ���� ������ �ϳ��� ��Ƽ� Ȱ���� ����
//
//			SessionManager::GetInstance()->SearchStart();
//			while (1)
//			{
//				info = SessionManager::GetInstance()->SearchData();	// Ŭ���̾�Ʈ�� �ϳ��� �޾ƿ´�.
//
//				// ������ �� Ȯ���ߴٸ� �����Ѵ�.
//				if (info == nullptr)
//					break;
//
//				//////////// �װ� �ƴϸ� �� �ؿ��� Select�� ����� ó���� �Ѵ� /////////////////
//
//				// read�� �� ���ߴٸ� �ٽ� �޵��� continue;
//				result = FDRead(info);
//				if (result == SOC_FALSE)
//					continue;
//
//				// write�� �� ���ߴٸ� �ٽ� �������� continue;
//				result = FDWrite(info);
//				if (result == SOC_FALSE)
//					continue;
//
//				// ptr�� ���°� ����ٸ� Ŭ������ ��� �����ش�.
//				if (info->GetState() == STATE::DISCONNECTED_STATE)
//				{
//					printf("\n[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
//						inet_ntoa(info->GetAddress().sin_addr), ntohs(info->GetAddress().sin_port));
//
//					// ���� �α׿� �������
//					LogManager::GetInstance()->ConnectFileWrite("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
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
