#pragma once
#ifdef JSCORE_EXPORTS
#define EXPORT_API extern "C" __declspec(dllexport)
#else
#define EXPORT_API extern "C" __declspec(dllimport)
#endif

#include "ScriptContext.h"
#include "wke.h"
#include "JSGC.h"

typedef JSVALUEINFO*(__stdcall * gobalFunctionCallBack)(char* methodName, JSVALUEINFO* args, int argCount);

typedef JSVALUEINFO*(_stdcall * getPropertyCallBack)(ScriptContext* context, char* propertyName);

typedef void(_stdcall * setPropertyCallBack)(ScriptContext* context, char* propertyName, long value);

typedef JSVALUEINFO*(__stdcall * functionCallBack)(ScriptContext* context, unsigned long* args, int argCount);


/*
JS集成初始化
*/
EXPORT_API void _stdcall ScriptInit();

/*
注册全局应用方法
*/
EXPORT_API void _stdcall AddGobalFunction(unsigned char* methodName, gobalFunctionCallBack* methodCallBack);

/*
创建ScriptContext
*/
EXPORT_API ScriptContext* _stdcall CreateScriptContext(wkeWebView* view);

/*
创建Object对象
*/
EXPORT_API unsigned long _stdcall CreateObject(ScriptContext* context,getPropertyCallBack getter,setPropertyCallBack setter);

/*
释放ScriptContext
*/
EXPORT_API void _stdcall DisposedScriptContext(ScriptContext* context);

/*
关闭JS集成
*/
EXPORT_API void _stdcall ScriptUnInit();

