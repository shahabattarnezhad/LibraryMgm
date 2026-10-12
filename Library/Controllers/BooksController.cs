using Library.Data;
using Library.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers;

public sealed class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .AsNoTracking()
            .Include(book => book.Category)
            .OrderBy(book => book.Name)
            .ToListAsync();

        return View(books);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var book = await _context.Books
            .AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id.Value);

        return book is null ? NotFound() : View(book);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? categoryId)
    {
        int? selectedCategoryId = null;
        if (categoryId.HasValue && await _context.Categories.AnyAsync(category => category.Id == categoryId.Value))
            selectedCategoryId = categoryId.Value;

        await PopulateCategoriesAsync(selectedCategoryId);
        return View(new Book { CategoryId = selectedCategoryId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ShortDescription,PhotoUrl,CategoryId")] Book book)
    {
        await ValidateCategoryAsync(book.CategoryId);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(book.CategoryId);
            return View(book);
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "کتاب جدید با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id.Value);
        if (book is null) return NotFound();

        await PopulateCategoriesAsync(book.CategoryId);
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ShortDescription,PhotoUrl,CategoryId")] Book input)
    {
        if (id != input.Id) return NotFound();

        await ValidateCategoryAsync(input.CategoryId);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(input.CategoryId);
            return View(input);
        }

        var book = await _context.Books.FirstOrDefaultAsync(item => item.Id == id);
        if (book is null) return NotFound();

        book.Name = input.Name;
        book.ShortDescription = input.ShortDescription;
        book.PhotoUrl = input.PhotoUrl;
        book.CategoryId = input.CategoryId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Books.AnyAsync(item => item.Id == id)) return NotFound();
            throw;
        }

        TempData["SuccessMessage"] = "تغییرات کتاب ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var book = await _context.Books
            .AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id.Value);

        return book is null ? NotFound() : View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "کتاب از فهرست حذف شد.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync(int? selectedCategoryId = null)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Title)
            .ToListAsync();

        ViewBag.CategoryId = new SelectList(categories, nameof(Category.Id), nameof(Category.Title), selectedCategoryId);
        ViewBag.HasCategories = categories.Count > 0;
    }

    private async Task ValidateCategoryAsync(int categoryId)
    {
        // Range validation already reports a missing selection; check existence only for positive IDs.
        if (categoryId <= 0) return;

        if (!await _context.Categories.AnyAsync(category => category.Id == categoryId))
            ModelState.AddModelError(nameof(Book.CategoryId), "دسته‌بندی انتخاب‌شده وجود ندارد. ابتدا یک دسته‌بندی معتبر انتخاب کنید.");
    }
}
