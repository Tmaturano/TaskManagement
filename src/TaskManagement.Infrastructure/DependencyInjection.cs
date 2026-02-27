using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TaskManagementDb"));

        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }
}
