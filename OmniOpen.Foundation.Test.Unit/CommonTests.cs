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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using FluentAssertions;
using OmniOpen.Foundation;
using OmniOpen.Test;
using System.Linq;

namespace OmniOpen.Foundation.Test.Unit
{
    [TestClass]
    public class CommonTests
    {
        private const string GenericArgumentName = "dummyArgument";

        public TestContext TestContext { get; set; }

        private void ValidateArgumentWithGenericTypedArgument<T>(T dummyArgument)
        {
            this.ValidateArgument(() => dummyArgument, x => x != null);
        }

        private void ValidateArgumentNotNullWithGenericTypedArgument<T>(T dummyArgument)
        {
            this.ValidateArgumentNotNull(() => dummyArgument);
        }

        [TestMethod]
        public void ValidateArgument_NullInvokingObject_DataIsValidated()
        {
            object nullInvokingObject = null;
            object invalidValue = null;
            Action invocation;

            //arrange

            invocation = () => nullInvokingObject.ValidateArgument(() => invalidValue, x => x != null);

            //act & assert

            invocation.ShouldThrow<ArgumentException>("because the value being validated is invalid");
        }

        [TestMethod]
        public void ValidateArgument_NullArgumentExpression_NoValidationPerformed()
        {
            Expression<Func<string>> argumentExpression = null;
            Action invocation;

            //arrange 

            invocation = () => this.ValidateArgument(argumentExpression, (s) => s != null);

            //act & assert

            invocation.ShouldNotThrow("because no argument was supplied");
        }

        [TestMethod]
        public void ValidateArgument_NullValidator_NoValidationPerformed()
        {
            string argument = null;
            Action invocation;

            //arrange 

            invocation = () => this.ValidateArgument(() => argument, null);

            //act & assert

            invocation.ShouldNotThrow("because no argument validator was supplied");
        }

        [TestMethod]
        public void ValidateArgument_ArgumentIsInvalid_ArgumentExceptionThrown()
        {
            string invalidArgument = null;
            Action invocation;
            ArgumentException actualException;

            //arrange 

            invocation = () => this.ValidateArgument(() => invalidArgument, argument => argument != null);

            //act

            actualException = invocation.ShouldThrow<ArgumentException>("because the argument is null and we are validating that it is not null").Subject.First();

            //assert

            actualException.Message
                .Should().NotStartWith(Environment.NewLine, "because error messages are prefixed with the custom error message followed by a new line");

            actualException.ParamName
            .Should().Be("invalidArgument");
        }

        [TestMethod]
        public void ValidateArgument_ArgumentIsValid_NoExceptionsAreThrown()
        {
            string validArgument = string.Empty;
            Action invocation;

            //arrange 

            invocation = () => this.ValidateArgument(() => validArgument, argument => argument != null);

            //act & assert

            invocation.ShouldNotThrow("because the argument is not null and we are validating that the argument is not null");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgument_InvalidValidationFailureMessages", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgument_InvalidValidationFailureMessages()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string validationFailureMessage = this.TestData<string>("ValidationFailureMessage");
            string invalidArgument = null;
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgument(() => invalidArgument, x => x != null, validationFailureMessage);

            //act & assert

            invocation.ShouldThrow<ArgumentException>(testDescription)
                .And.Message.Should().NotStartWith(Environment.NewLine, "{0} > {1}", testDescription, "because error messages are prefixed with the custom error message followed by a new line");
        }

        [TestMethod]
        public void ValidateArgument_ArgumentIsInvalidAndValidationFailureMessageIsValid_ValidationFailureMessageIsIncludedInTheExceptionMessage()
        {
            const string ValidationFailureMessage = "OmniOpen";

            Action invocation;
            object invalidArgument = null;

            //arrange

            invocation = () => this.ValidateArgument(() => invalidArgument, x => x != null, ValidationFailureMessage);

            //act & assert

            invocation.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain(ValidationFailureMessage);
        }

        [TestMethod]
        public void ValidateArgument_ArgumentIsGenericTypeAndNull_ThrowsArgumentException()
        {
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentWithGenericTypedArgument<string>(null);

            //act & assert

            invocation.ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be(GenericArgumentName);
        }

