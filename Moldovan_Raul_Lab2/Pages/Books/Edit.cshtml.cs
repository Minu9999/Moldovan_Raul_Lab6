using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moldovan_Raul_Lab2.Data;
using Moldovan_Raul_Lab2.Models;

namespace Moldovan_Raul_Lab2.Pages.Books
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Moldovan_Raul_Lab2.Data.Moldovan_Raul_Lab2Context _context;

        public EditModel(Moldovan_Raul_Lab2.Data.Moldovan_Raul_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                        .Include(b => b.Publisher)
                        .Include(a => a.Author)
                        .Include(b => b.BookCategories).ThenInclude(b => b.Category)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            //apelam PopulateAssignedCategoryData pentru o obtine informatiile necesare checkbox-
            //urilor folosind clasa AssignedCategoryData
            PopulateAssignedCategoryData(_context, book);

            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "ID",
            "PublisherName");

            //var authorList = _context.Author.Select(x => new
            //{
            //    x.ID,
            //    FullName = x.LastName + " " + x.FirstName
            //});
            ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "ID",
            "LastName");

            Book = book;


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }
            //se va include Author conform cu sarcina de la lab 2
            var bookToUpdate = await _context.Book
                .Include(a => a.Author)
                .Include(i => i.Publisher)
                .Include(i => i.BookCategories)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            //se va modifica AuthorID conform cu sarcina de la lab 2
            if (await TryUpdateModelAsync<Book>(bookToUpdate, "Book",
            i => i.Title, i => i.Author,
            i => i.Price, i => i.PublishingDate, i => i.PublisherID))
            {
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            //Apelam UpdateBookCategories pentru a aplica informatiile din checkboxuri la entitatea Books care
            //este editata
            UpdateBookCategories(_context, selectedCategories, bookToUpdate);
            PopulateAssignedCategoryData(_context, bookToUpdate);

            return Page();
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}