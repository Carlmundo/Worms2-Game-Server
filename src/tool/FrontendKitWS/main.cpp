#define WIN32_LEAN_AND_MEAN
#include "fk.h"
#include <windows.h>

struct wsock32_dll
{
	HMODULE dll;
	FARPROC AcceptEx;
	FARPROC Arecv;
	FARPROC Asend;
	FARPROC EnumProtocolsA;
	FARPROC EnumProtocolsW;
	FARPROC GetAcceptExSockaddrs;
	FARPROC GetAddressByNameA;
	FARPROC GetAddressByNameW;
	FARPROC GetNameByTypeA;
	FARPROC GetNameByTypeW;
	FARPROC GetServiceA;
	FARPROC GetServiceW;
	FARPROC GetTypeByNameA;
	FARPROC GetTypeByNameW;
	FARPROC MigrateWinsockConfiguration;
	FARPROC NPLoadNameSpaces;
	FARPROC NSPStartup;
	FARPROC SetServiceA;
	FARPROC SetServiceW;
	FARPROC TransmitFile;
	FARPROC WEP;
	FARPROC WSAAsyncGetHostByAddr;
	FARPROC WSAAsyncGetHostByName;
	FARPROC WSAAsyncGetProtoByName;
	FARPROC WSAAsyncGetProtoByNumber;
	FARPROC WSAAsyncGetServByName;
	FARPROC WSAAsyncGetServByPort;
	FARPROC WSAAsyncSelect;
	FARPROC WSACancelAsyncRequest;
	FARPROC WSACancelBlockingCall;
	FARPROC WSACleanup;
	FARPROC WSAGetLastError;
	FARPROC WSAIsBlocking;
	FARPROC WSARecvEx;
	FARPROC WSASetBlockingHook;
	FARPROC WSASetLastError;
	FARPROC WSAStartup;
	FARPROC WSAUnhookBlockingHook;
	FARPROC WSApSetPostRoutine;
	FARPROC WSHEnumProtocols;
	FARPROC WsControl;
	FARPROC __WSAFDIsSet;
	FARPROC accept;
	FARPROC bind;
	FARPROC closesocket;
	FARPROC closesockinfo;
	FARPROC connect;
	FARPROC dn_expand;
	FARPROC gethostbyaddr;
	FARPROC gethostbyname;
	FARPROC gethostname;
	FARPROC getnetbyname;
	FARPROC getpeername;
	FARPROC getprotobyname;
	FARPROC getprotobynumber;
	FARPROC getservbyname;
	FARPROC getservbyport;
	FARPROC getsockname;
	FARPROC getsockopt;
	FARPROC htonl;
	FARPROC htons;
	FARPROC inet_addr;
	FARPROC inet_network;
	FARPROC inet_ntoa;
	FARPROC ioctlsocket;
	FARPROC listen;
	FARPROC ntohl;
	FARPROC ntohs;
	FARPROC rcmd;
	FARPROC recv;
	FARPROC recvfrom;
	FARPROC rexec;
	FARPROC rresvport;
	FARPROC s_perror;
	FARPROC select;
	FARPROC send;
	FARPROC sendto;
	FARPROC sethostname;
	FARPROC setsockopt;
	FARPROC shutdown;
	FARPROC socket;
} wsock32;

