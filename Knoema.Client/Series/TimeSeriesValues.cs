using System;
using Deedle;
using Knoema.Data;

namespace Knoema.Series
{
	public class TimeSeriesValues : TimeSeriesId
	{
		public readonly DateTime StartDate;
		public readonly DateTime EndDate;
		public readonly string Unit;
		public readonly double Scale;
		public readonly string Mnemonics;
		public readonly Series<DateTime, double> Values;
		public string Frequency
		{
			get
			{
				return Freq.ToChar().ToString();
			}
		}

		public TimeSeriesValues(RegularTimeSeriesRawData d, Frequency frequency, AttributesMap<string> keys, AttributesMap<object> attributes, Series<DateTime, double> values)
			: base(frequency, keys, attributes)
		{
			StartDate = d.StartDate;
			EndDate = d.EndDate;
			Unit = d.Unit;
			Scale = d.Scale;
			Mnemonics = d.Mnemonics;
			Values = values;
		}
	}
}
