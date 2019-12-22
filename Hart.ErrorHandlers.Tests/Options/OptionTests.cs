using ErrorHandlersTests;
using ErrorHandlersTests.Helpers;
using Hart.ErrorHandlers.Options;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Hart.ErrorHandlers.Tests.Options
{
    public class OptionTests
    {

        [Fact]
        public void WrapAndRetrieveSome()
        {
            IOption<int> option = FakeService.GetOption(true);
           
            var some = (Some<int>)option;

            Assert.IsType<Some<int>>(option);
            Assert.Equal(10, some);
        } 

        [Fact]
        public void WrapAndRetrieveNone()
        {
            IOption<int> option = FakeService.GetOption(false);

            var some = (None<int>)option;

            Assert.IsType<None<int>>(option);
        }

        [Fact]
        public void WrapAndRetrieveSomeWithPatternMatching()
        {
            IOption<int> option = FakeService.GetOption(true);

            int res = 0;
            switch(option)
            {
                case Some<int> some:
                    res = some;
                    break;
                case None<int> _:
                    res = 0;
                    break;
            }

            Assert.IsType<Some<int>>(option);
            Assert.Equal(10, res);
        }

        [Fact]
        public void WrapAndRetrieveNoneWithPatternMatching()
        {
            IOption<int> option = FakeService.GetOption(false);

            int res = 0;
            switch (option)
            {
                case Some<int> some:
                    res = some;
                    break;
                case None<int> _:
                    // Set it to -1 for the assert to check something different to 0.
                    res = -1;
                    break;
            }

            Assert.IsType<None<int>>(option);
            Assert.Equal(-1, res);
        }

        [Fact]
        public void BuildSomeWithHelper()
        {
            var some = Option.Some(1);

            Assert.IsType<Some<int>>(some);
            Assert.Equal(1, some);
        } 

        [Fact]
        public void BuildNoneWithHelper()
        {
            var none = Option.None<int>();

            Assert.IsType<None<int>>(none);
        }

        [Fact]
        public void MapOptionIntToStringTest()
        {
            var someNum = Option.Some(8);
            var res = someNum.Map(x =>
            {
                switch (x)
                {
                    case Some<int> s: return ((int)s).ToString();
                    default: return string.Empty;
                }
            });

            Assert.Equal("8", res);
        }

        [Fact]
        public void MapOptionStringToIntTest()
        {
            var someNum = Option.Some("8");

            var optionStringToInt = new Func<IOption<string>, int>((x) =>
                x is Some<string> s
                ? int.Parse(s)
                : throw new InvalidCastException());

            int res = someNum.Map(optionStringToInt);

            Assert.Equal(8, res);
        }

        [Fact]
        public void MapOptionStringToUppercaseStringTest()
        {
            var someNum = Option.Some("hello world");

            var res = someNum.Map(x =>
                x is Some<string> s
                ? ((string)s).ToUpper()
                : string.Empty);

            Assert.Equal("HELLO WORLD", res);
        }
    }
}
