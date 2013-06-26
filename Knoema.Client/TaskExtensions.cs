using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knoema
{
	public static class TaskExtensions
	{
		// Code originated from http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx

		public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, Task<T2>> next)
		{
			if (first == null) 
				throw new ArgumentNullException("first");

			if (next == null)
				throw new ArgumentNullException("next");

			var tcs = new TaskCompletionSource<T2>();

			first.ContinueWith(delegate
			{
				if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
				else if (first.IsCanceled) tcs.TrySetCanceled();
				else
				{
					try
					{
						var t = next(first.Result);
						if (t == null) tcs.TrySetCanceled();
						else t.ContinueWith(delegate
						{
							if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
							else if (t.IsCanceled) tcs.TrySetCanceled();
							else tcs.TrySetResult(t.Result);
						}, TaskContinuationOptions.ExecuteSynchronously);
					}
					catch (Exception exc) { tcs.TrySetException(exc); }
				}
			}, 
			TaskContinuationOptions.ExecuteSynchronously);

			return tcs.Task;
		}
	}
}
