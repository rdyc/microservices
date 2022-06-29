using System;
using System.Collections.Generic;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Behaviours;
using Product.Domain.Converters;
using Product.Domain.Handlers;
using Product.Domain.Persistence;
using Product.Domain.Profiles;
using Product.Domain.Repositories;
using Product.Domain.Validators;

namespace Product.Domain;

public static class ServiceExtension
{        
    public static IServiceCollection AddDomainContext(this IServiceCollection services, 
        Action<DbContextOptionsBuilder> optionsAction, 
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped, 
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) =>
        services.AddDbContext<ProductContext>(optionsAction, contextLifetime, optionsLifetime);

    public static IServiceCollection AddDomainService(this IServiceCollection services) =>
        services
            .AddAutoMapper(config =>
            {
                config.AddProfile<EntityToDtoProfile>();
            })
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            // .AddScoped<IProductRepository, ProductRepository>();
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddConverters();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            // attribute
            .AddScoped<IValidator<CreateAttributeCommand>, CreateAttributeValidator>()
            .AddScoped<IValidator<UpdateAttributeCommand>, UpdateAttributeValidator>()
            .AddScoped<IValidator<DeleteAttributeCommand>, DeleteAttributeValidator>()
            // currency
            .AddScoped<IValidator<CreateCurrencyCommand>, CreateCurrencyValidator>()
            .AddScoped<IValidator<UpdateCurrencyCommand>, UpdateCurrencyValidator>()
            .AddScoped<IValidator<DeleteCurrencyCommand>, DeleteCurrencyValidator>()
            // product
            .AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>()
            .AddScoped<IValidator<UpdateProductCommand>, UpdateProductValidator>()
            .AddScoped<IValidator<DeleteProductCommand>, DeleteProductValidator>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            // attribute
            .AddScoped<IRequestHandler<CreateAttributeCommand, IAttributeDto>, CreateAttributeHandler>()
            .AddScoped<IRequestHandler<UpdateAttributeCommand, IAttributeDto>, UpdateAttributeHandler>()
            .AddScoped<IRequestHandler<DeleteAttributeCommand, IAttributeDto>, DeleteAttributeHandler>()
            // curency
            .AddScoped<IRequestHandler<CreateCurrencyCommand, ICurrencyDto>, CreateCurrencyHandler>()
            .AddScoped<IRequestHandler<UpdateCurrencyCommand, ICurrencyDto>, UpdateCurrencyHandler>()
            .AddScoped<IRequestHandler<DeleteCurrencyCommand, ICurrencyDto>, DeleteCurrencyHandler>()
            // product
            .AddScoped<IRequestHandler<CreateProductCommand, IProductDto>, CreateProductHandler>()
            .AddScoped<IRequestHandler<UpdateProductCommand, IProductDto>, UpdateProductHandler>()
            .AddScoped<IRequestHandler<DeleteProductCommand, IProductDto>, DeleteProductHandler>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            // attribute
            .AddScoped<IRequestHandler<GetAllAttributesQuery, IEnumerable<IAttributeDto>>, GetAllAttributesHandler>()
            .AddScoped<IRequestHandler<GetListAttributesQuery, IEnumerable<IAttributeDto>>, GetListAttributesHandler>()
            .AddScoped<IRequestHandler<GetAttributeQuery, IAttributeDto>, GetAttributeHandler>()
            // currency
            .AddScoped<IRequestHandler<GetAllCurrenciesQuery, IEnumerable<ICurrencyDto>>, GetAllCurrenciesHandler>()
            .AddScoped<IRequestHandler<GetListCurrenciesQuery, IEnumerable<ICurrencyDto>>, GetListCurrenciesHandler>()
            .AddScoped<IRequestHandler<GetCurrencyQuery, ICurrencyDto>, GetCurrencyHandler>()
            // product
            .AddScoped<IRequestHandler<GetAllProductsQuery, IEnumerable<IProductDto>>, GetAllProductsHandler>()
            .AddScoped<IRequestHandler<GetListProductsQuery, IEnumerable<IProductDto>>, GetListProductsHandler>()
            .AddScoped<IRequestHandler<GetProductQuery, IProductDto>, GetProductHandler>();

    private static IServiceCollection AddConverters(this IServiceCollection services) =>
        services.AddScoped<DtoConverter>();

    public static void UseDbMigration(IServiceScope scope)
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ProductContext>();

            using (context)
            {
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.GetBaseException().Message);
        }
    }
}