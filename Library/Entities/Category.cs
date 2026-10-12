using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public sealed class Category : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "نام دسته‌بندی را وارد کنید.")]
    [Display(Name = "نام دسته‌بندی")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "توضیحات دسته‌بندی را وارد کنید.")]
    [Display(Name = "توضیحات")]
    public string Description { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Title is { Length: > 120 })
            yield return new ValidationResult("نام دسته‌بندی نمی‌تواند بیشتر از ۱۲۰ کاراکتر باشد.", [nameof(Title)]);
        if (Description is { Length: > 800 })
            yield return new ValidationResult("توضیحات نمی‌تواند بیشتر از ۸۰۰ کاراکتر باشد.", [nameof(Description)]);
    }
}
