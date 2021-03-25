using System;
using System.Collections.Generic;
using System.Text;

namespace YuanShen.Core.Data
{
    public static class TimeHelper
    {
		private static readonly DateTime UtcOffset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
		public static long GetUnixTime(this DateTime dateTime)
		{
			return (long)(dateTime - TimeHelper.UtcOffset).TotalSeconds;
		}
	}
}
