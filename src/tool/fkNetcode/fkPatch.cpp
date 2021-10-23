#include "fkPatch.h"
#include <stdexcept>

namespace fk
{
	Patch::Patch(LPVOID lpAddress, SIZE_T dwSize)
		: _lpAddress(static_cast<LPBYTE>(lpAddress))
		, _dwSize(dwSize)
		, dwPosition(0)
	{
		if (!_lpAddress || !_dwSize)
			throw std::invalid_argument("Address and size must not be 0.");
		if (!VirtualProtect(_lpAddress, _dwSize, PAGE_EXECUTE_READWRITE, &_flOldProtect))
			throw std::exception("VirtualProtect failed, call GetLastError for more info.");
	}

	Patch::~Patch()
	{
		close();
	}

	void Patch::close() const
	{
		DWORD oldProtect;
		if (!VirtualProtect(_lpAddress, _dwSize, _flOldProtect, &oldProtect))
			throw std::exception("VirtualProtect failed, call GetLastError for more info.");
	}
};
