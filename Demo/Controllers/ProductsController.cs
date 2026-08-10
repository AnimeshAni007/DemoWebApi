using Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes your base URL path: api/products
    public class ProductsController : ControllerBase
    {
        //private readonly AppDbContext _context;
        // 1. DEPENDENCY INJECTION: This automatically requests the database context from Program.cs
        //public ProductsController(AppDbContext context)
        //{
        //    _context = context;
        //}
        //public class TodoItem
        //{
        //    public int Id { get; set; }
        //    public string Title { get; set; } = string.Empty;
        //}

        //// Temporary storage in system memory
        ////private static List<TodoItem> MyTasks = new List<TodoItem>
        ////{
        ////    new TodoItem { Id = 1, Title = "abc" },
        ////    new TodoItem { Id = 2, Title = "Configure launchSettings.json" }
        ////};

        //// 1. READ (Get all tasks)
        //[HttpGet("tasks")]
        //public async Task<IActionResult> GetTasks()
        //{
        //    var tasks = await _context.Products.ToListAsync();
        //    return Ok(tasks);
        //}

        //// 2. CREATE (Add a new task)
        //[HttpPost("add-task")]
        //public async Task<IActionResult> AddTask([FromQuery] string title)
        //{
        //    if (string.IsNullOrEmpty(title))
        //        return BadRequest("Title cannot be empty");

        //    // Build our tracking model. SQL Server manages auto-incrementing the ID columns automatically!
        //    var newItem = new ProductItem { Title = title };

        //    // Add the item to the EF tracker and commit to the actual database file
        //    _context.Products.Add(newItem);
        //    await _context.SaveChangesAsync();

        //    return Ok(newItem);

        //    //if (string.IsNullOrEmpty(title)) return BadRequest("Title cannot be empty");

        //    //var newId = MyTasks.Count > 0 ? MyTasks.Max(t => t.Id) + 1 : 1;
        //    //var newItem = new TodoItem { Id = newId, Title = title };
        //    //MyTasks.Add(newItem);

        //    //return Ok(newItem);
        //}

        //// 3. UPDATE (Edit an existing task's text)
        //[HttpPut("update-task/{id}")]
        //public async Task<IActionResult> UpdateTask(int id, [FromQuery] string abcTitle)
        //{
        //    if (string.IsNullOrEmpty(abcTitle))
        //        return BadRequest("Title cannot be empty");

        //    // Look inside your physical database rows for a row matching the Primary Key ID
        //    var task = await _context.Products.FindAsync(id);
        //    if (task == null)
        //        return NotFound("Task not found");

        //    // Update the entity property and save tracking changes back down to disk
        //    task.Title = abcTitle;
        //    await _context.SaveChangesAsync();

        //    return Ok(task);
        //    //var task = MyTasks.FirstOrDefault(t => t.Id == id);
        //    //if (task == null) return NotFound("Task not found");
        //    //if (string.IsNullOrEmpty(abcTitle)) return BadRequest("Title cannot be empty");

        //    //task.Title = abcTitle;
        //    //return Ok(task);
        //}

        //// 4. DELETE (Remove a task)
        //[HttpDelete("delete-task/{id}")]
        //public async Task<IActionResult> DeleteTask(int id)
        //{
        //    // Look inside your physical database rows for a row matching the Primary Key ID
        //    var task = await _context.Products.FindAsync(id);
        //    if (task == null)
        //        return NotFound("Task not found");

        //    // Instruct the engine to stage a deletion row scrub and save
        //    _context.Products.Remove(task);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = $"Deleted task {id}" });

        //    //var task = MyTasks.FirstOrDefault(t => t.Id == id);
        //    //if (task == null) return NotFound("Task not found");

        //    //MyTasks.Remove(task);
        //    //return Ok(new { message = $"Deleted task {id}" });
        //}

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Everyone with a valid token (Admin or User) can read tasks
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.Products.ToListAsync();
            return Ok(tasks);
        }

        // Lock writing endpoints down strictly to the "Admin" role claim
        [HttpPost("add-task")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTask([FromQuery] string title)
        {
            if (string.IsNullOrEmpty(title)) return BadRequest("Title cannot be empty");
            var newItem = new ProductItem { Title = title };
            _context.Products.Add(newItem);
            await _context.SaveChangesAsync();
            return Ok(newItem);
        }

        [HttpPut("update-task/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTask(int id, [FromQuery] string abcTitle)
        {
            if (string.IsNullOrEmpty(abcTitle)) return BadRequest("Title cannot be empty");
            var task = await _context.Products.FindAsync(id);
            if (task == null) return NotFound("Task not found");

            task.Title = abcTitle;
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("delete-task/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Products.FindAsync(id);
            if (task == null) return NotFound("Task not found");

            _context.Products.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Deleted task {id}" });
        }
    }
}