        [TestMethod]
        public void ValidateArgumentNotNull_NullInvokingObject_ValidationPerformed()
        {
            object nullInvokingObject = null;
            object nullArgument = null;
            Action invocation;

            //arrange

            invocation = () => nullInvokingObject.ValidateArgumentNotNull(() => nullArgument);

            //act & assert

            invocation.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("nullArgument");
        }

        [TestMethod]
        public void ValidateArgumentNotNull_NullArgumentExpression_NoValidationPerformed()
        {
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNull(null);

            //act & assert

            invocation.ShouldNotThrow("because validation cannot fail since validation is undefined");
        }

        [TestMethod]
        public void ValidateArgumentNotNull_ArgumentIsNotNull_NoExceptionThrown()
        {
            object nonNullArgument = string.Empty;
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNull(() => nonNullArgument);

            //act & assert

            invocation.ShouldNotThrow("because the value is not null");
        }

        [TestMethod]
        public void ValidateArgumentNotNull_ArgumentIsNull_ArgumentNullExceptionThrown()
        {
            object nullArgument = null;
            Action invocation;
            ArgumentNullException actualArgument;

            //arrange

            invocation = () => this.ValidateArgumentNotNull(() => nullArgument);

            //act

            actualArgument = invocation.ShouldThrow<ArgumentNullException>("because the value is null").Subject.First();

            //assert

            actualArgument.Message
                .Should().NotStartWith(Environment.NewLine, "because error messages are prefixed with the custom error message followed by a new line");

            actualArgument.ParamName
                .Should().Be("nullArgument");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgumentNotNull_InvalidValidationFailureMessages", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgumentNotNull_InvalidValidationFailureMessages()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string validationFailureMessage = this.TestData<string>("ValidationFailureMessage");
            string invalidArgument = null;
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNull(() => invalidArgument, validationFailureMessage);

            //act & assert

            invocation.ShouldThrow<ArgumentException>(testDescription)
                .And.Message.Should().NotStartWith(Environment.NewLine, "{0} > {1}", testDescription, "because error messages are prefixed with the custom error message followed by a new line");
        }

        [TestMethod]
        public void ValidateArgumentNotNull_ArgumentIsNullAndValidationFailureMessageIsValid_ValidationFailureMessageIsInExceptionMessage()
        {
            const string ValidationFailureMessage = "OmniOpen";

            object nullArgument = null;
            Action invocation;
            ArgumentNullException actualArgument;

            //arrange

            invocation = () => this.ValidateArgumentNotNull(() => nullArgument, ValidationFailureMessage);

            //act

            actualArgument = invocation.ShouldThrow<ArgumentNullException>("because the value is null").Subject.First();

            //assert

            actualArgument.Message
                .Should().Contain(ValidationFailureMessage);

            actualArgument.ParamName
                .Should().Be("nullArgument");
        }

