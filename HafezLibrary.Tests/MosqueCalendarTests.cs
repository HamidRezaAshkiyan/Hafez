using System;
using HafezLibrary.Services;
using Xunit;

namespace HafezLibrary.Tests
{
    public class MosqueCalendarTests
    {
        [Fact]
        public void CalculatePrayerTimes_Tehran_ShouldBeInChronologicalOrder()
        {
            var date = new DateTime(2026, 6, 21);
            PrayerTimes times = MosqueCalendarService.CalculatePrayerTimes(date, 35.6892, 51.3890, 3.5);

            Assert.True(times.Fajr < times.Sunrise);
            Assert.True(times.Sunrise < times.Dhuhr);
            Assert.True(times.Dhuhr < times.Asr);
            Assert.True(times.Asr < times.Sunset);
            Assert.True(times.Sunset < times.Maghrib);
            Assert.True(times.Maghrib < times.Isha);
        }

        [Fact]
        public void GetNextPrayer_ShouldReturnUpcomingPrayerAndPositiveRemaining()
        {
            var now = new DateTime(2026, 6, 21, 10, 0, 0);
            PrayerTimes times = MosqueCalendarService.CalculatePrayerTimes(now);
            NextPrayerInfo next = MosqueCalendarService.GetNextPrayer(now, times);

            Assert.Equal("اذان ظهر", next.Name);
            Assert.True(next.Remaining > TimeSpan.Zero);
        }

        [Fact]
        public void FormatDatesAndBanner_ShouldReturnNonEmptyPersianAndHijriStrings()
        {
            var now = new DateTime(2026, 3, 21, 12, 0, 0);
            string persian = MosqueCalendarService.FormatPersianDate(now);
            string hijri = MosqueCalendarService.FormatHijriDate(now);
            string banner = MosqueCalendarService.BuildBannerText(now, "تهران", 35.6892, 51.3890, 3.5);

            Assert.Contains("فروردین", persian);
            Assert.NotEmpty(hijri);
            Assert.Contains("تهران", banner);
        }
    }
}
