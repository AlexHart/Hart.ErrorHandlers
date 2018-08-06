using ErrorHandlersTests;
using ErrorHandlersTests.Helpers;
using Hart.ErrorHandlers.Options;
using System;
using System.Collections.Generic;
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
           
            var some = option as Some<int>;

            Assert.IsType<Some<int>>(option);
            Assert.Equal(10, some);
        } 

        [Fact]
        public void WrapAndRetrieveNone()
        {
            IOption<int> option = FakeService.GetOption(false);

            var some = option as None<int>;

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
    }
}
