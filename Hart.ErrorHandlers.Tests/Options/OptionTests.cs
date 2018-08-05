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

    }
}
