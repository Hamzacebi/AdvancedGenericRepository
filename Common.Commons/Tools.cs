using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Commons
{
    public static class Tools
    {
        public static void TryCatch(this Action action, Func<Exception, bool> catchAndDo = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                var rethrow = catchAndDo?.Invoke(e) ?? true;
                if (rethrow)
                {
                    throw;
                }
            }
        }

        public static T TryCatch<T>(this Func<T> function, Func<Exception, bool> catchAndDo = null)
        {
            try
            {
                if (function != null)
                {
                    return function();
                }
            }
            catch (Exception e)
            {
                var rethrow = catchAndDo?.Invoke(e) ?? true;
                if (rethrow)
                {
                    throw;
                }
            }
            return default(T);
        }
    }
}
