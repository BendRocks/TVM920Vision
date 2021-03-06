// ManagedDLL.h

#pragma once

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

//#include "msclr\marshal_cppstd.h"

// http://stackoverflow.com/questions/1827102/managed-c-to-form-a-bridge-between-c-sharp-and-c

namespace ManagedDLL {

	public ref class DoWork
	{
	public:

		void Stop()
		{
			ManagedCSharp::ManagedClass::Stop();
		}

		void SetInput(int input)
		{
			ManagedCSharp::ManagedClass::SetInput(input);
		}

		void GetData(array<unsigned char>^ buf, int* byteCount)
		{
			ManagedCSharp::ManagedClass::GetData(buf, *byteCount);
		}

		array<unsigned char>^ GetPng(int* byteCount)
		{
			return ManagedCSharp::ManagedClass::GetPng(*byteCount);
		}

		int Add(int i, int j)
			{
			return ManagedCSharp::ManagedClass::Add(i, j);
			}
	};
}

__declspec(dllexport) void __cdecl DLL_Stop()
{
	ManagedDLL::DoWork work;
	work.Stop();
}

__declspec(dllexport) void __cdecl DLL_SetInput(int input)
{
	ManagedDLL::DoWork work;
	work.SetInput(input);
}

__declspec(dllexport) void __cdecl DLL_GetData(unsigned char* buf, int* byteCount)
{
	ManagedDLL::DoWork work;
	array<unsigned char>^ mba = gcnew array<unsigned char>(720 * 576 * 4 + 4);
	//array<unsigned char>^ mba= work.GetData(buf, byteCount);
	work.GetData(mba, byteCount);

	//unsigned char * buf = new unsigned char[mba->Length];
	Marshal::Copy(mba, 0, (IntPtr)buf, mba->Length);

	//delete mba;

	//return buf;
}

__declspec(dllexport) unsigned char* __cdecl DLL_GetPng(int* byteCount)
{
	ManagedDLL::DoWork work;
	array<unsigned char>^ mba = work.GetPng(byteCount);

	unsigned char * buf = new unsigned char[mba->Length];
	Marshal::Copy(mba, 0, (IntPtr)buf, mba->Length);

	return buf;
}

__declspec(dllexport) int __cdecl DLL_Add(int i, int j)
{
	ManagedDLL::DoWork work;
	return work.Add(i, j);
}
