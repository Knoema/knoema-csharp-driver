using System.Linq;

namespace Knoema.Series
{
	public class TimeSeriesId
	{
		public readonly Frequency Freq;
		public readonly AttributesMap<int> Keys;
		public readonly AttributesMap<object> Attributes;

		public TimeSeriesId(Frequency frequency, AttributesMap<int> keys, AttributesMap<object> attributes)
		{
			Freq = frequency;
			Keys = keys;
			Attributes = attributes;
		}

		public override string ToString()
		{
			return Freq.ToChar().ToString() + ": " + string.Join(", ", Attributes.Select(a => string.Format("{0}={1}", a.Key, a.Value)));
		}

		public override int GetHashCode()
		{
			var result = (int)Freq;
			var d = Keys.Values;
			for (var i = 0; i < d.Length; i++)
				result = result * 17 + d[i];
			return result;
		}

		public override bool Equals(object obj)
		{
			var tsId = obj as TimeSeriesId;
			if (tsId == null)
				return false;

			if (Freq != tsId.Freq)
				return false;

			var d1 = tsId.Keys.Values;
			var d2 = Keys.Values;
			if (d1.Length != d2.Length)
				return false;

			for (var i = 0; i < d1.Length; i++)
			{
				if (d1[i] != d2[i])
					return false;
			}

			return true;
		}
	}
}
