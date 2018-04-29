# Hart.ErrorHandlers

## Purpose

This project purpose is to provide helpers for error handling in .NET Standard. Basically anything involving wrapping exceptions, method calls, retrying, etc.

## Language
This is a C# .Net Standard 2.0 library.

## Safe Result returns
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

> There are examples of how to use the library on the Hart.ErrorHandlersTests project.

---

## Retrying

In the namespace Hart.ErrorHandlers.Retry you will find and static class `Retry`
used to configure and retry functions or actions in a safe way. 
There are options to configure the number of retries, the ms wait between retries 
or even if the retrier will keep retrying until successful.

In it's simplest way it looks like this:  
``` CSharp
var res = Retrier.Init() // Create the default retry config
                 .Invoke(() => 2 + 2); // Pass the function to execute

int four = res.Result;

```

More Examples:
``` CSharp

/// Call a function that will fail 5 times and return a default value.
var res = Retrier.Init()
    .WithMsWaitOf(100)
    .WithNumberOfRetries(5)
    .Invoke(() => 2 / zero)
    .WithFallBackValue(33);

// Calling and action.
var res = Retrier.Init()
    .Invoke(() => Trace.WriteLine("action"))
    .WithFallBack(() => Trace.WriteLine("fallback"));;

```

The default configuration is **3 retries with 0 ms wait** between retries.

Of course **it also supports async methods**. But it's important to remember that you
should use the .InvokeAsync method instead of the regular Invoke. It works for
Funcs and Actions.

``` CSharp

// You can use the .WaitForValue to keep chaining methods.
// (this way the method that contains this Retry doesn't to be marked async)
public int GetValue() {
    var result = Retrier.Init()
        .WithNumberOfRetries(1)
        .InvokeAsync(async () => await yourAsyncFunction())
        .WaitForValue();
}

// Or if it's the last call, just use the regular await syntax.
public async int GetValueAsync() {
    // In this case needs the async and await keywords.
    var result = await Retrier.Init()
        .WithNumberOfRetries(1)
        .InvokeAsync(async () => await yourAsyncFunction());
}

// You also can use fallback async calls.
var result = Retrier.Init()
    .WithNumberOfRetries(5)
    .InvokeAsync(async () => await yourAsyncFunction())
    .WaitForValue()
    .WithFallbackAsync(async () => await yourAsyncFallbackFunction())
    .WaitForValue();
```

This class will try to handle any kind of errors that happen inside and return
the result if successful, a collection with the possible exceptions thrown and 
also information about the fallback execution. Also contains the number of executions.

You can find more about what it returns on the class `RetryResult<T>` or `RetryResult`.

> IMPORTANT: Since it doesn't throw exceptions **you should check that the operation was successful 
before using the returned value**. For example, a function that returns `int` that fails will return 0 as a Result, but also a list of exceptions.

**Example:**
``` CSharp
var res = Retrier.Init()
                 .Invoke(() => 2 + 2);

if (res.Successful)
{
    Console.WriteLine(res.Result); // 4  
} else {
    Console.WriteLine(res.RetryInfo
                        .Exceptions
                        .FirstOrDefault()
                        .Message);
}

// Number of executions of the function. (1)
Console.WriteLine(res.RetryInfo.Executions);

``` 

> If too many failed retries are done the Exceptions collection could grow so much as to throw an OutOfMemoryException. Please, use it with caution. In future versions it will be possible to opt-out from doing exception logging and only store the last one or none.