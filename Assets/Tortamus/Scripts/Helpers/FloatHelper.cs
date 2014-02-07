using System;
using UnityEngine;

namespace Helpers
{
    public static class FloatHelper
    {
        public static bool IsSameSign(this float f1, float f2)
        {
            return (f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0);
        }      

		public static bool IsEqual(this float f1, float f2, float f3 = 0.0001f)
		{
			return Mathf.Abs(f1 - f2) < f3; 
		}

		public static bool IsAbsLess(this float f1, float f2)
		{
			return false;
			//return (Mathf.Abs(f1) < 
		}
    }
}
