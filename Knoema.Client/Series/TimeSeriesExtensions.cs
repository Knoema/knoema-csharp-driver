using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Series
{
	public static class TimeSeriesExtensions
	{
		public static Frequency ToFrequency(this string value)
		{
			return (Frequency)ToFrequencyInt(value[0]);
		}

		private static int ToFrequencyInt(char value)
		{
			switch (value)
			{
				case 'A':
					return (int)Frequency.Annual;
				case 'H':
				case 'S':
					return (int)Frequency.SemiAnnual;
				case 'Q':
					return (int)Frequency.Quarterly;
				case 'M':
					return (int)Frequency.Monthly;
				case 'W':
					return (int)Frequency.Weekly;
				case 'D':
					return (int)Frequency.Daily;
			}
			return -1;
		}

		public static char ToChar(this Frequency frequency)
		{
			switch (frequency)
			{
				case Frequency.Annual:
					return 'A';
				case Frequency.SemiAnnual:
					return 'H';
				case Frequency.Quarterly:
					return 'Q';
				case Frequency.Monthly:
					return 'M';
				case Frequency.Weekly:
					return 'W';
				case Frequency.Daily:
					return 'D';
			}
			return '\0';
		}

		public static IEnumerable<KeyValuePair<string, object>> ToPropertySet(this object obj)
		{
			if (obj == null)
				return Enumerable.Empty<KeyValuePair<string, object>>();

			if (obj is IEnumerable<KeyValuePair<string, object>>)
				return (obj as IEnumerable<KeyValuePair<string, object>>);

			if (obj is IEnumerable<Tuple<string, object>>)
				return (obj as IEnumerable<Tuple<string, object>>).Select(p => new KeyValuePair<string, object>(p.Item1, p.Item2));

			if (obj is string)
			{
				var mapping = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj as string);
				return mapping != null ? mapping : Enumerable.Empty<KeyValuePair<string, object>>();
			}

			return TypeDescriptor.GetProperties(obj.GetType())
				.Cast<PropertyDescriptor>()
				.Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(obj)));
		}
	}


}
