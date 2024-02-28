const int _GROUP_DATA_SIZE = 8;
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
	int count = xsArrayGetSize(_Group_Array);
	SetArgument(0, count);
}

void Group_CreateGroup()
{
	int count = xsArrayGetSize(_Group_Array);
	xsArrayResizeInt(_Group_Array, count + 1);
	
	int g = xsArrayCreateInt(_GROUP_DATA_SIZE, 0, "_Group_Data" + count);
	xsArraySetInt(_Group_Array, count, g);
	SetArgument(0, count);
}

void Group_GetGroup()
{
	int id = GetArgument(0);
	int g = xsArrayGetInt(_Group_Array, id);
	int data = -1;
	
	for (i = 0; < _GROUP_DATA_SIZE)
	{
		data = xsArrayGetInt(g, i);
		SetArgument(i, data);
	}
}

void Group_SetGroup()
{
	int id = GetArgument(0);
	int g = xsArrayGetInt(_Group_Array, id);
	int data = -1;
	
	for (i = 0; < _GROUP_DATA_SIZE)
	{
		data = GetArgument(1 + i);
		xsArraySetInt(g, i, data);
	}
}

void Group_GetGroupId()
{
	int object_id = GetArgument(0);
	int id = xsArrayGetInt(_Group_Ids, object_id);
	SetArgument(0, id);
}

void Group_SetGroupId()
{
	int object_id = GetArgument(0);
	int id = GetArgument(1);
	xsArraySetInt(_Group_Ids, object_id, id);
}