using System;
using System.Globalization;
using System.Text;

namespace ExpressionCode
{
    public static class ExpressionExtensions
    {
        // Converts a character to lower-case for the default culture.
        public static char ToLower(this char c)
        {
            return ToLower(c, CultureInfo.CurrentUICulture);
        }
        // Converts a character to lower-case for the specified culture.
         // <;<;Not fully implemented>;>;
        public static char ToLower(char c, CultureInfo culture)
        {
            
            if (culture == null)
                throw new ArgumentNullException("culture");
            //Contract.EndContractBlock();
            //return culture.TextInfo.ToLower(c);
            return c.ToLower();
        }

    }
}
