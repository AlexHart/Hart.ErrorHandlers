# Hart.ErrorHandlers

## Purpose

This project purpose is to provide helpers for error handling. Wrapping exceptions, method calls, retrying, etc.

## Language
This is a C# .Net Standard 2.0 library.

## IResult.cs
IResult is the interface that any type built to wrap function call results should implement. It's main purpose is to allow the needed covariance to return multiple types from the same function but "*packed*" under the same umbrella. IT also provides some methods to check if the call is Ok or if it returned any value.

**Usage example:**
``` CSharp
var calculator = new Calculator();
// Divide by zero exception.
IResult result = calculator.DoDivision(10, 0);

// The user has to check the type before accessing to the value.
if (result is Error) {
    var exception = (result as Error).Exception;
    Console.WriteLine(exception);
} 

// Or...
if (result is Success && result is Success<double>) {
    double divResult = (result as Success<double>).Value;
    Console.WriteLine(divResult);
}
```

**Implementation in method:**
``` CSharp
// Doing division could throw a divide by zero exception.
public IResult DoDivision(int x, int y)
{
	IResult result;
	try
	{
		result = new Success<double>(x / y);
	}
	catch(DivideByZeroException ex)
	{
		result = new Error<DivideByZeroException>(ex);
	}
	catch (Exception ex)
	{
		result = new Error(ex);
	}
	return result;
}
```

> There are examples of how to use it on the Hart.ErrorHandlersTests project.