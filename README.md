Foundation
==========

A library for the most ubiquitous boiler plate code

Download the [NuGet Package](https://www.nuget.org/packages/OmniOpen.Foundation/) by searching for "OmniOpen Foundation" in Visual Studio's Nuget Package Manager.


##Usage
The library has extension methods for conveniently and concisely validating method parameters.  For example

```CSharp
using OmniOpen.Foundation;

public void AddUser(string userName, int age, IAddress address)
{
  //If 'age' is less than 18 then the line below will throw an ArgumentException with the parameter
  //name correctly identified as "age" (no hard coded, error prone, and brittle string literals!) and
  //will contain "User must be 18 years or older" in the error message.
  //
  //It is equivalent to writing
  //if (age >=10) throw new ArgumentException("User must be 18 years or older", "age");
  this.ValidateArgument(() => age, x => x >= 18, "User must be 18 years or older");
  
  //This will throw an ArgumentNullException with the parameter name correctly identified as "address"
  //(no hard coded, error prone, and brittle string literals!) and will contain "User address cannot be null"
  //in the error message.
  //
  //It is equivalent to writing
  //if (address == null) throw new ArgumentNullException("address", "User address cannot be null")
  this.ValidateArgumentNotNull(() => address, "User address cannot be null");
  
  //if the user name is null or empty (zero length string) it will throw an ArgumentExcetion with the
  //parameter name correctly identified as "userName" (no hard coded, error prone, and brittle
  //string literals!) and will contain "User name cannot be blank!" text in the error message.
  //
  //It is equivalent to writing
  //if (string.IsNullOrEmpty(userName)) throw new ArgumentException("User name cannot be blank!", "userName")
  this.ValidateArgumentNotNullOrEmpty(() => userName, "User name cannot be blank!");
  
  //if the user name is null or white space it will throw an ArgumentExcetion with the
  //parameter name correctly identified as "userName" (no hard coded, error prone, and brittle
  //string literals!) and will contain "User name cannot be blank!" text in the error message.
  //
  //It is equivalent to writing
  //if (string.IsNullOrWhitespace(userName)) throw new ArgumentException("User name cannot be blank!", "userName")
  this.ValidateArgumentNotNullOrWhitespace(() => userName, "User name cannot be blank!");
}

```


##Release Notes
1.0.1 (2014-01-30)
* Fixed bug where null reference exception is thrown while validating null generic typed arguments

1.0.0 (2014-01-30)
* Initial release including helper methods for basic argument validation
