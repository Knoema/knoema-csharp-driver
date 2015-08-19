using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knoema
{
	public static class StringExtensions
	{
		public static string AddUrlParam(this string query, string name, string value)
		{
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(name))
				return query;

			if (string.IsNullOrEmpty(query))
				query = query + string.Format("{0}={1}", name, value);
			else
				query = query + string.Format("&{0}={1}", name, value);

			return query;
		}
	}
}
