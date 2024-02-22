const int _GROUP_ARG0 = 450;
const int _GROUP_DATA_SIZE = 1;
const int _GROUP_IDS_LENGTH = 100000;

int _Group_Ids = -1;
int _Group_Array = -1;

void Group_Initialize()
{
	_Group_Ids = xsArrayCreateInt(_GROUP_IDS_LENGTH, -1, "_Group_Ids");
	_Group_Array = xsArrayCreateInt(0, -1, "_Group_Array");
}

void Group_GetGroupCount()
{
	int count = -1;
	count = xsArrayGetSize(_Group_Array);
	xsSetStrategicNumber(_GROUP_ARG0, count);
}

void Group_CreateGroup()
{
	int count = -1;
	count = xsArrayGetSize(_Group_Array);
	xsArrayResizeInt(_Group_Array, count + 1);
	
	int g = -1;
	g = xsArrayCreateInt(_GROUP_DATA_SIZE, -1, "_Group_Data" + count);
	xsArraySetInt(_Group_Array, count, g);
	xsSetStrategicNumber(_GROUP_ARG0, count);
}

void Group_GetGroup()
{
	int id = -1;
	id = xsGetStrategicNumber(_GROUP_ARG0);
	int g = -1;
	g = xsArrayGetInt(_Group_Array, id);
	int data = -1;
	
	for (i = 0; < _GROUP_DATA_SIZE)
	{
		data = xsArrayGetInt(g, i);
		xsSetStrategicNumber(_GROUP_ARG0 + i, data);
	}
}

void Group_SetGroup()
{
	int id = -1;
	id = xsGetStrategicNumber(_GROUP_ARG0);
	int g = -1;
	g = xsArrayGetInt(_Group_Array, id);
	int data = -1;
	
	for (i = 0; < _GROUP_DATA_SIZE)
	{
		data = xsGetStrategicNumber(_GROUP_ARG0 + 1 + i);
		xsArraySetInt(g, i, data);
	}
}

void Group_GetGroupId()
{
	int object_id = -1;
	object_id = xsGetStrategicNumber(_GROUP_ARG0);
	int id = -1;
	id = xsArrayGetInt(_Group_Ids, object_id);
	xsSetStrategicNumber(_GROUP_ARG0, id);
}

void Group_SetGroupId()
{
	int object_id = -1;
	object_id = xsGetStrategicNumber(_GROUP_ARG0);
	int id = -1;
	id = xsGetStrategicNumber(_GROUP_ARG0 + 1);
	xsArraySetInt(_Group_Ids, object_id, id);
}