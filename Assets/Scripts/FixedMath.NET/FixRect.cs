using System;

namespace FixMath.NET {
	[Serializable]
	public struct Rect {
		public Fix64 xMin;
		public Fix64 xMax;
		public Fix64 yMin;
		public Fix64 yMax;

		public Rect(float xMin, float xMax, float yMin, float yMax) {
			this.xMin = (Fix64) xMin;
			this.xMax = (Fix64) xMax;
			this.yMin = (Fix64) yMin;
			this.yMax = (Fix64) yMax;
		}
	}
}