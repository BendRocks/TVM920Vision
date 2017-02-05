// DLL.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

__declspec(dllimport) void DLL_Stop();
__declspec(dllimport) void DLL_SetInput(int input);
__declspec(dllimport) void DLL_GetData(unsigned char * buffer, int * byteCount);
__declspec(dllimport) unsigned char * DLL_GetPng(int * byteCount);

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

	__declspec(dllexport) void GetData(unsigned char * buffer, int* byteCount)
	{
		DLL_GetData(buffer, byteCount);
	}

	__declspec(dllexport) unsigned char * GetPng(int* byteCount)
	{
		return DLL_GetPng(byteCount);
	}

	__declspec(dllexport) int Add(int i, int j)
	{
		return DLL_Add(i, j);
	}
}
