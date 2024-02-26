// xsGetGoal xsSetGoal xsGetStrategicNumber xsSetStrategicNumber

const int SN_ARG0 = 450;

int GetArgument(int index = -1)
{
	int arg = xsGetStrategicNumber(SN_ARG0 + index);
	
	return (arg);
}

void SetArgument(int index = -1, int value = -1)
{
	xsSetStrategicNumber(SN_ARG0 + index, value);
}

include "..\ai\Deimos\Memory.xs";
include "..\ai\Deimos\Group.xs";