package kurumi;

import java.util.Calendar;
import java.util.TimeZone;

//{
	public class DateTimeProxy 
	{
		//see https://github.com/weimingtom/mochalua/blob/master/Mochalua/src/com/groundspeak/mochalua/LuaOSLib.java
		private Calendar _calendar;
		 
        public DateTimeProxy()
        {
        	this._calendar = Calendar.getInstance(TimeZone.getTimeZone("GMT"));
        }

        public DateTimeProxy(int year, int month, int day, int hour, int min, int sec)
        {
        	this._calendar = Calendar.getInstance(TimeZone.getTimeZone("GMT"));
        	this._calendar.set(year, month, day, hour, min, sec);
        }

        public void setUTCNow()
        {
        	_calendar = Calendar.getInstance(TimeZone.getTimeZone("UTC"));
        }

        public void setNow()
        {
        	_calendar = Calendar.getInstance(TimeZone.getTimeZone("GMT"));  
        }

        public int getSecond()
        {
        	return _calendar.get(Calendar.SECOND);
        }

        public int getMinute()
        {
        	return _calendar.get(Calendar.MINUTE);
        }

        public int getHour()
        {
        	return _calendar.get(Calendar.HOUR_OF_DAY);
        }

        public int getDay()
        {
        	return _calendar.get(Calendar.DATE);
        }

        public int getMonth()
        {
        	return _calendar.get(Calendar.MONTH) + 1;
        }

        public int getYear()
        {
			return _calendar.get(Calendar.YEAR);
        }

        public int getDayOfWeek()
        {
        	return _calendar.get(Calendar.DAY_OF_WEEK);
        }

        public int getDayOfYear()
        {
        	return _calendar.get(Calendar.DAY_OF_YEAR);
        }

        //http://www.cnblogs.com/zyw-205520/p/4632490.html
        //https://github.com/anonl/luajpp2/blob/master/core/src/main/java/nl/weeaboo/lua2/lib/OsLib.java
		public boolean IsDaylightSavingTime() 
        {
			return _calendar.get(Calendar.DST_OFFSET) != 0;
        }

		//https://github.com/weimingtom/mochalua/blob/master/Mochalua/src/com/groundspeak/mochalua/LuaOSLib.java
        public double getTicks()
        {
			return _calendar.getTime().getTime();
        }

        //https://github.com/anonl/luajpp2/blob/master/core/src/main/java/nl/weeaboo/lua2/lib/OsLib.java
        private static final long _t0 = System.currentTimeMillis();
        public static double getClock()
        {
        	return (System.currentTimeMillis() - _t0) / 1000.;
        }
    }
//}