#include <shlwapi.h>

#pragma comment(lib, "shlwapi")

#pragma comment(linker, "\"/manifestdependency:type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")

using namespace std;

static LRESULT lpfn(int code, WPARAM wParam, LPARAM lParam) {
	auto vkCode = ((KBDLLHOOKSTRUCT*)lParam)->vkCode;
	return vkCode == VK_LWIN || vkCode == VK_RWIN ? TRUE : FALSE;
}

static DWORD lpStartAddress(LPVOID lpThreadParameter) {
	auto idThread = (DWORD*)lpThreadParameter;
	while (true)
	{
		if (!PathFileExists(__wargv[1]) || DeleteFileW(__wargv[1])) {
			PostThreadMessage(*idThread, WM_QUIT, NULL, NULL);
		}
		Sleep(1000);
	}

	return EXIT_SUCCESS;
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow) {
	SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

	if (__argc < 2 || !PathFileExists(__wargv[1]) || DeleteFileW(__wargv[1])) {
		MessageBoxW(NULL, L"Cannot run Xwindow alone", L"Qwilight", MB_ICONWARNING);
		return EXIT_FAILURE;
	}

	auto hhk = SetWindowsHookExW(WH_KEYBOARD_LL, lpfn, NULL, NULL);

	auto idThread = GetCurrentThreadId();
	CreateThread(NULL, 0, lpStartAddress, &idThread, 0, NULL);

	MSG msg;
	while (GetMessage(&msg, NULL, NULL, NULL)) {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	UnhookWindowsHookEx(hhk);

	return EXIT_SUCCESS;
}