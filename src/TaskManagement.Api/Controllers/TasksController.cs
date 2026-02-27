using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetAllTasksAsync(cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    [ProducesResponseType<TaskResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await taskService.CreateTaskAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
    }

    [HttpPatch("{id:guid}/toggle")]
    [ProducesResponseType<TaskResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskService.ToggleTaskAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }
}
