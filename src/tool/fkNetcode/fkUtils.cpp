#include "fkUtils.h"
#include <string>
#include "fkPatch.h"

namespace fk
{
	int getGameVersion(DWORD timeDateStamp)
	{
		switch (timeDateStamp)
		{
			case 0x3528DAFA: return GAME_VERSION_BR;
			case 0x3528DCB1: return GAME_VERSION_EN;
			case 0x3528DB52: return GAME_VERSION_GE;
			case 0x3528DA98: return GAME_VERSION_NA;
			case 0x3528DBDA: return GAME_VERSION_SA;
			case 0x3587BE19: return GAME_VERSION_TRY;
		}
		return GAME_VERSION_NONE;
	}

	std::string getErrorMessage(int error)
	{
		if (error == ERROR_SUCCESS)
			return std::string();

		LPTSTR buffer = NULL;
		const DWORD cchMsg = FormatMessageA(
			FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_ALLOCATE_BUFFER, NULL,
			error, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), reinterpret_cast<LPTSTR>(&buffer), 0, NULL);
		if (cchMsg > 0)
		{
			std::string message(buffer);
			LocalFree(buffer);
			return message;
		}
		else
		{
			CHAR buffer[32];
			sprintf_s(buffer, "Error code 0x%08X.", error);
			return buffer;
		}
	}

	void patchNops(ULONG dwAddr, SIZE_T dwPatchSize)
	{
		fk::Patch patch(reinterpret_cast<void*>(dwAddr), dwPatchSize);
		while (dwPatchSize--)
			patch.write<BYTE>(0x90);
	}

	void patchJump(PVOID pDest, SIZE_T dwPatchSize, PVOID pCallee, DWORD dwJumpType)
	{
		fk::Patch patch(pDest, dwPatchSize);

		if (dwPatchSize >= 5 && pDest)
		{
			BYTE OpSize, OpCode;
			switch (dwJumpType)
			{
				case IJ_PUSHRET: OpSize = 6; OpCode = 0x68; break;
				case IJ_FARJUMP: OpSize = 7; OpCode = 0xEA; break;
				case IJ_FARCALL: OpSize = 7; OpCode = 0x9A; break;
				case IJ_CALL: OpSize = 5; OpCode = 0xE8; break;
				default: OpSize = 5; OpCode = 0xE9; break;
			}

			if (dwPatchSize < OpSize)
				throw std::exception("Not enough space to patch opcode.");

			patch.write(OpCode);
			switch (OpSize)
			{
				case 7:
					patch.write((ULONG)pCallee);
					patch.write<WORD>(0x23);
					break;
				case 6:
					patch.write((ULONG)pCallee);
					patch.write<BYTE>(0xC3);
					break;
				default:
					patch.write((ULONG)pCallee - (ULONG)pDest - 5);
					break;
			}
			for (DWORD i = OpSize; i < dwPatchSize; i++)
				patch.write<BYTE>(0x90);
		}
	}
}