using Bookcase.Services.Catalog.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Catalog.UnitTests.Models
{
    public class BookTest
    {
        [Theory]
        [InlineData("9780394415765", "9780394415765")]
        [InlineData("978 0 394 41576 5", "9780394415765")]
        [InlineData("978-0-394-41576-5", "9780394415765")]
        [InlineData("0394415760", "0394415760")]
        public void Isbn_SetValid_Success(string isbnToSet, string resultIsbn)
        {
            var b = new Book();
            b.Isbn = isbnToSet;
            Assert.Equal(resultIsbn, b.Isbn);
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("1234567890123")]
        [InlineData("123-456-789-01-23")]
        [InlineData("20 56 789 01 23")]
        public void Isbn_SetInvalid_ThrowsArgumentException(string isbn)
        {
            var b = new Book();
            Assert.Throws<ArgumentException>(() => b.Isbn = isbn);
        }
    }
}
