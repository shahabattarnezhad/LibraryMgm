# کتابخانه‌یار

پنل مدیریت کتاب‌ها و دسته‌بندی‌ها با ASP.NET Core MVC و Entity Framework Core 10.

## نیازمندی‌ها

- .NET SDK 10
- SQL Server (یا LocalDB در صورت تغییر Connection String)
- دسترسی اینترنت در زمان اجرای رابط کاربری برای دریافت Tailwind CSS و فونت Vazirmatn از CDN

## راه‌اندازی

1. فایل `Library/appsettings.json` را باز کنید و مقدار `ConnectionStrings:BookConnection` را با سرور SQL Server خود هماهنگ کنید.
2. از پوشه‌ای که فایل `Library.slnx` قرار دارد، در ترمینال اجرا کنید:

   ```bash
   dotnet restore Library/Library.csproj
   dotnet tool install --global dotnet-ef --version 10.0.12
   dotnet ef database update --project Library/Library.csproj --startup-project Library/Library.csproj
   dotnet run --project Library/Library.csproj
   ```

   اگر ابزار `dotnet-ef` از قبل نصب شده است، مرحله نصب ابزار را رد کنید. اگر نسخه ابزار نصب‌شده متفاوت است، نسخه سازگار با EF Core 10 را به کار ببرید.

3. آدرس اجراشده در ترمینال را در مرورگر باز کنید. پروفایل HTTP پروژه به‌صورت پیش‌فرض از `http://localhost:5208` استفاده می‌کند.

## امکانات

- عملیات فهرست، ایجاد، مشاهده جزئیات، ویرایش و حذف کتاب و دسته‌بندی
- انتخاب دسته‌بندی از فهرست معتبر و جلوگیری از حذف دسته‌بندی دارای کتاب
- دریافت و ذخیره فقط URL تصویر در ویژگی `Book.PhotoUrl`؛ آپلود فایل تصویر در این پروژه انجام نمی‌شود
- پیش‌نمایش زنده تصویر پس از واردکردن URL
- تم روشن و تاریک با ذخیره انتخاب کاربر در مرورگر
- نمایش وضعیت در حال انجام داخل دکمه‌های ثبت، ذخیره و حذف
- اعتبارسنجی سمت سرور و سمت کاربر

## نکته تصویر

ابتدا فایل تصویر را در سرویس ثالث موردنظر آپلود کنید و سپس URL مستقیم تصویر را در فیلد «آدرس تصویر» قرار دهید. این برنامه هیچ فایل تصویری را به سرویس ثالث ارسال نمی‌کند.
