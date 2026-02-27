using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs;

public sealed record CreateTaskRequest(
    [Required, MinLength(1), MaxLength(200)] string Title,
    [MaxLength(1000)] string? Description
);