__declspec(naked) void AcceptEx() { _asm { jmp[wsock32.AcceptEx] } }
__declspec(naked) void Arecv() { _asm { jmp[wsock32.Arecv] } }
__declspec(naked) void Asend() { _asm { jmp[wsock32.Asend] } }
__declspec(naked) void EnumProtocolsA() { _asm { jmp[wsock32.EnumProtocolsA] } }
__declspec(naked) void EnumProtocolsW() { _asm { jmp[wsock32.EnumProtocolsW] } }
__declspec(naked) void GetAcceptExSockaddrs() { _asm { jmp[wsock32.GetAcceptExSockaddrs] } }
__declspec(naked) void GetAddressByNameA() { _asm { jmp[wsock32.GetAddressByNameA] } }
__declspec(naked) void GetAddressByNameW() { _asm { jmp[wsock32.GetAddressByNameW] } }
__declspec(naked) void GetNameByTypeA() { _asm { jmp[wsock32.GetNameByTypeA] } }
__declspec(naked) void GetNameByTypeW() { _asm { jmp[wsock32.GetNameByTypeW] } }
__declspec(naked) void GetServiceA() { _asm { jmp[wsock32.GetServiceA] } }
__declspec(naked) void GetServiceW() { _asm { jmp[wsock32.GetServiceW] } }
__declspec(naked) void GetTypeByNameA() { _asm { jmp[wsock32.GetTypeByNameA] } }
__declspec(naked) void GetTypeByNameW() { _asm { jmp[wsock32.GetTypeByNameW] } }
__declspec(naked) void MigrateWinsockConfiguration() { _asm { jmp[wsock32.MigrateWinsockConfiguration] } }
__declspec(naked) void NPLoadNameSpaces() { _asm { jmp[wsock32.NPLoadNameSpaces] } }
__declspec(naked) void NSPStartup() { _asm { jmp[wsock32.NSPStartup] } }
__declspec(naked) void SetServiceA() { _asm { jmp[wsock32.SetServiceA] } }
__declspec(naked) void SetServiceW() { _asm { jmp[wsock32.SetServiceW] } }
__declspec(naked) void TransmitFile() { _asm { jmp[wsock32.TransmitFile] } }
__declspec(naked) void WEP() { _asm { jmp[wsock32.WEP] } }
__declspec(naked) void WSAAsyncGetHostByAddr() { _asm { jmp[wsock32.WSAAsyncGetHostByAddr] } }
__declspec(naked) void WSAAsyncGetHostByName() { _asm { jmp[wsock32.WSAAsyncGetHostByName] } }
__declspec(naked) void WSAAsyncGetProtoByName() { _asm { jmp[wsock32.WSAAsyncGetProtoByName] } }
__declspec(naked) void WSAAsyncGetProtoByNumber() { _asm { jmp[wsock32.WSAAsyncGetProtoByNumber] } }
__declspec(naked) void WSAAsyncGetServByName() { _asm { jmp[wsock32.WSAAsyncGetServByName] } }
__declspec(naked) void WSAAsyncGetServByPort() { _asm { jmp[wsock32.WSAAsyncGetServByPort] } }
__declspec(naked) void WSAAsyncSelect() { _asm { jmp[wsock32.WSAAsyncSelect] } }
__declspec(naked) void WSACancelAsyncRequest() { _asm { jmp[wsock32.WSACancelAsyncRequest] } }
__declspec(naked) void WSACancelBlockingCall() { _asm { jmp[wsock32.WSACancelBlockingCall] } }
__declspec(naked) void WSACleanup() { _asm { jmp[wsock32.WSACleanup] } }
__declspec(naked) void WSAGetLastError() { _asm { jmp[wsock32.WSAGetLastError] } }
__declspec(naked) void WSAIsBlocking() { _asm { jmp[wsock32.WSAIsBlocking] } }
__declspec(naked) void WSARecvEx() { _asm { jmp[wsock32.WSARecvEx] } }
__declspec(naked) void WSASetBlockingHook() { _asm { jmp[wsock32.WSASetBlockingHook] } }
__declspec(naked) void WSASetLastError() { _asm { jmp[wsock32.WSASetLastError] } }
__declspec(naked) void WSAStartup() { _asm { jmp[wsock32.WSAStartup] } }
__declspec(naked) void WSAUnhookBlockingHook() { _asm { jmp[wsock32.WSAUnhookBlockingHook] } }
__declspec(naked) void WSApSetPostRoutine() { _asm { jmp[wsock32.WSApSetPostRoutine] } }
__declspec(naked) void WSHEnumProtocols() { _asm { jmp[wsock32.WSHEnumProtocols] } }
__declspec(naked) void WsControl() { _asm { jmp[wsock32.WsControl] } }
__declspec(naked) void __WSAFDIsSet() { _asm { jmp[wsock32.__WSAFDIsSet] } }
__declspec(naked) void accept() { _asm { jmp[wsock32.accept] } }
__declspec(naked) void bind() { _asm { jmp[wsock32.bind] } }
__declspec(naked) void closesocket() { _asm { jmp[wsock32.closesocket] } }
__declspec(naked) void closesockinfo() { _asm { jmp[wsock32.closesockinfo] } }
__declspec(naked) void connect() { _asm { jmp[wsock32.connect] } }
__declspec(naked) void dn_expand() { _asm { jmp[wsock32.dn_expand] } }
__declspec(naked) void gethostbyaddr() { _asm { jmp[wsock32.gethostbyaddr] } }
__declspec(naked) void gethostbyname() { _asm { jmp[wsock32.gethostbyname] } }
__declspec(naked) void gethostname() { _asm { jmp[wsock32.gethostname] } }
__declspec(naked) void getnetbyname() { _asm { jmp[wsock32.getnetbyname] } }
__declspec(naked) void getpeername() { _asm { jmp[wsock32.getpeername] } }
__declspec(naked) void getprotobyname() { _asm { jmp[wsock32.getprotobyname] } }
__declspec(naked) void getprotobynumber() { _asm { jmp[wsock32.getprotobynumber] } }
__declspec(naked) void getservbyname() { _asm { jmp[wsock32.getservbyname] } }
__declspec(naked) void getservbyport() { _asm { jmp[wsock32.getservbyport] } }
__declspec(naked) void getsockname() { _asm { jmp[wsock32.getsockname] } }
__declspec(naked) void getsockopt() { _asm { jmp[wsock32.getsockopt] } }
__declspec(naked) void htonl() { _asm { jmp[wsock32.htonl] } }
__declspec(naked) void htons() { _asm { jmp[wsock32.htons] } }
__declspec(naked) void inet_addr() { _asm { jmp[wsock32.inet_addr] } }
__declspec(naked) void inet_network() { _asm { jmp[wsock32.inet_network] } }
__declspec(naked) void inet_ntoa() { _asm { jmp[wsock32.inet_ntoa] } }
__declspec(naked) void ioctlsocket() { _asm { jmp[wsock32.ioctlsocket] } }
__declspec(naked) void listen() { _asm { jmp[wsock32.listen] } }
__declspec(naked) void ntohl() { _asm { jmp[wsock32.ntohl] } }
__declspec(naked) void ntohs() { _asm { jmp[wsock32.ntohs] } }
__declspec(naked) void rcmd() { _asm { jmp[wsock32.rcmd] } }
__declspec(naked) void recv() { _asm { jmp[wsock32.recv] } }
__declspec(naked) void recvfrom() { _asm { jmp[wsock32.recvfrom] } }
__declspec(naked) void rexec() { _asm { jmp[wsock32.rexec] } }
__declspec(naked) void rresvport() { _asm { jmp[wsock32.rresvport] } }
__declspec(naked) void s_perror() { _asm { jmp[wsock32.s_perror] } }
__declspec(naked) void select() { _asm { jmp[wsock32.select] } }
__declspec(naked) void send() { _asm { jmp[wsock32.send] } }
__declspec(naked) void sendto() { _asm { jmp[wsock32.sendto] } }
__declspec(naked) void sethostname() { _asm { jmp[wsock32.sethostname] } }
__declspec(naked) void setsockopt() { _asm { jmp[wsock32.setsockopt] } }
__declspec(naked) void shutdown() { _asm { jmp[wsock32.shutdown] } }
__declspec(naked) void socket() { _asm { jmp[wsock32.socket] } }

