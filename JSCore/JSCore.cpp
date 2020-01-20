// JSCore.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "JSCore.h"
#include "JSGC.h"

JSGC* gc=new JSGC();

EXPORT_API void _stdcall ScriptInit()
{
	gc->Init();
}

EXPORT_API void _stdcall ScriptUnInit()
{
	gc->Close();
	delete gc;
}
