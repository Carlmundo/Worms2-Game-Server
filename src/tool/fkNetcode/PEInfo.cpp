#include "PEInfo.h"

PEInfo::PEInfo(HMODULE hModule)
{
	Reset(hModule);
}

void PEInfo::Reset(HMODULE hModule)
{
	Handle = hModule == 0 ? GetModuleHandleA(0) : hModule;
	DOS = (IMAGE_DOS_HEADER*)Handle;
	NT = (IMAGE_NT_HEADERS*)((DWORD)DOS + DOS->e_lfanew);
	FH = (IMAGE_FILE_HEADER*)&NT->FileHeader;
	OPT = (IMAGE_OPTIONAL_HEADER*)&NT->OptionalHeader;
}

DWORD PEInfo::Offset(DWORD off)
{
	return (DWORD)Handle + off;
}

BOOL PEInfo::PtrInCode(PVOID ptr)
{
	return DWORD(ptr) >= Offset(OPT->BaseOfCode)
		&& DWORD(ptr) < Offset(OPT->BaseOfCode) + OPT->SizeOfCode;
}

BOOL PEInfo::PtrInData(PVOID ptr)
{
	return DWORD(ptr) >= Offset(OPT->BaseOfData)
		&& DWORD(ptr) < Offset(OPT->BaseOfData) + OPT->SizeOfInitializedData + OPT->SizeOfUninitializedData;
}
