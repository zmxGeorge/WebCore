#include "stdafx.h"
#include "JSGC.h"
#pragma  comment (lib,"wke.lib")


JSGC::JSGC()
{
}


JSGC::~JSGC()
{
}

void JSGC::Init()
{
	head = nullptr;
}

JSVALUEINFO* JSGC::AddObject(ScriptContext* context,
	wkeJSValue value, bool keepAlive)
{
	JSVALUEINFO* jsInfo = new JSVALUEINFO();
	jsInfo->id = len;
	jsInfo->conext = context;
	jsInfo->keepAlive = keepAlive;
	jsInfo->value = value;
	if (keepAlive&&head!=nullptr)
	{
		JSVALUEINFO* end = (JSVALUEINFO*)(head + sizeof(JSVALUEINFO)*(len-1));
		end->next = jsInfo;
		jsInfo->up = end;
		jsInfo->next = nullptr;
		len++;
	}
	else if(!keepAlive&&head != nullptr)
	{
		head->up = jsInfo;
		jsInfo->up = nullptr;
		jsInfo->next = head;
		head = jsInfo;
		len++;
	}
	else if (head == nullptr)
	{
		head = jsInfo;
		len++;
	}
	else
	{
		delete jsInfo;
	}
	return jsInfo;
}

void JSGC::RealseObject(JSVALUEINFO* info)
{
	info->keepAlive = false;
}

void JSGC::Collect()
{
	JSVALUEINFO* temp = head;
	while (temp)
	{
		if (temp->keepAlive)
		{
			continue;
		}
		temp->conext->GCValue(temp->value);
		JSVALUEINFO* nextNode = temp->next;
		if (temp->up != nullptr)
		{
			temp->up = nextNode;
		}
		JSVALUEINFO* c = temp;
		c->up = nullptr;
		c->next = nullptr;
		delete c;
		temp = nextNode;
	}
	wkeJSCollectGarbge();
}

void JSGC::Close()
{
	wkeJSCollectGarbge();
}
