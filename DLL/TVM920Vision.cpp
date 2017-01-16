// DLL.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

__declspec(dllimport) void DLL_Stop();
__declspec(dllimport) void DLL_SetInput(int input);
__declspec(dllimport) unsigned char * DLL_GetData(int * byteCount);

__declspec(dllimport) int DLL_Add(int i, int j); 

extern "C"
{
	__declspec(dllexport) void Stop()
	{
		DLL_Stop();
	}

	__declspec(dllexport) void SetInput(int input)
	{
		DLL_SetInput(input);
	}

	__declspec(dllexport) unsigned char * GetData(int* byteCount)
	{
		return DLL_GetData(byteCount);
	}

	__declspec(dllexport) int Add(int i, int j)
	{
		return DLL_Add(i, j);
	}
}
