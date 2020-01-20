#pragma once

#include "wke.h"

#ifdef WKEJAVASCRIPT_EXPORTS
#define EXPORT_API extern "C" __declspec(dllexport)//ע��declǰ���������»���
#else
#define EXPORT_API extern "C" __declspec(dllimport)
#endif

/*
����JS����
*/
EXPORT_API long _stdcall JsCreateFunction(wkeJSState* state, wkeJSCallAsFunctionCallback fun);

/*
����JS����
*/
EXPORT_API long _stdcall JsCreateObject(
	wkeJSState* state,
	wkeJSGetPropertyCallback getter,
	wkeJSSetPropertyCallback setter,
	wkeJSCallAsFunctionCallback callBack,
	wkeJSFinalizeCallback finalCallBack
	);

struct JSSTRINGINFO
{
	int strLen;
	const char* strPtr;
};

/*
��ȡJs�ַ���
*/
EXPORT_API JSSTRINGINFO _stdcall GetTempJavaScriptString(wkeJSState* state, __int64 value);
