#pragma once
#include <Windows.h>

namespace fk
{
	struct Patch
	{
	public:
		DWORD dwPosition;

		Patch(LPVOID lpAddress, SIZE_T dwSize);
		~Patch();

		void close() const;
		template <class T> void write(const T& value);

	private:
		LPBYTE _lpAddress;
		SIZE_T _dwSize;
		DWORD _flOldProtect;
	};
}

#include "fkPatch.inl"
