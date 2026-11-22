using System;
using System.Collections.Generic;
using System.Globalization;

namespace HafezLibrary.Services
{
    public record PrayerTimes(
        DateTime Date,
        TimeSpan Fajr,
        TimeSpan Sunrise,
        TimeSpan Dhuhr,
        TimeSpan Asr,
        TimeSpan Sunset,
        TimeSpan Maghrib,
        TimeSpan Isha);

    public record NextPrayerInfo(string Name, TimeSpan Time, TimeSpan Remaining);

    /// <summary>
    /// Computes daily Islamic prayer times (Tehran / Institute of Geophysics method),
    /// Jalali (Shamsi) & Hijri dates, religious occasions, and next-prayer countdowns.
    /// </summary>
    public static class MosqueCalendarService
    {
        private static readonly string[] PersianMonths =
        {
            "", "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        private static readonly string[] HijriMonths =
        {
            "", "محرم", "صفر", "ربیع‌الاول", "ربیع‌الثانی", "جمادی‌الاول", "جمادی‌الثانی",
            "رجب", "شعبان", "رمضان", "شوال", "ذی‌القعده", "ذی‌الحجه"
        };

        private static readonly Dictionary<(int Month, int Day), string> HijriOccasions = new()
        {
            [(1, 1)] = "آغاز سال نو هجری قمری",
            [(1, 9)] = "تاسوعای حسینی",
            [(1, 10)] = "عاشورای حسینی",
            [(2, 20)] = "اربعین حسینی",
            [(2, 28)] = "رحلت حضرت رسول اکرم (ص) و شهادت امام حسن مجتبی (ع)",
            [(2, 30)] = "شهادت امام رضا (ع)",
            [(3, 17)] = "ولادت حضرت رسول اکرم (ص) و امام جعفر صادق (ع)",
            [(7, 13)] = "ولادت امام علی (ع)",
            [(7, 27)] = "عید سعید مبعث",
            [(8, 15)] = "ولادت حضرت ولی‌عصر (عج) — نیمه شعبان",
            [(9, 19)] = "شب قدر و ضربت خوردن امام علی (ع)",
            [(9, 21)] = "شهادت امام علی (ع)",
            [(9, 23)] = "شب قدر",
            [(10, 1)] = "عید سعید فطر",
            [(12, 10)] = "عید سعید قربان",
            [(12, 18)] = "عید سعید غدیر خم"
        };

        public static PrayerTimes CalculatePrayerTimes(
            DateTime date,
            double latitude = 35.6892,
            double longitude = 51.3890,
            double timeZoneHours = 3.5)
        {
            double jd = JulianDate(date.Year, date.Month, date.Day) - longitude / (15.0 * 24.0);
            (double decl, double eqt) = SunPosition(jd + 0.5);

            double dhuhr = FixHour(12.0 + timeZoneHours - longitude / 15.0 - eqt);
            double sunrise = dhuhr - SunAngleTime(0.833, latitude, decl);
            double sunset = dhuhr + SunAngleTime(0.833, latitude, decl);
            double fajr = dhuhr - SunAngleTime(17.7, latitude, decl);
            double maghrib = dhuhr + SunAngleTime(4.5, latitude, decl);
            double isha = dhuhr + SunAngleTime(14.0, latitude, decl);
            double asr = dhuhr + AsrTime(1.0, latitude, decl);

            return new PrayerTimes(
                date.Date,
                HoursToTimeSpan(fajr),
                HoursToTimeSpan(sunrise),
                HoursToTimeSpan(dhuhr),
                HoursToTimeSpan(asr),
                HoursToTimeSpan(sunset),
                HoursToTimeSpan(maghrib),
                HoursToTimeSpan(isha));
        }

        public static NextPrayerInfo GetNextPrayer(DateTime now, PrayerTimes times)
        {
            TimeSpan current = now.TimeOfDay;
            (string Name, TimeSpan Time)[] schedule =
            {
                ("اذان صبح", times.Fajr),
                ("طلوع آفتاب", times.Sunrise),
                ("اذان ظهر", times.Dhuhr),
                ("اذان عصر", times.Asr),
                ("اذان مغرب", times.Maghrib),
                ("اذان عشا", times.Isha)
            };

            foreach (var (name, time) in schedule)
            {
                if (time > current)
                {
                    return new NextPrayerInfo(name, time, time - current);
                }
            }

            TimeSpan tomorrowFajrRemaining = TimeSpan.FromHours(24) - current + times.Fajr;
            return new NextPrayerInfo("اذان صبح فردا", times.Fajr, tomorrowFajrRemaining);
        }

        public static string FormatPersianDate(DateTime date)
        {
            var pc = new PersianCalendar();
            int y = pc.GetYear(date);
            int m = pc.GetMonth(date);
            int d = pc.GetDayOfMonth(date);
            return $"{d} {PersianMonths[m]} {y}";
        }

        public static string FormatHijriDate(DateTime date)
        {
            var hc = new HijriCalendar();
            int y = hc.GetYear(date);
            int m = hc.GetMonth(date);
            int d = hc.GetDayOfMonth(date);
            return $"{d} {HijriMonths[m]} {y}";
        }

        public static string GetReligiousOccasion(DateTime date)
        {
            var hc = new HijriCalendar();
            int m = hc.GetMonth(date);
            int d = hc.GetDayOfMonth(date);
            return HijriOccasions.TryGetValue((m, d), out string occasion) ? occasion : string.Empty;
        }

        public static string BuildBannerText(DateTime now, string cityName, double lat, double lon, double tz)
        {
            PrayerTimes times = CalculatePrayerTimes(now, lat, lon, tz);
            NextPrayerInfo next = GetNextPrayer(now, times);
            string persianDate = FormatPersianDate(now);
            string hijriDate = FormatHijriDate(now);
            string occasion = GetReligiousOccasion(now);

            string baseBanner =
                $"{persianDate} | {hijriDate} | {cityName} — {next.Name}: {next.Time:hh\\:mm} (باقی‌مانده: {next.Remaining:hh\\:mm})";

            return string.IsNullOrEmpty(occasion) ? baseBanner : $"{occasion}  •  {baseBanner}";
        }

        private static double JulianDate(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            double a = Math.Floor(year / 100.0);
            double b = 2 - a + Math.Floor(a / 4.0);
            return Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5;
        }

        private static (double Declination, double EquationOfTime) SunPosition(double jd)
        {
            double d = jd - 2451545.0;
            double g = FixAngle(357.529 + 0.98560028 * d);
            double q = FixAngle(280.459 + 0.98564736 * d);
            double l = FixAngle(q + 1.915 * Math.Sin(DegToRad(g)) + 0.020 * Math.Sin(DegToRad(2 * g)));
            double e = 23.439 - 0.00000036 * d;

            double ra = RadToDeg(Math.Atan2(Math.Cos(DegToRad(e)) * Math.Sin(DegToRad(l)), Math.Cos(DegToRad(l)))) / 15.0;
            double decl = RadToDeg(Math.Asin(Math.Sin(DegToRad(e)) * Math.Sin(DegToRad(l))));
            double eqt = q / 15.0 - FixHour(ra);

            return (decl, eqt);
        }

        private static double SunAngleTime(double angle, double latitude, double decl)
        {
            double numerator = -Math.Sin(DegToRad(angle)) - Math.Sin(DegToRad(latitude)) * Math.Sin(DegToRad(decl));
            double denominator = Math.Cos(DegToRad(latitude)) * Math.Cos(DegToRad(decl));
            double cosH = Math.Clamp(numerator / denominator, -1.0, 1.0);
            return RadToDeg(Math.Acos(cosH)) / 15.0;
        }

        private static double AsrTime(double factor, double latitude, double decl)
        {
            double angle = -RadToDeg(Math.Atan(1.0 / (factor + Math.Tan(Math.Abs(DegToRad(latitude - decl))))));
            return SunAngleTime(angle, latitude, decl);
        }

        private static TimeSpan HoursToTimeSpan(double hours)
        {
            double normalized = FixHour(hours);
            int totalMinutes = (int)Math.Round(normalized * 60.0);
            return TimeSpan.FromMinutes(totalMinutes % (24 * 60));
        }

        private static double DegToRad(double d) => d * Math.PI / 180.0;
        private static double RadToDeg(double r) => r * 180.0 / Math.PI;
        private static double FixAngle(double a) => a - 360.0 * Math.Floor(a / 360.0);
        private static double FixHour(double h) => h - 24.0 * Math.Floor(h / 24.0);
    }
}
