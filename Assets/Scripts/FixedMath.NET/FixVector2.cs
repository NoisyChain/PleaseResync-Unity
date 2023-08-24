using System;
using UnityEngine;

namespace FixMath.NET {
	[Serializable]
	public struct FixVector2 {
		public Fix64 x;
		public Fix64 y;

		public FixVector2(Fix64 x, Fix64 y)
		{
			this.x = x;
			this.y = y;
		}

		public static readonly FixVector2 Zero = new FixVector2();

		public static Fix64 Distance(FixVector2 vec0, FixVector2 vec1) {
			Fix64 dx = vec0.x - vec1.x;
			Fix64 dy = vec0.y - vec1.y;
			return Fix64.Sqrt(dx * dx + dy * dy);
		}

		public static explicit operator Vector2(FixVector2 value)
        {
			return new Vector2((float)value.x, (float)value.y);
		}

		public override string ToString()
		{
			return $"X: {x}\nY: {y}";
		}
	}
}