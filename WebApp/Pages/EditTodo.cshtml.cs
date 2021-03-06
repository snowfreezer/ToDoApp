using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class EditTodo : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditTodo(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Todo Todo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Todo = await _context.Todos
                .Include(t => t.Category)
                .Include(t => t.User).FirstOrDefaultAsync(m => m.TodoId == id);

            if (Todo == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(Todo.TodoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("TodoList", new {id = Todo.UserId});
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.TodoId == id);
        }
    }
}