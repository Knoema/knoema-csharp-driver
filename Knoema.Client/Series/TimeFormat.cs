using System;
using System.Collections.Generic;
using System.Globalization;

namespace Knoema.Series
{
	public enum Frequency
	{
		Annual = 0,
		SemiAnnual = 1,
		Quarterly = 2,
		Monthly = 3,
		Weekly = 4,
		Daily = 5
	}

	public class TimeFormat
	{
		public class DatasetCultureInfo : CultureInfo
		{
			private readonly Calendar _calendar;
			private readonly int _calendarNumber;
			private readonly string _name;

			public DatasetCultureInfo(int cultureId, int calendarNumber)
				: base(cultureId)
			{
				_calendarNumber = calendarNumber;
				_calendar = OptionalCalendars[calendarNumber];
				DateTimeFormat.Calendar = _calendar;

				_name = (_calendarNumber > 0
						? _calendarNumber * 100000 + LCID
						: LCID)
						.ToString();
			}

			public override Calendar Calendar
			{
				get
				{
					return _calendar;
				}
			}

			public int CalendarNumber
			{
				get
				{
					return _calendarNumber;
				}
			}

			public override string Name
			{
				get
				{
					return _name;
				}
			}
		}

		public static readonly TimeFormat InvariantTimeFormat = new TimeFormat(CultureInfo.InvariantCulture);
		private static readonly Dictionary<int, TimeFormat> _timeFormats = new Dictionary<int, TimeFormat>();

		public static TimeFormat GetTimeFormat(int calendarCode)
		{
			TimeFormat result;
			if (calendarCode == 0)
				result = InvariantTimeFormat;
			else
			{
				if (!_timeFormats.TryGetValue(calendarCode, out result))
				{
					result = new TimeFormat(GetCulture(calendarCode));
					_timeFormats[calendarCode] = result;
				}
			}
			return result;
		}

		private static CultureInfo GetCulture(int calendarCode)
		{
			if (calendarCode == 127)
				calendarCode = 0;

			CultureInfo culture;
			if (calendarCode == 0)
				culture = new CultureInfo(127); // Writeable InvariantCulture
			else
			{
				var calendar = calendarCode / 100000;
				var localeId = calendarCode % 100000;
				culture = new DatasetCultureInfo(localeId, calendar);
			}
			return culture;
		}

		public readonly CultureInfo Culture;

		private TimeFormat(CultureInfo culture)
		{
			Culture = culture;
		}

		public Calendar Calendar
		{
			get
			{
				return Culture.Calendar;
			}
		}

		public IReadOnlyList<DateTime> ExpandRangeSelection(DateTime startDate, DateTime endDate, Frequency frequency)
		{
			var result = new List<DateTime>();
			var calendar = Calendar;

			var month = calendar.GetMonth(startDate);
			var year = calendar.GetYear(startDate);
			var day = calendar.GetDayOfMonth(startDate);

			var endYear = calendar.GetYear(endDate);
			var endMonth = calendar.GetMonth(endDate);

			var lastWeekDate = DateTime.MinValue;
			for (; ; )
			{
				if (frequency == Frequency.Annual)
				{
					if (month % 12 == 1 && day == 1)
						result.Add(new DateTime(year, 1, 1));
				}
				else if (frequency == Frequency.SemiAnnual)
				{
					if (month % 6 == 1 && day == 1)
						result.Add(new DateTime(year, month, 1));
				}
				else if (frequency == Frequency.Quarterly)
				{
					if (month % 3 == 1 && day == 1)
						result.Add(new DateTime(year, month, 1));
				}
				else if (frequency == Frequency.Monthly)
				{
					if (day == 1)
						result.Add(new DateTime(year, month, 1));
				}
				else if (frequency == Frequency.Weekly)
				{
					//Find num of weeks in that month
					var numberOfDays = year == endYear && month == endMonth ? calendar.GetDayOfMonth(endDate) : calendar.GetDaysInMonth(year, month);
					var firstDayOfWeek = DayOfWeek.Monday;// Culture.DateTimeFormat.FirstDayOfWeek;
					var curDay = day;
					while (curDay <= numberOfDays)
					{
						var weekDate = new DateTime(year, month, curDay, calendar);
						if (calendar.GetDayOfWeek(weekDate) == firstDayOfWeek)
							break;
						curDay++;
					}
					while (curDay <= numberOfDays)
					{
						var weekDate = new DateTime(year, month, curDay, calendar);
						if (lastWeekDate != weekDate)
						{
							lastWeekDate = weekDate;
							result.Add(weekDate);
						}
						curDay += 7;
					}
				}
				else if (frequency == Frequency.Daily)
				{
					var numberOfDays = year == endYear && month == endMonth ? calendar.GetDayOfMonth(endDate) : calendar.GetDaysInMonth(year, month);
					for (var i = day; i <= numberOfDays; i++)
						result.Add(new DateTime(year, month, i, calendar));
				}
				if (year > endYear || (year == endYear && month >= endMonth))
					break;
				day = 1;
				if (month < 12)
					month++;
				else
				{
					year++;
					month = 1;
				}
			}
			return result;
		}
	}
}
