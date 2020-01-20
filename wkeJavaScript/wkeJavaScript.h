#pragma once

#include "wke.h"

#ifdef WKEJAVASCRIPT_EXPORTS
#define EXPORT_API extern "C" __declspec(dllexport)//注意decl前面是两个下划线
#else
#define EXPORT_API extern "C" __declspec(dllimport)
#endif

/*
创建JS函数
*/
EXPORT_API long _stdcall JsCreateFunction(wkeJSState* state, wkeJSCallAsFunctionCallback fun);

/*
创建JS对象
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
获取Js字符串
*/
EXPORT_API JSSTRINGINFO _stdcall GetTempJavaScriptString(wkeJSState* state, __int64 value);
