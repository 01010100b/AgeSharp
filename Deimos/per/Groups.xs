const int MAX_GROUPS = 100;

int _Groups_GroupById = -1;
int _Groups_GroupsArray = -1:

void Groups_Initialize()
{
	_Groups_GroupById = xsArrayCreateInt(100000, -1, "_Groups_GroupById");
	_Groups_GroupsArray = xsArrayCreateInt(MAX_GROUPS, -1, "_Groups_GroupsArray");

	for (int i = 0; < MAX_GROUPS)
	{
		int g = xsArrayCreateInt(20, -1, "_Groups_GroupsArray g" + i);
		xsArraySetInt(_Groups_GroupsArray, i, g);
	}
}

void Groups_SetGroup(int id, int group)
{
	
}