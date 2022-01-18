using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace nanoFramework.SDCard.Helpers
{
    public class Strings
    { /// <summary>
      /// Finds and replaces occurances within a string
      /// </summary>
      /// <param name="Source"></param>
      /// <param name="ToFind"></param>
      /// <param name="ReplaceWith"></param>
      /// <returns></returns>
        public static string Replace(string Source, string ToFind, string ReplaceWith)
        {
            int i;
            int iStart = 0;

            if (Source == string.Empty || Source == null || ToFind == string.Empty || ToFind == null)
                return Source;

            while (true)
            {
                i = Source.IndexOf(ToFind, iStart);
                if (i < 0) break;

                if (i > 0)
                    Source = Source.Substring(0, i) + ReplaceWith + Source.Substring(i + ToFind.Length);
                else
                    Source = ReplaceWith + Source.Substring(i + ToFind.Length);

                iStart = i + ReplaceWith.Length;
            }
            return Source;
        }

        public static string ReplaceEmptyOrNull(string value, string replaceWith)
        {
            if (value == string.Empty || value == null)
                return replaceWith;
            return value;
        }

        public static string ReplaceEmptyOrNull(object value, string replaceWith)
        {
            if (value == null || value.ToString() == string.Empty)
                return replaceWith;
            return value.ToString();
        }
    }
}
