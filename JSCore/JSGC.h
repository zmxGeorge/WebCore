#pragma once
#include "wke.h"
#include "ScriptContext.h"
struct JSVALUEINFO
{
	long id=0;
	bool keepAlive=false;
	wkeJSValue value=0;
	wkeJSType jsType= wkeJSType::JSTYPE_UNDEFINED;
	ScriptContext* conext;
	JSVALUEINFO* next=nullptr;
	JSVALUEINFO* up = nullptr;
};

class JSGC
{
private:
	JSVALUEINFO* head;
	long len = 0;
public:
	JSGC();
	~JSGC();
	void Init();
   JSVALUEINFO* AddObject(ScriptContext* context,
		wkeJSValue value,bool keepAlive=false);
	void RealseObject(JSVALUEINFO* info);
	void Collect();
	void Close();
};

