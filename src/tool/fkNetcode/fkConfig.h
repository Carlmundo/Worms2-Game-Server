#pragma once
#include <Windows.h>

namespace fk
{
	class Config
	{
	public:
		Config(LPCSTR fileName);

		void get(LPCSTR category, LPCSTR key, BOOL& result, UINT fallback) const;
		void get(LPCSTR category, LPCSTR key, UINT& result, UINT fallback) const;
		void get(LPCSTR category, LPCSTR key, LPSTR result, INT resultLength, LPCSTR fallback = NULL) const;
		void set(LPCSTR category, LPCSTR key, UINT value) const;
		void set(LPCSTR category, LPCSTR key, LPCSTR value) const;

	private:
		CHAR _filePath[MAX_PATH];
	};
}
