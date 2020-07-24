using Bookcase.Services.Catalog.API.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace Catalog.UnitTests.ViewModels
{
    public class BookInputViewModelTest
    {
        [Theory]
        [InlineData("9780394415765")]
        [InlineData("978 0 394 41576 5")]
        [InlineData("978-0-394-41576-5")]
        [InlineData("0-394-41576-5")]
        public void Isbn_ValidValue_Success(string isbn)
        {
            var b = new BookInputViewModel();
            b.Isbn = isbn;
            ValidateProperty(b).Invoke();
        }

        [Theory]
        [InlineData("22")]
        [InlineData("12345678901234567890")]
        [InlineData("0123456789-abc")]
        public void Isbn_InvalidValue_ThrowsValidationException(string isbn)
        {
            var b = new BookInputViewModel();
            b.Isbn = isbn;
            Assert.Throws<ValidationException>(ValidateProperty(b));
        }

        private Action ValidateProperty(BookInputViewModel vm)
        {
            return () =>
            {
                var ctx = new ValidationContext(vm) { MemberName = "Isbn" };
                Validator.ValidateProperty(vm.Isbn, ctx);
            };
        }
    }
}
