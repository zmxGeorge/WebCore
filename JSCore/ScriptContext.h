#pragma once
#include "wke.h"
class ScriptContext
{
private:
	wkeWebView* _view;
public:
	ScriptContext(wkeWebView* view);
	~ScriptContext();
	void GCValue(wkeJSValue value);
};

