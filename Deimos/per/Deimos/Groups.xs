const int MAX_GROUPS = 100;
const int GROUP_DATA_LENGTH = 10;

int _Groups_GroupById = -1;
int _Groups_GroupsArray = -1;
int _Groups_GroupCounts = -1;

void Groups_Initialize()
{
	_Groups_GroupById = xsArrayCreateInt(100000, -1, "_Groups_GroupById");
	_Groups_GroupsArray = xsArrayCreateInt(MAX_GROUPS, -1, "_Groups_GroupsArray");
	_Groups_GroupCounts = xsArrayCreateInt(MAX_GROUPS, 0, "_Groups_GroupCounts");

	for (i = 0; < MAX_GROUPS)
	{
		int g = xsArrayCreateInt(GROUP_DATA_LENGTH, -1, "_Groups_GroupsArray g" + i);
		xsArraySetInt(_Groups_GroupsArray, i, g);
	}
}

int Groups_GetCount(int index = -1)
{
	int count = xsArrayGetInt(_Groups_GroupCounts, index);
	
	return (count);
}

int Groups_GetDataArray(int index = -1)
{
	int arr = xsArrayGetInt(_Groups_GroupsArray, index);
	
	return (arr);
}

int Groups_GetGroupIndex(int id = -1)
{
	int index = xsArrayGetInt(_Groups_GroupById, id);

	return (index);
}

void Groups_SetGroup(int id = -1, int index = -1)
{
	int current = Groups_GetGroupIndex(id);
	int count = -1;
	
	if (current >= 0)
	{
		count = xsArrayGetInt(_Groups_GroupCounts, current);
		count--;
		xsArraySetInt(_Groups_GroupCounts, current, count);
	}
	
	xsArraySetInt(_Groups_GroupById, id, index);
	
	if (index >= 0)
	{
		count = xsArrayGetInt(_Groups_GroupCounts, index);
		count++;
		xsArraySetInt(_Groups_GroupCounts, index, count);
	}
}