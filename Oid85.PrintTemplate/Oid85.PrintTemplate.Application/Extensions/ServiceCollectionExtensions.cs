using Microsoft.Extensions.DependencyInjection;
using Oid85.PrintTemplate.Application.Interfaces.Services;
using Oid85.PrintTemplate.Application.Services;

namespace Oid85.PrintTemplate.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddTransient<IPrintTemplateService, PrintTemplateService>();
    }
}