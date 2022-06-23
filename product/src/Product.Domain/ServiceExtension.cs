using System;
using System.Collections.Generic;
using AutoMapper;
using Core.Commands;
using Core.EventStoreDB;
using Core.EventStoreDB.Repository;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Contract.Commands;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Behaviours;
using Product.Domain.Commands;
using Product.Domain.Converters;
using Product.Domain.Handlers;
using Product.Domain.Models;
using Product.Domain.Persistence;
using Product.Domain.Profiles;
using Product.Domain.Repositories;
using Product.Domain.Validators;

namespace Product.Domain
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddProductModule(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddDomainService()
                .AddEventStoreDB(config);

            return services;
        }
        
        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            services.AddDbContext<ProductContext>(optionsAction, contextLifetime, optionsLifetime);

            return services;
        }

        private static IServiceCollection AddDomainService(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<EntityToDtoProfile>();
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IEventStoreDBRepository<CurrencyModel>, EventStoreDBRepository<CurrencyModel>>()
                    .AddCommandValidators()
                    .AddCommandHandlers()
                    .AddQueryHandlers()
                    .AddConverters();

            return services;
        }

        private static IServiceCollection AddCommandValidators(this IServiceCollection services)
        {
            // attribute
            services.AddScoped<IValidator<CreateAttributeCommand>, CreateAttributeValidator>();
            services.AddScoped<IValidator<UpdateAttributeCommand>, UpdateAttributeValidator>();
            services.AddScoped<IValidator<DeleteAttributeCommand>, DeleteAttributeValidator>();

            // currency
            services.AddScoped<IValidator<CreateCurrencyCommand>, CreateCurrencyValidator>();
            services.AddScoped<IValidator<UpdateCurrencyCommand>, UpdateCurrencyValidator>();
            services.AddScoped<IValidator<DeleteCurrencyCommand>, DeleteCurrencyValidator>();

            // product
            services.AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>();
            services.AddScoped<IValidator<UpdateProductCommand>, UpdateProductValidator>();
            services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductValidator>();

            return services;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            // attribute
            services.AddScoped<IRequestHandler<CreateAttributeCommand, IAttributeDto>, CreateAttributeHandler>();
            services.AddScoped<IRequestHandler<UpdateAttributeCommand, IAttributeDto>, UpdateAttributeHandler>();
            services.AddScoped<IRequestHandler<DeleteAttributeCommand, IAttributeDto>, DeleteAttributeHandler>();

            // curency
            services.AddScoped<IRequestHandler<CreateCurrencyCommand, ICurrencyDto>, CreateCurrencyHandler>();
            services.AddScoped<IRequestHandler<UpdateCurrencyCommand, ICurrencyDto>, UpdateCurrencyHandler>();
            services.AddScoped<IRequestHandler<DeleteCurrencyCommand, ICurrencyDto>, DeleteCurrencyHandler>();

            // product
            services.AddScoped<IRequestHandler<CreateProductCommand, IProductDto>, CreateProductHandler>();
            services.AddScoped<IRequestHandler<UpdateProductCommand, IProductDto>, UpdateProductHandler>();
            services.AddScoped<IRequestHandler<DeleteProductCommand, IProductDto>, DeleteProductHandler>();


            services.AddCommandHandler<CreateCurrencyCmd, CreateCurrencyCmdHandler>();
            services.AddCommandHandler<UpdateCurrencyCmd, UpdateCurrencyCmdHandler>();

            return services;
        }

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            // attribute
            services.AddScoped<IRequestHandler<GetAllAttributesQuery, IEnumerable<IAttributeDto>>, GetAllAttributesHandler>();
            services.AddScoped<IRequestHandler<GetListAttributesQuery, IEnumerable<IAttributeDto>>, GetListAttributesHandler>();
            services.AddScoped<IRequestHandler<GetAttributeQuery, IAttributeDto>, GetAttributeHandler>();

            // currency
            services.AddScoped<IRequestHandler<GetAllCurrenciesQuery, IEnumerable<ICurrencyDto>>, GetAllCurrenciesHandler>();
            services.AddScoped<IRequestHandler<GetListCurrenciesQuery, IEnumerable<ICurrencyDto>>, GetListCurrenciesHandler>();
            services.AddScoped<IRequestHandler<GetCurrencyQuery, ICurrencyDto>, GetCurrencyHandler>();

            // product
            services.AddScoped<IRequestHandler<GetAllProductsQuery, IEnumerable<IProductDto>>, GetAllProductsHandler>();
            services.AddScoped<IRequestHandler<GetListProductsQuery, IEnumerable<IProductDto>>, GetListProductsHandler>();
            services.AddScoped<IRequestHandler<GetProductQuery, IProductDto>, GetProductHandler>();

            return services;
        }

        private static IServiceCollection AddConverters(this IServiceCollection services)
        {
            services.AddScoped<DtoConverter>();

            return services;
        }

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
}