using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OmniOpen.Foundation
{
    public static class Common
    {
        public static void ValidateArgument<TArgument>(this object @this, Expression<Func<TArgument>> argument, Expression<Func<TArgument, bool>> validator, string validationFailureMessage = null)
        {
            //validate the argument only if the argument and its validator are defined
            if (argument != null && validator != null)
            {
                //throw an ArgumentException if validation fails
                if (!validator.Compile()(argument.Compile()()))
                {
                    string argumentName = (argument.Body as MemberExpression).Member.Name;

                    throw new ArgumentException(GetNullIfWhitespace(validationFailureMessage), argumentName);
                }
            }
        }

        private static string GetNullIfWhitespace(string theString)
        {
            return string.IsNullOrWhiteSpace(theString) ? null : theString;
        }

        public static void ValidateArgumentNotNull(this object @this, Expression<Func<object>> argument, string validationFailureMessage = null)
        {
            try
            {
                @this.ValidateArgument(argument, x => x != null, validationFailureMessage);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentNullException(e.ParamName, GetNullIfWhitespace(validationFailureMessage));
            }
        }

        public static void ValidateArgumentNotNullOrEmpty(this object @this, Expression<Func<string>> argument, string validationFailureMessage = null)
        {
            ValidateArgument(@this, argument, s => !string.IsNullOrEmpty(s), validationFailureMessage);
        }

        public static void ValidateArgumentNotNullOrWhitespace(this object @this, Expression<Func<string>> argument, string validationFailureMessage = null)
        {
            ValidateArgument(@this, argument, s => !string.IsNullOrWhiteSpace(s), validationFailureMessage);
        }
    }
}
