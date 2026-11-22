# 🕌 حافظ (Hafez)

[![Hafez CI](https://github.com/HamidRezaAshkiyan/Hafez/actions/workflows/ci.yml/badge.svg)](https://github.com/HamidRezaAshkiyan/Hafez/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

**حافظ (Hafez)** یک نرم‌افزار متن‌باز و مدرن مبتنی بر **.NET 10** و **WPF** برای مدیریت نمایشگرهای LED، ویدئوپروژکتورها و تابلوهای اعلانات در مساجد و مراسم مذهبی است.

🌐 **وب‌سایت پروژه:** https://hamidrezaashkiyan.github.io/Hafez/  
📱 **اپلیکیشن کنترلر اندروید:** [HamidRezaAshkiyan/hafez_controller](https://github.com/HamidRezaAshkiyan/hafez_controller)  
📦 **دانلود آخرین نسخه:** [GitHub Releases](https://github.com/HamidRezaAshkiyan/Hafez/releases/latest)

---

## ✨ ویژگی‌های کلیدی نسخه ۲.۰

- **پرتابل و بدون نیاز به SQL Server:** مهاجرت کامل از LocalDB به **SQLite** (ذخیره‌سازی خودکار در `%AppData%\Hafez\hafez.db`) و تنظیمات **JSON** در `%AppData%\Hafez\settings.json`.
- **کنترل بی‌سیم با SignalR و اسکن QR:** سرور داخلی ASP.NET Core Minimal API + SignalR (`/hub/hafez`) با اتصال سریع موبایل از طریق اسکن QR Code.
- **قرآن، مفاتیح و ادعیه اختصاصی:** نمایش متن عربی و ترجمه فارسی همراه با جستجوی هوشمند بدون حساسیت به اعراب و خروجی/ورودی اکسل (با ClosedXML و بدون نیاز به نصب Office).
- **اوقات شرعی و تقویم هجری/شمسی:** محاسبه اوقات شرعی، شمارش معکوس تا اذان بعدی و نمایش مناسبت‌های مذهبی.
- **پخش زنده دوربین با OpenCvSharp4:** دریافت روان و کم‌مصرف تصاویر از وب‌کم و کارت‌های کپچر مدرن.
- **زیرنویس روان سخت‌افزاری (GPU) و پیش‌نمایش زنده:** انیمیشن روان مبتنی بر `TranslateTransform`، پیش‌نمایش زنده خروجی و انتقال خودکار نمایشگر به مانیتور دوم.
- **حالت شب (Dark Mode) و امنیت بهبودیافته:** هش رمز عبور با PBKDF2-SHA256 و رابط کاربری مناسب اتاق فرمان کم‌نور مساجد.

---

## ⌨️ کلیدهای میانبر اپراتور

| کلید میانبر | عملکرد |
|---|---|
| `Ctrl + Q` | باز کردن پنجره اتصال موبایل (QR Code) |
| `Ctrl + P` | باز کردن پیش‌نمایش زنده خروجی و اوقات شرعی |
| `Ctrl + D` | تغییر حالت شب / روز (Dark Mode) |
| `F11` | انتقال خودکار خروجی به مانیتور دوم (پروژکتور / LED) |

---

## 📥 نصب و اجرا

1. آخرین بسته ویندوز را از بخش [Releases](https://github.com/HamidRezaAshkiyan/Hafez/releases/latest) دانلود کنید (نسخه Self-Contained شامل .NET 10 بوده و نیازی به نصب پیش‌نیاز ندارد).
2. فایل فشرده را استخراج کرده و `HafezWPFUI.exe` را اجرا کنید.
3. برای کنترل از طریق موبایل، کلید `Ctrl + Q` را بزنید و کد QR را با اپلیکیشن [کنترل حافظ](https://github.com/HamidRezaAshkiyan/hafez_controller/releases/latest) اسکن کنید.

---

## 🛠️ ساخت از کد منبع (توسعه‌دهندگان)

پیش‌نیاز: **.NET 10 SDK** روی ویندوز

```bash
git clone https://github.com/HamidRezaAshkiyan/Hafez.git
cd Hafez
dotnet build HafezWPFUI/HafezWPFUI.csproj -c Release
dotnet test HafezLibrary.Tests/HafezLibrary.Tests.csproj -c Release
```

## 📄 مجوز

این پروژه تحت مجوز **MIT** منتشر شده است.
