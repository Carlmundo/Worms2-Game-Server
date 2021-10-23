#pragma once
#include <Windows.h>

typedef struct PEInfo
{
	PEInfo(HMODULE hModule = 0);

	void Reset(HMODULE hModule);
	DWORD Offset(DWORD off);
	BOOL PtrInCode(PVOID ptr);
	BOOL PtrInData(PVOID ptr);

	HANDLE Handle;
	IMAGE_DOS_HEADER* DOS;
	IMAGE_NT_HEADERS* NT;
	IMAGE_FILE_HEADER* FH;
	IMAGE_OPTIONAL_HEADER* OPT;
} *PPEInfo;
