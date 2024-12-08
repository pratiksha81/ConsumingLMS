using Microsoft.AspNetCore.Mvc;
using ConsumingLMS.Services;
using ConsumingLMS.Models;

namespace ConsumingLMS.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // Display list of authors and the add/edit form
        public async Task<IActionResult> Author(int? id = null)
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            Auth currentAuthor = null;

            if (id.HasValue)
            {
                currentAuthor = await _authorService.GetAuthorByIdAsync(id.Value);
                if (currentAuthor == null)
                {
                    ModelState.AddModelError("", "Author not found.");
                }
            }

            var viewModel = new AuthorViewModel
            {
                Authors = authors.ToList(),
                CurrentAuthor = currentAuthor
            };

            return View(viewModel);
        }


        // Add or update an author
        [HttpPost]
        public async Task<IActionResult> SaveAuthor(Auth author)
        {
            if (ModelState.IsValid)
            {
                if (author.AuthorID == 0)
                {
                    // Add author
                    var addSuccess = await _authorService.AddAuthorAsync(author);
                    if (!addSuccess)
                    {
                        ModelState.AddModelError("", "Failed to add author.");
                        Console.WriteLine("Failed to add author");
                    }
                }
                else
                {
                    // Update author
                    var updateSuccess = await _authorService.UpdateAuthorAsync(author);
                    if (!updateSuccess)
                    {
                        ModelState.AddModelError("", "Failed to update author.");
                        Console.WriteLine("Failed to update author");
                    }
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Author");
                }
            }

            // Reload authors in case of errors
            var authors = await _authorService.GetAllAuthorsAsync();
            var viewModel = new AuthorViewModel
            {
                Authors = authors.ToList(),
                CurrentAuthor = author
            };

            return View("Author", viewModel);
        }

        // Delete an author
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var deleteSuccess = await _authorService.DeleteAuthorAsync(id);
            if (!deleteSuccess)
            {
                ModelState.AddModelError("", "Failed to delete author.");
                Console.WriteLine($"Failed to delete author with ID {id}");
            }

            return RedirectToAction("Author");
        }
    }
}
