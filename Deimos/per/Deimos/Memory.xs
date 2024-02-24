int _Memory_Heap = -1;
int _Memory_Free = -1;

void Memory_Initialize()
{
	_Memory_Heap = xsArrayCreateInt(0, -1, "_Memory_Heap");
	_Memory_Free = xsArrayCreateInt(0, 0, "_Memory_Free");
}

void Memory_Allocate()
{
	int size = -1;
	size = GetArgument(0);
	int allocations = -1;
	allocations = xsArrayGetSize(_Memory_Free);
	int free_index = -1;
	
	for (i = 0; < allocations)
	{
		int free = -1;
		free = xsArrayGetInt(_Memory_Free, i);
		
		if (free == 1)
		{
			free_index = i;
			
			break;
		}
	}
	
	int array = -1;
	
	if (free_index == -1)
	{
		free_index = allocations;
		xsArrayResizeInt(_Memory_Heap, allocations + 1);
		xsArrayResizeInt(_Memory_Free, allocations + 1);
		array = xsArrayCreateInt(size, 0, "_Memory_Heap" + free_index);
		xsArraySetInt(_Memory_Heap, free_index, array);
	}
	else
	{
		array = xsArrayGetInt(_Memory_Heap, free_index);
		allocations = xsArrayGetSize(array);
		
		if (allocations < size)
		{
			xsArrayResizeInt(array, size);
			allocations = size;
		}
		
		for (i = 0; < allocations)
		{
			xsArraySetInt(array, i, 0);
		}
	}
	
	xsArraySetInt(_Memory_Free, free_index, 0);
	SetArgument(0, free_index);
}

void Memory_Free()
{
	int ptr = -1;
	ptr = GetArgument(0);
	xsArraySetInt(_Memory_Free, ptr, 1);
}

void Memory_GetValue()
{
	int ptr = -1;
	int index = -1;
	ptr = GetArgument(0);
	ptr = xsArrayGetInt(_Memory_Heap, ptr);
	index = GetArgument(1);
	int value = -1;
	value = xsArrayGetInt(ptr, index);
	SetArgument(0, value);
}

void Memory_SetValue()
{
	int ptr = -1;
	int index = -1;
	int value = -1;
	ptr = GetArgument(0);
	ptr = xsArrayGetInt(_Memory_Heap, ptr);
	index = GetArgument(1);
	value = GetArgument(2);
	xsArraySetInt(ptr, index, value);
}