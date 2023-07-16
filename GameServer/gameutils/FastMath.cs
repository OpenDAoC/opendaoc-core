using System;

namespace DOL.GS.Utils
{
	public static class FastMath
	{
		public static int Abs(this int x)
		{
			return (x >= 0) ? x : -x;
		}

		public static float Abs(this float x)
		{
			return (x >= 0) ? x : -x;
		}

		public static long Abs(this long x)
		{
			return (x >= 0) ? x : -x;
		}

		public static int Clamp(this int input, int min, int max)
		{
			int val = input;

			if (input < min)
			{
				val = min;
			}

			if (input > max)
			{
				val = max;
			}

			return val;
		}
	}
}

