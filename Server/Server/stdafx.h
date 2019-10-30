/*
미리 컴파일 된 헤더
컴파일 시간을 줄여준다. 주로 크기가 크고, 소스가 변경될 일이 없는 헤더를 포함시킨다.
*/
#pragma once

// libraries
#pragma comment(lib, "ws2_32")
#include <WinSock2.h>
#include <Windows.h>
#include<iostream>
#include <stdio.h>
#include <time.h>
#include <tchar.h>
#include <process.h>
#include <locale.h>
#include <mstcpip.h>
#include <random>

// STL
#include <list>
#include <vector>
#include <queue>
#include <algorithm>

// custom STL
#include "C_LinkedList.h"
#include "C_List.h"
#include "C_Node.h"
#include "C_Stack.h"

// Network Model
#include "C_IOCP.h"

// Utility
#include "C_ComfortableCS.h"
#include "C_SyncCS.h"

// Encryption
#include "C_Encrypt.h"

// ETC
#include "LogManager.h"
#include "UtilityManager.h"
#include "SessionManager.h"

// state
#include "C_State.h"

// Database
#include "DatabaseManager.h"