//  Copyright (C) 2014 Jerome Bell (jeromebell0509@gmail.com)
//
//  This file is part of OmniOpen Test.
//  OmniOpen Test is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  OmniOpen Test is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with OmniOpen Test.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OmniOpen.Foundation
{
    /// <summary>
    ///     A library of extension methods for the most ubiquitous boiler plate code
    /// </summary>
    public static class Common
    {
        /// <summary>
        ///     Validates an argument throwing an <see cref="ArgumentException"/> with the proper name of <paramref name="argument"/>
        /// </summary>
        /// <typeparam name="TArgument">the argument type</typeparam>
        /// <param name="this">the invoking method</param>
        /// <param name="argument">an expression of the argument to be validated</param>
        /// <param name="validator">a validation function to perform on <paramref name="argument"/></param>
        /// <param name="validationFailureMessage">
        ///     The message to be included in the <see cref="ArgumentException"/> upon validation failure.  If null or whitespace it will 
        ///     effectively be ignored
        /// </param>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="argument"/> fails validation as defined by <paramref name="validator"/>
        /// </exception>
        /// <remarks>
        ///     If either <paramref name="argument"/> (the expression) or <paramref name="validator"/> are null then no validation will be performed.
        /// </remarks>
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

        /// <summary>
        ///     Validates that an argument is not null throwing an <see cref="ArgumentNullException"/> with the proper name of <paramref name="argument"/>
        /// </summary>
        /// <typeparam name="TArgument">the argument type</typeparam>
        /// <param name="this">the invoking method</param>
        /// <param name="argument">an expression of the argument to be validated</param>
        /// <param name="validationFailureMessage">
        ///     The message to be included in the <see cref="ArgumentNullException"/> upon validation failure.  If null or whitespace it will 
        ///     effectively be ignored
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="argument"/> is null
        /// </exception>
        /// <remarks>
        ///     If <paramref name="argument"/> (the expression) is null then no validation will be performed.
        /// </remarks>
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

        /// <summary>
        ///     Validates that a string argument is not null or empty throwing an <see cref="ArgumentException"/> with the proper name of <paramref name="argument"/>
        /// </summary>
        /// <param name="this">the invoking method</param>
        /// <param name="argument">an expression of the argument to be validated</param>
        /// <param name="validationFailureMessage">
        ///     The message to be included in the <see cref="ArgumentException"/> upon validation failure.  If null or whitespace it will 
        ///     effectively be ignored
        /// </param>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="argument"/> is null or empty (zero length)
        /// </exception>
        /// <remarks>
        ///     If <paramref name="argument"/> (the expression) is null then no validation will be performed.
        /// </remarks>
        public static void ValidateArgumentNotNullOrEmpty(this object @this, Expression<Func<string>> argument, string validationFailureMessage = null)
        {
            ValidateArgument(@this, argument, s => !string.IsNullOrEmpty(s), validationFailureMessage);
        }

        /// <summary>
        ///     Validates that a string argument is not null or whitespace throwing an <see cref="ArgumentException"/> with the proper name of <paramref name="argument"/>
        /// </summary>
        /// <param name="this">the invoking method</param>
        /// <param name="argument">an expression of the argument to be validated</param>
        /// <param name="validationFailureMessage">
        ///     The message to be included in the <see cref="ArgumentException"/> upon validation failure.  If null or whitespace it will 
        ///     effectively be ignored
        /// </param>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="argument"/> is null or whitespace
        /// </exception>
        /// <remarks>
        ///     If <paramref name="argument"/> (the expression) is null then no validation will be performed.
        /// </remarks>
        public static void ValidateArgumentNotNullOrWhitespace(this object @this, Expression<Func<string>> argument, string validationFailureMessage = null)
        {
            ValidateArgument(@this, argument, s => !string.IsNullOrWhiteSpace(s), validationFailureMessage);
        }
    }
}
