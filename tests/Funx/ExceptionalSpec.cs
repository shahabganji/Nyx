using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using static Funx.Helpers;

namespace Funx.Tests
{
    public class ExceptionalSpec
    {
        [Fact]
        public void IsSuccess_Should_return_true_when_not_an_Exception()
        {
            Exceptional<string> exceptional = Success("success");

            Assert.True(exceptional.IsSuccess);
            Assert.False(exceptional.IsException);
        }

        [Fact]
        public void IsException_Should_return_true_when_not_a_Success()
        {
            Exceptional<int> exceptional = new InvalidCastException();

            Assert.False(exceptional.IsSuccess);
            Assert.True(exceptional.IsException);
        }

        [Fact]
        public void Exceptions_should_be_cast_to_Exceptional()
        {
            var exception = new InvalidCastException();
            Exceptional<int> exceptional = exception;

            Assert.True(exceptional.IsException);

            exceptional.OnException(ex => Assert.Equal(exception, ex));
        }

        [Fact]
        public void Data_of_T_should_cast_to_Exceptional()
        {
            const string value = "sample";
            Exceptional<string> exceptional = value;

            Assert.True(exceptional.IsSuccess);

            exceptional.OnSuccess(data => Assert.Equal(value, data));
        }

        [Fact]
        public async Task OnExceptionAsync_should_be_called_when_on_it_is_on_exception_state()
        {
            var exception = new InvalidCastException();
            Exceptional<int> exceptional = exception;

            Assert.True(exceptional.IsException);

            Task ShouldBeCalledOnException(Exception ex)
            {
                Assert.Equal(exception, ex);
                return Task.CompletedTask;
            }

            await exceptional.OnExceptionAsync(ShouldBeCalledOnException).ConfigureAwait(false);
        }

        [Fact]
        public async Task OnSuccessAsync_should_be_called_when_on_it_is_on_success_state()
        {
            const string value = "sample";
            Exceptional<string> exceptional = value;

            Assert.True(exceptional.IsSuccess);

            Task ShouldBeCalledOnSuccess(string data)
            {
                Assert.Equal(value, data);
                return Task.CompletedTask;
            }

            await exceptional.OnSuccessAsync(ShouldBeCalledOnSuccess);
        }

        [Fact]
        public void Match_should_convert_the_underlying_type_to_R()
        {
            var data = Exceptional<int>.Success(1);

            var result =data.Match(
                _ => "no error",
                d => d.ToString()
            );

            result.Should().BeOfType<string>();
            result.Should().Be("1");

        }
        
        [Fact]
        public void Match_should_convert_the_underlying_exception_to_R()
        {
            Exceptional<int> data = new InvalidOperationException("error"); 

            var result =data.Match(
                ex => ex.Message,
                d => d.ToString()
            );

            result.Should().BeOfType<string>();
            result.Should().Be("error");

        }
        
    }
}
