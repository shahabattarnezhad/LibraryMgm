using Library.Data;
using Library.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers;

public sealed class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Include(category => category.Books)
            .OrderBy(category => category.Title)
            .ToListAsync();

        return View(categories);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var category = await _context.Categories
            .AsNoTracking()
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id.Value);

        return category is null ? NotFound() : View(category);
    }

    [HttpGet]
    public IActionResult Create() => View(new Category());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description")] Category category)
    {
        if (!ModelState.IsValid) return View(category);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "دسته‌بندی جدید ایجاد شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id.Value);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Category input)
    {
        if (id != input.Id) return NotFound();
        if (!ModelState.IsValid) return View(input);

        var category = await _context.Categories.FirstOrDefaultAsync(item => item.Id == id);
        if (category is null) return NotFound();

        category.Title = input.Title;
        category.Description = input.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Categories.AnyAsync(item => item.Id == id)) return NotFound();
            throw;
        }

        TempData["SuccessMessage"] = "تغییرات دسته‌بندی ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var category = await _context.Categories
            .AsNoTracking()
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id.Value);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (category is null) return NotFound();

        if (category.Books.Count > 0)
        {
            ModelState.AddModelError(string.Empty, "این دسته‌بندی شامل کتاب است. ابتدا کتاب‌های آن را به دسته‌بندی دیگری منتقل کنید یا حذف کنید.");
            return View("Delete", category);
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "دسته‌بندی حذف شد.";
        return RedirectToAction(nameof(Index));
    }
}
