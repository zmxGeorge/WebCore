// wkeJavaScript.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "wkeJavaScript.h"
#pragma  comment (lib,"wke.lib")

static void js_releaseFunction(wkeJSData* data)
{
	delete data;
}

EXPORT_API long _stdcall JsCreateFunction(wkeJSState * state, wkeJSCallAsFunctionCallback fun)
{
	wkeJSData* data = new wkeJSData();
	memset(data, 0, sizeof(wkeJSData));
	strcpy(data->typeName, "Function");
	data->callAsFunction = fun;
	data->finalize = js_releaseFunction;
	return wkeJSFunction(state, data);
}

EXPORT_API long _stdcall JsCreateObject(wkeJSState * state,
	wkeJSGetPropertyCallback getter, 
	wkeJSSetPropertyCallback setter,
	wkeJSCallAsFunctionCallback callBack,
	wkeJSFinalizeCallback finalCallBack)
{
	wkeJSData* data = new wkeJSData();
	memset(data, 0, sizeof(wkeJSData));
	strcpy(data->typeName, "Object");
	data->propertyGet = getter;
	data->propertySet = setter;
	data->callAsFunction = callBack;
	data->finalize = finalCallBack;
	return wkeJSObject(state,data);
}



EXPORT_API JSSTRINGINFO _stdcall GetTempJavaScriptString(wkeJSState* state, __int64 value)
{
	const char *expr = wkeJSToTempString(state, value);
	JSSTRINGINFO info;
	info.strLen = strlen(expr);
	info.strPtr = expr;
	return info;
}



