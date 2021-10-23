#include <vector>
#include <Windows.h>

std::vector<HMODULE> modules;

void fkAttach()
{
	// Get executable directory.
	CHAR buffer[MAX_PATH];
	GetModuleFileName(NULL, buffer, MAX_PATH);
	*(strrchr(buffer, '\\') + 1) = '\0';

	// Attempt to load all library files matching the fk*.dll search pattern.
	lstrcat(buffer, "fk*.dll");
	WIN32_FIND_DATA findFileData;
	HANDLE hFindFile = FindFirstFile(buffer, &findFileData);
	if (hFindFile == INVALID_HANDLE_VALUE)
		return;
	do
	{
		if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
			continue;
		HMODULE hLibrary = LoadLibrary(findFileData.cFileName);
		if (hLibrary)
		{
			modules.push_back(hLibrary);
		}
		else
		{
			sprintf_s(buffer, "Could not load module %s.", findFileData.cFileName);
			MessageBox(NULL, buffer, "FrontendKit", MB_ICONWARNING);
		}
	} while (FindNextFile(hFindFile, &findFileData));
	FindClose(hFindFile);
}

void fkDetach()
{
	// Release all loaded modules.
	for (HMODULE hModule : modules)
		FreeLibrary(hModule);
	modules.clear();
}