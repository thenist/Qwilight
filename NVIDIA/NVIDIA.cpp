#include "pch.h"

using namespace sl;

extern "C"
{
	__declspec(dllexport) void InitNVLL(void*);
	__declspec(dllexport) void SetNVLLConfigure(ReflexMode, uint32_t);
	__declspec(dllexport) void GetNVLLFrame();
	__declspec(dllexport) void WaitNVLL();
	__declspec(dllexport) void SetNVLLFlag(ReflexMarker);
	__declspec(dllexport) void NotifyNVLL(uint32_t);
	__declspec(dllexport) bool IsNVLLAvailable();
}

static BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	return TRUE;
}

FrameToken* frame = {};
ReflexState state = {};

void InitNVLL(void* d3dDevice)
{
	Preferences pref = {};
	pref.featuresToLoad = new Feature[1]{ kFeatureReflex };
	pref.numFeaturesToLoad = sizeof(*pref.featuresToLoad) / sizeof(Feature);
	pref.engineVersion = "1.1.0";
	pref.renderAPI = RenderAPI::eD3D11;
	if (SL_FAILED(r, slInit(pref)))
	{
		return;
	}
	if (SL_FAILED(r, slSetD3DDevice(d3dDevice)))
	{
		return;
	}
	if (SL_FAILED(r, slReflexGetState(state)))
	{
		return;
	}
	slGetNewFrameToken(frame);
}

void SetNVLLConfigure(ReflexMode mode, uint32_t frameLimitUs)
{
	ReflexOptions options = {};
	options.mode = mode;
	options.frameLimitUs = frameLimitUs;
	if (SL_FAILED(r, slReflexSetOptions(options)))
	{
		return;
	}
	slReflexGetState(state);
}

void GetNVLLFrame()
{
	auto frameIndex = *frame + 1;
	slGetNewFrameToken(frame, &frameIndex);
}

void SetNVLLFlag(ReflexMarker marker)
{
	slReflexSetMarker(marker, *frame);
}

void NotifyNVLL(uint32_t statsWindowMessage)
{
	if (state.statsWindowMessage == statsWindowMessage)
	{
		slReflexSetMarker(ReflexMarker::ePCLatencyPing, *frame);
	}
}

void WaitNVLL()
{
	slReflexSleep(*frame);
}

bool IsNVLLAvailable()
{
	return state.lowLatencyAvailable;
}