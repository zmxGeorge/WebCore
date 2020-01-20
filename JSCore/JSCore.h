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
JS���ɳ�ʼ��
*/
EXPORT_API void _stdcall ScriptInit();

/*
ע��ȫ��Ӧ�÷���
*/
EXPORT_API void _stdcall AddGobalFunction(unsigned char* methodName, gobalFunctionCallBack* methodCallBack);

/*
����ScriptContext
*/
EXPORT_API ScriptContext* _stdcall CreateScriptContext(wkeWebView* view);

/*
����Object����
*/
EXPORT_API unsigned long _stdcall CreateObject(ScriptContext* context,getPropertyCallBack getter,setPropertyCallBack setter);

/*
�ͷ�ScriptContext
*/
EXPORT_API void _stdcall DisposedScriptContext(ScriptContext* context);

/*
�ر�JS����
*/
EXPORT_API void _stdcall ScriptUnInit();

