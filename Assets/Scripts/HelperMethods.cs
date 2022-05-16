using System;

public static class HelperMethods
{
	public static bool IsClassOrSubclassOf(this Type type1, Type type2)
	{
		return type1 == type2 || type1.IsSubclassOf(type2);
	}
}