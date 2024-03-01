#define _SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING

#include <codecvt>
#include <string>
#include <Windows.h>

#pragma comment(linker, "\"/manifestdependency:type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

using namespace std;

int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
	SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
	const auto hFile = CreateFileW(L"//./pipe/Qwilight", GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE) {
		MessageBoxW(NULL, L"Cannot find Qwilight", L"Qwilight", MB_ICONWARNING);
	}
	else {
		const auto lpBuffer = wstring_convert<std::codecvt_utf8<wchar_t>>().to_bytes(wstring(lpCmdLine).c_str());
		WriteFile(hFile, lpBuffer.c_str(), (DWORD)lpBuffer.length(), NULL, NULL);
		CloseHandle(hFile);
	}
}