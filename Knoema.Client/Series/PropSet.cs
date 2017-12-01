using System.Collections;
using System.Collections.Generic;

namespace Knoema.Series
{
	public class PropSet : IEnumerable<KeyValuePair<string, object>>
	{
		private readonly List<KeyValuePair<string, object>> _list;

		public PropSet()
		{
			_list = new List<KeyValuePair<string, object>>();
		}

		public void Add(string key, object value)
		{
			_list.Add(new KeyValuePair<string, object>(key, value));
		}

		private IEnumerator<KeyValuePair<string, object>> GetEnum()
		{
			return _list.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return GetEnum();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnum();
		}
	}
}
