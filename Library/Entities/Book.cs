using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public sealed class Book : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان کتاب را وارد کنید.")]
    [Display(Name = "عنوان کتاب")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "توضیحات کوتاه را وارد کنید.")]
    [Display(Name = "توضیحات کوتاه")]
    public string ShortDescription { get; set; } = string.Empty;

    // فایل تصویر در سرویس ثالث آپلود می‌شود؛ در اینجا فقط آدرس آن ذخیره می‌شود.
    [Url(ErrorMessage = "یک آدرس معتبر برای تصویر وارد کنید (با http یا https).")]
    [Display(Name = "آدرس تصویر")]
    public string? PhotoUrl { get; set; }

    [Display(Name = "دسته‌بندی")]
    [Range(1, int.MaxValue, ErrorMessage = "یک دسته‌بندی انتخاب کنید.")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name is { Length: > 180 })
            yield return new ValidationResult("عنوان کتاب نمی‌تواند بیشتر از ۱۸۰ کاراکتر باشد.", [nameof(Name)]);
        if (ShortDescription is { Length: > 1200 })
            yield return new ValidationResult("توضیحات نمی‌تواند بیشتر از ۱۲۰۰ کاراکتر باشد.", [nameof(ShortDescription)]);
        if (PhotoUrl is { Length: > 2048 })
            yield return new ValidationResult("آدرس تصویر بیش از حد طولانی است.", [nameof(PhotoUrl)]);
    }
}
