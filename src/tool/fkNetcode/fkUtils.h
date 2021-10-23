#pragma once
#include <stdexcept>
#include <Windows.h>

namespace fk
{
	enum GameVersion
	{
		GAME_VERSION_NONE = -1,
		GAME_VERSION_BR, // 1.05 Br
		GAME_VERSION_EN, // 1.05 Du, En, Fr, It, Po, Sp, Sw
		GAME_VERSION_GE, // 1.05
		GAME_VERSION_NA, // 1.05
		GAME_VERSION_SA, // 1.05
		GAME_VERSION_TRY // 1.07 Trymedia
	};

	enum InsertJump
	{
		IJ_JUMP, // Insert a jump (0xE9) with patchJump
		IJ_CALL, // Insert a call (0xE8) with patchJump
		IJ_FARJUMP, // Insert a farjump (0xEA) with patchJump
		IJ_FARCALL, // Insert a farcall (0x9A) with patchJump
		IJ_PUSHRET, // Insert a pushret with patchJump
	};

	int getGameVersion(DWORD timeDateStamp);
	std::string getErrorMessage(int error);

	void patchNops(ULONG dwAddr, SIZE_T dwPatchSize);
	void patchJump(PVOID pDest, SIZE_T dwPatchSize, PVOID pCallee, DWORD dwJumpType = IJ_JUMP);
}
