#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <WinInet.h>
#include <winsock.h>
#include "fkConfig.h"
#include "fkUtils.h"
#include "PEInfo.h"

// ---- Configuration ----

CHAR cfgFallbackIP[16];
CHAR cfgServiceUrl[MAX_PATH];
BOOL cfgShowErrors;

void configure()
{
	fk::Config config("fkNetcode.ini");

	// Load INI settings.
	config.get("AddressResolval", "FallbackIP", cfgFallbackIP, 16);
	config.get("AddressResolval", "ServiceUrl", cfgServiceUrl, MAX_PATH, "http://ip.syroot.com");
	config.get("AddressResolval", "ShowErrors", cfgShowErrors, TRUE);

	// Ensure INI file has been created with default setting.
	config.set("AddressResolval", "FallbackIP", cfgFallbackIP);
	config.set("AddressResolval", "ServiceUrl", cfgServiceUrl);
	config.set("AddressResolval", "ShowErrors", cfgShowErrors);

	// Validate fallback IP.
	BYTE b;
	if (*cfgFallbackIP && sscanf_s(cfgFallbackIP, "%hhu.%hhu.%hhu.%hhu", &b, &b, &b, &b) != 4)
	{
		*cfgFallbackIP = NULL;
		MessageBox(NULL, "Invalid fallback IP setting in fkNetcode.ini has been ignored.", "fkNetcode", MB_ICONWARNING);
	}
}

// ---- Patch: IP Resolval ----

CHAR cachedIP[16] = {};

bool resolveIPCached(LPSTR buffer)
{
	if (!*cachedIP)
		return false;
	lstrcpy(buffer, cachedIP);
	return true;
}

bool resolveIPExternal(LPSTR buffer)
{
	if (!*cfgServiceUrl)
		return false;

	// Query a web service which replies with the IP in plain text.
	HINTERNET hInternet = 0, hFile = 0;
	if (hInternet = InternetOpen(NULL, INTERNET_OPEN_TYPE_DIRECT, NULL, NULL, 0))
	{
		if (hFile = InternetOpenUrl(hInternet, cfgServiceUrl, NULL, 0,
			INTERNET_FLAG_NO_COOKIES | INTERNET_FLAG_NO_CACHE_WRITE | INTERNET_FLAG_RELOAD, NULL))
		{
			DWORD responseLength = 0;
			CHAR response[16];
			if (InternetReadFile(hFile, response, 16, &responseLength))
			{
				if (responseLength >= 8)
				{
					response[responseLength] = '\0';
					BYTE temp;
					if (sscanf_s(response, "%hhu.%hhu.%hhu.%hhu", &temp, &temp, &temp, &temp) == 4)
						lstrcpy(buffer, response);
					else
						SetLastError(0x20000002);
				}
				else
				{
					SetLastError(0x20000001);
				}
			}
		}
	}

	DWORD error = GetLastError();
	if (hFile) InternetCloseHandle(hFile);
	if (hInternet) InternetCloseHandle(hInternet);
	if (error && cfgShowErrors)
	{
		CHAR msg[512];
		sprintf_s(msg, "Could not resolve your IP through the web service. %s", fk::getErrorMessage(error).c_str());
		MessageBox(NULL, msg, "fkNetcode", MB_ICONWARNING);
	}
	return !error;
}

bool resolveIPFallback(LPSTR buffer)
{
	if (!*cfgFallbackIP)
		return false;
	lstrcpy(buffer, cfgFallbackIP);
	return true;
}

bool resolveIPOriginal(LPSTR buffer)
{
	// Use the original logic to "resolve" the (NAT) IP.
	CHAR hostName[200];
	hostent* host;
	if (gethostname(hostName, 200) || !(host = gethostbyname(hostName)))
		return false;

	sprintf_s(hostName, "%hhu.%hhu.%hhu.%hhu",
		host->h_addr_list[0][0],
		host->h_addr_list[0][1],
		host->h_addr_list[0][2],
		host->h_addr_list[0][3]);
	lstrcpy(buffer, hostName);
	return true;
}

bool __stdcall patchResolveIP(LPSTR buffer, int bufferLength)
{
	// Return value not used by W2, but meant to be 0 if no error.
	if (resolveIPCached(buffer) || resolveIPExternal(buffer) || resolveIPFallback(buffer) || resolveIPOriginal(buffer))
	{
		lstrcpy(cachedIP, buffer);
		return false;
	}
	else
	{
		return true;
	}
}

// ---- Patch ----

void patch(PEInfo& pe, int gameVersion)
{
	fk::patchJump((PVOID)pe.Offset(0x00001799), 5, &patchResolveIP, fk::IJ_JUMP); // replace IP resolve with web service

	if (gameVersion == fk::GAME_VERSION_TRY)
	{
		fk::patchNops(pe.Offset(0x00053B96), 5); // prevent overriding IP with user name
		fk::patchNops(pe.Offset(0x00054693), 5); // prevent overriding IP with NAT IP
		fk::patchNops(pe.Offset(0x00054635), 11); // useless sleep when connecting to server
	}
	else
	{
		fk::patchNops(pe.Offset(0x00053E96), 5); // prevent overriding IP with user name
		fk::patchNops(pe.Offset(0x00054935), 11); // useless sleep when connecting to server
		fk::patchNops(pe.Offset(0x00054993), 5); // prevent overriding IP with NAT IP
	}
}

// ---- Main ----

BOOL WINAPI DllMain(HMODULE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	switch (fdwReason)
	{
		case DLL_PROCESS_ATTACH:
		{
			PEInfo pe;
			int version = fk::getGameVersion(pe.FH->TimeDateStamp);
			if (version == fk::GAME_VERSION_NONE)
			{
				MessageBox(NULL, "fkNetcode is incompatible with your game version. Please run the 1.05 patch or "
					"1.07 release of Worms 2. Otherwise, you can delete the module to remove this warning.",
					"fkNetcode", MB_ICONWARNING);
			}
			else
			{
				configure();
				patch(pe, version);
			}
		}
		break;

		case DLL_PROCESS_DETACH:
			break;
	}
	return TRUE;
}
