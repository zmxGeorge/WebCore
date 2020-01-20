#include "stdafx.h"
#include "ScriptContext.h"
#pragma  comment (lib,"wke.lib")


ScriptContext::ScriptContext(wkeWebView* view)
{
	_view = view;
}


ScriptContext::~ScriptContext()
{
}

void ScriptContext::GCValue(wkeJSValue value)
{
}
