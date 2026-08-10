using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        public class TodoItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
        }

        // Temporary storage in system memory
        private static List<TodoItem> MyTasks = new List<TodoItem>
        {
        new TodoItem { Id = 1, Title = "Learn .NET API Setup" },
        new TodoItem { Id = 2, Title = "Configure launchSettings.json" }
        };

        // 1. READ (Get all tasks)
        [HttpGet("tasks")]
        public IActionResult GetTasks()
        {
            return Ok(MyTasks);
        }

        // 2. CREATE (Add a new task)
        [HttpPost("add-task")]
        public IActionResult AddTask([FromQuery] string title)
        {
            if (string.IsNullOrEmpty(title)) return BadRequest("Title cannot be empty");

            var newId = MyTasks.Count > 0 ? MyTasks.Max(t => t.Id) + 1 : 1;
            var newItem = new TodoItem { Id = newId, Title = title };
            MyTasks.Add(newItem);

            return Ok(newItem);
        }

        // 3. UPDATE (Edit an existing task's text)
        [HttpPut("update-task/{id}")]
        public IActionResult UpdateTask(int id, [FromQuery] string abcTitle)
        {
            var task = MyTasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound("Task not found");
            if (string.IsNullOrEmpty(abcTitle)) return BadRequest("Title cannot be empty");

            task.Title = abcTitle;
            return Ok(task);
        }

        // 4. DELETE (Remove a task)
        [HttpDelete("delete-task/{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = MyTasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound("Task not found");

            MyTasks.Remove(task);
            return Ok(new { message = $"Deleted task {id}" });
        }
    }  
}
