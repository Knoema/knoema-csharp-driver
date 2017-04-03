using System;

namespace Knoema
{
	public abstract class ClientBase
	{
		protected readonly DataAccessor _accessor;

		protected ClientBase(string host)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			_accessor = new DataAccessor(host);
		}

		protected ClientBase(string host, string token)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException("token");

			_accessor = new DataAccessor(host, token);
		}

		protected ClientBase(string host, string appId, string appSecret)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(appId))
				throw new ArgumentNullException("appId");

			if (string.IsNullOrEmpty(appSecret))
				throw new ArgumentNullException("appSecret");

			_accessor = new DataAccessor(host, appId, appSecret);
		}
	}
}