char path[MAX_PATH + 16];
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			GetSystemDirectoryA(path, MAX_PATH);
			lstrcatA(path, "\\WSOCK32.dll");
			wsock32.dll = LoadLibraryA(path);
			wsock32.AcceptEx = GetProcAddress(wsock32.dll, "AcceptEx");
			wsock32.Arecv = GetProcAddress(wsock32.dll, "Arecv");
			wsock32.Asend = GetProcAddress(wsock32.dll, "Asend");
			wsock32.EnumProtocolsA = GetProcAddress(wsock32.dll, "EnumProtocolsA");
			wsock32.EnumProtocolsW = GetProcAddress(wsock32.dll, "EnumProtocolsW");
			wsock32.GetAcceptExSockaddrs = GetProcAddress(wsock32.dll, "GetAcceptExSockaddrs");
			wsock32.GetAddressByNameA = GetProcAddress(wsock32.dll, "GetAddressByNameA");
			wsock32.GetAddressByNameW = GetProcAddress(wsock32.dll, "GetAddressByNameW");
			wsock32.GetNameByTypeA = GetProcAddress(wsock32.dll, "GetNameByTypeA");
			wsock32.GetNameByTypeW = GetProcAddress(wsock32.dll, "GetNameByTypeW");
			wsock32.GetServiceA = GetProcAddress(wsock32.dll, "GetServiceA");
			wsock32.GetServiceW = GetProcAddress(wsock32.dll, "GetServiceW");
			wsock32.GetTypeByNameA = GetProcAddress(wsock32.dll, "GetTypeByNameA");
			wsock32.GetTypeByNameW = GetProcAddress(wsock32.dll, "GetTypeByNameW");
			wsock32.MigrateWinsockConfiguration = GetProcAddress(wsock32.dll, "MigrateWinsockConfiguration");
			wsock32.NPLoadNameSpaces = GetProcAddress(wsock32.dll, "NPLoadNameSpaces");
			wsock32.NSPStartup = GetProcAddress(wsock32.dll, "NSPStartup");
			wsock32.SetServiceA = GetProcAddress(wsock32.dll, "SetServiceA");
			wsock32.SetServiceW = GetProcAddress(wsock32.dll, "SetServiceW");
			wsock32.TransmitFile = GetProcAddress(wsock32.dll, "TransmitFile");
			wsock32.WEP = GetProcAddress(wsock32.dll, "WEP");
			wsock32.WSAAsyncGetHostByAddr = GetProcAddress(wsock32.dll, "WSAAsyncGetHostByAddr");
			wsock32.WSAAsyncGetHostByName = GetProcAddress(wsock32.dll, "WSAAsyncGetHostByName");
			wsock32.WSAAsyncGetProtoByName = GetProcAddress(wsock32.dll, "WSAAsyncGetProtoByName");
			wsock32.WSAAsyncGetProtoByNumber = GetProcAddress(wsock32.dll, "WSAAsyncGetProtoByNumber");
			wsock32.WSAAsyncGetServByName = GetProcAddress(wsock32.dll, "WSAAsyncGetServByName");
			wsock32.WSAAsyncGetServByPort = GetProcAddress(wsock32.dll, "WSAAsyncGetServByPort");
			wsock32.WSAAsyncSelect = GetProcAddress(wsock32.dll, "WSAAsyncSelect");
			wsock32.WSACancelAsyncRequest = GetProcAddress(wsock32.dll, "WSACancelAsyncRequest");
			wsock32.WSACancelBlockingCall = GetProcAddress(wsock32.dll, "WSACancelBlockingCall");
			wsock32.WSACleanup = GetProcAddress(wsock32.dll, "WSACleanup");
			wsock32.WSAGetLastError = GetProcAddress(wsock32.dll, "WSAGetLastError");
			wsock32.WSAIsBlocking = GetProcAddress(wsock32.dll, "WSAIsBlocking");
			wsock32.WSARecvEx = GetProcAddress(wsock32.dll, "WSARecvEx");
			wsock32.WSASetBlockingHook = GetProcAddress(wsock32.dll, "WSASetBlockingHook");
			wsock32.WSASetLastError = GetProcAddress(wsock32.dll, "WSASetLastError");
			wsock32.WSAStartup = GetProcAddress(wsock32.dll, "WSAStartup");
			wsock32.WSAUnhookBlockingHook = GetProcAddress(wsock32.dll, "WSAUnhookBlockingHook");
			wsock32.WSApSetPostRoutine = GetProcAddress(wsock32.dll, "WSApSetPostRoutine");
			wsock32.WSHEnumProtocols = GetProcAddress(wsock32.dll, "WSHEnumProtocols");
			wsock32.WsControl = GetProcAddress(wsock32.dll, "WsControl");
			wsock32.__WSAFDIsSet = GetProcAddress(wsock32.dll, "__WSAFDIsSet");
			wsock32.accept = GetProcAddress(wsock32.dll, "accept");
			wsock32.bind = GetProcAddress(wsock32.dll, "bind");
			wsock32.closesocket = GetProcAddress(wsock32.dll, "closesocket");
			wsock32.closesockinfo = GetProcAddress(wsock32.dll, "closesockinfo");
			wsock32.connect = GetProcAddress(wsock32.dll, "connect");
			wsock32.dn_expand = GetProcAddress(wsock32.dll, "dn_expand");
			wsock32.gethostbyaddr = GetProcAddress(wsock32.dll, "gethostbyaddr");
			wsock32.gethostbyname = GetProcAddress(wsock32.dll, "gethostbyname");
			wsock32.gethostname = GetProcAddress(wsock32.dll, "gethostname");
			wsock32.getnetbyname = GetProcAddress(wsock32.dll, "getnetbyname");
			wsock32.getpeername = GetProcAddress(wsock32.dll, "getpeername");
			wsock32.getprotobyname = GetProcAddress(wsock32.dll, "getprotobyname");
			wsock32.getprotobynumber = GetProcAddress(wsock32.dll, "getprotobynumber");
			wsock32.getservbyname = GetProcAddress(wsock32.dll, "getservbyname");
			wsock32.getservbyport = GetProcAddress(wsock32.dll, "getservbyport");
			wsock32.getsockname = GetProcAddress(wsock32.dll, "getsockname");
			wsock32.getsockopt = GetProcAddress(wsock32.dll, "getsockopt");
			wsock32.htonl = GetProcAddress(wsock32.dll, "htonl");
			wsock32.htons = GetProcAddress(wsock32.dll, "htons");
			wsock32.inet_addr = GetProcAddress(wsock32.dll, "inet_addr");
			wsock32.inet_network = GetProcAddress(wsock32.dll, "inet_network");
			wsock32.inet_ntoa = GetProcAddress(wsock32.dll, "inet_ntoa");
			wsock32.ioctlsocket = GetProcAddress(wsock32.dll, "ioctlsocket");
			wsock32.listen = GetProcAddress(wsock32.dll, "listen");
			wsock32.ntohl = GetProcAddress(wsock32.dll, "ntohl");
			wsock32.ntohs = GetProcAddress(wsock32.dll, "ntohs");
			wsock32.rcmd = GetProcAddress(wsock32.dll, "rcmd");
			wsock32.recv = GetProcAddress(wsock32.dll, "recv");
			wsock32.recvfrom = GetProcAddress(wsock32.dll, "recvfrom");
			wsock32.rexec = GetProcAddress(wsock32.dll, "rexec");
			wsock32.rresvport = GetProcAddress(wsock32.dll, "rresvport");
			wsock32.s_perror = GetProcAddress(wsock32.dll, "s_perror");
			wsock32.select = GetProcAddress(wsock32.dll, "select");
			wsock32.send = GetProcAddress(wsock32.dll, "send");
			wsock32.sendto = GetProcAddress(wsock32.dll, "sendto");
			wsock32.sethostname = GetProcAddress(wsock32.dll, "sethostname");
			wsock32.setsockopt = GetProcAddress(wsock32.dll, "setsockopt");
			wsock32.shutdown = GetProcAddress(wsock32.dll, "shutdown");
			wsock32.socket = GetProcAddress(wsock32.dll, "socket");
			fkAttach();
			break;

		case DLL_PROCESS_DETACH:
			FreeLibrary(wsock32.dll);
			fkDetach();
			break;
	}
	return TRUE;
}
