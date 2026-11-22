using HafezLibrary.Services;
using Xunit;

namespace HafezLibrary.Tests
{
    public class ContentCacheTests
    {
        [Theory]
        [InlineData("بِسْمِ اللَّهِ الرَّحْمَٰنِ الرَّحِيمِ", "بسم الله الرحمن الرحیم")]
        [InlineData("كِتَابُ الْعَرَبِيَّة", "کتاب العربیه")]
        [InlineData("آیت الکرسی", "ایت الکرسی")]
        public void NormalizeArabicPersian_ShouldStripDiacriticsAndUnifyLetters(string input, string expected)
        {
            string normalized = ContentCacheService.NormalizeArabicPersian(input);
            Assert.Equal(expected, normalized);
        }
    }
}