        [TestMethod]
        public void ValidateArgumentNotNull_ArgumentIsGenericTypeAndNull_ThrowsArgumentNullException()
        {
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullWithGenericTypedArgument<string>(null);

            //act & assert

            invocation.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be(GenericArgumentName);
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrEmpty_NullInvokingObject_ValidationPerformed()
        {
            object nullInvokingObject = null;
            string nullArgument = null;
            Action invocation;

            //arrange

            invocation = () => nullInvokingObject.ValidateArgumentNotNullOrEmpty(() => nullArgument);

            //act & assert

            invocation.ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("nullArgument");
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrEmpty_NullArgumentExpression_NoValidationPerformed()
        {
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrEmpty(null);

            //act & assert

            invocation.ShouldNotThrow("because validation cannot fail since validation is undefined");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgumentNotNullOrEmpty_InvalidArguments", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgumentNotNullOrEmpty_InvalidArguments()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string argument = this.TestData<string>("Argument");
            Action invocation;
            ArgumentException actualException;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrEmpty(() => argument);

            //act

            actualException = invocation.ShouldThrow<ArgumentException>(testDescription).Subject.First();

            //assert

            actualException.Message
                .Should().NotStartWith(Environment.NewLine, "{0} > {1}", testDescription, "because error messages are prefixed with the custom error message followed by a new line");

            actualException.ParamName
                .Should().Be("argument", testDescription);
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrEmpty_ArgumentIsNotNullOrEmpty_NoExceptionsThrown()
        {
            string validArgument = "OmniOpen";
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrEmpty(() => validArgument);

            //act & assert

            invocation.ShouldNotThrow("because the argument is not null or empty");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgumentNotNullOrEmpty_InvalidValidationFailureMessages", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgumentNotNullOrEmpty_InvalidValidationFailureMessages()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string invalidArgument = null;
            string validationFailureMessage = this.TestData<string>("ValidationFailureMessage");
            Action invocation;
            ArgumentException actualException;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrEmpty(() => invalidArgument, validationFailureMessage);

            //act

            actualException = invocation.ShouldThrow<ArgumentException>(testDescription).Subject.First();

            //assert

            actualException.Message
                .Should().NotStartWith(Environment.NewLine, "{0} and {1}", testDescription, "error messages are prefixed with the custom error message followed by a new line");
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrEmpty_ArgumentIsInvalidAndValidationFailureMessageIsValidArgumentIsNotNullOrEmpty_ValidationFailureMessageIsInExceptionMessage()
        {
            const string ValidationFailureMessage = "OmniOpen";

            string invalidArgument = null;
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrEmpty(() => invalidArgument, ValidationFailureMessage);

            //act & assert

            invocation.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain(ValidationFailureMessage);
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrWhitespace_NullInvokingObject_ValidationPerformed()
        {
            object nullInvokingObject = null;
            string nullArgument = null;
            Action invocation;

            //arrange

            invocation = () => nullInvokingObject.ValidateArgumentNotNullOrWhitespace(() => nullArgument);

            //act & assert

            invocation.ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("nullArgument");
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrWhitespace_NullArgumentExpression_NoValidationPerformed()
        {
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrWhitespace(null);

            //act & assert

            invocation.ShouldNotThrow("because validation cannot fail since validation is undefined");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgumentNotNullOrWhitespace_InvalidArguments", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgumentNotNullOrWhitespace_InvalidArguments()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string argument = this.TestData<string>("Argument");
            Action invocation;
            ArgumentException actualException;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrWhitespace(() => argument);

            //act & assert

            actualException = invocation.ShouldThrow<ArgumentException>(testDescription).Subject.First();

            //assert

            actualException.Message
                .Should().NotStartWith(Environment.NewLine, "{0} and {1}", testDescription, "error messages are prefixed with the custom error message followed by a new line");

            actualException.ParamName
                .Should().Be("argument");
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\OmniOpen.Foundation.Test.Unit.CommonTests.xml", "ValidateArgumentNotNullOrWhitespace_InvalidValidationFailureMessages", DataAccessMethod.Sequential)]
        [DeploymentItem(@"Data\OmniOpen.Foundation.Test.Unit.CommonTests.xml")]
        public void ValidateArgumentNotNullOrWhitespace_InvalidValidationFailureMessages()
        {
            string testDescription = this.TestData<string>("TestDescription");
            string invalidArgument = null;
            string validationFailureMessage = this.TestData<string>("ValidationFailureMessage");
            Action invocation;
            ArgumentException actualException;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrWhitespace(() => invalidArgument, validationFailureMessage);

            //act

            actualException = invocation.ShouldThrow<ArgumentException>(testDescription).Subject.First();

            //assert

            actualException.Message
                .Should().NotStartWith(Environment.NewLine, "{0} and {1}", testDescription, "error messages are prefixed with the custom error message followed by a new line");
        }

        [TestMethod]
        public void ValidateArgumentNotNullOrWhitespace_ArgumentIsNotNullOrEmpty_NoExceptionsThrown()
        {
            string validArgument = "OmniOpen";
            Action invocation;

            //arrange

            invocation = () => this.ValidateArgumentNotNullOrWhitespace(() => validArgument);

            //act & assert

            invocation.ShouldNotThrow("because the argument is not null or empty");
        }
    }
}
