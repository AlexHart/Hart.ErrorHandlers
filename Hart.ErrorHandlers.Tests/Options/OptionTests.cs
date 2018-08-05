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
            IOption option = FakeService.GetOption(true);
           
            var some = option as Some<int>;

            Assert.IsType<Some<int>>(option);
            Assert.Equal(10, some);
        } 

        [Fact]
        public void WrapAndRetrieveNone()
        {
            IOption option = FakeService.GetOption(false);

            var some = option as None;

            Assert.IsType<None>(option);
        }

        [Fact]
        public void WrapAndRetrieveSomeWithPatternMatching()
        {
            var option = FakeService.GetOption(true);

            int res = 0;
            switch(option)
            {
                case Some<int> some:
                    res = some;
                    break;
                case None _:
                    res = 0;
                    break;
            }

            Assert.IsType<Some<int>>(option);
            Assert.Equal(10, res);
        }

        [Fact]
        public void WrapAndRetrieveNoneWithPatternMatching()
        {
            var option = FakeService.GetOption(false);

            int res = 0;
            switch (option)
            {
                case Some<int> some:
                    res = some;
                    break;
                case None _:
                    // Set it to -1 for the assert to check something different to 0.
                    res = -1;
                    break;
            }

            Assert.IsType<None>(option);
            Assert.Equal(-1, res);
        }

    }
}
