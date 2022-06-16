using System;
using System.Collections.Generic;
using AutoMapper;
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

namespace Product.Domain
{
    public static class ServiceExtension
    {
        public static void AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            services.AddDbContext<ProductContext>(optionsAction, contextLifetime, optionsLifetime);
        }

        public static void AddDomainService(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<EntityToDtoProfile>();
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // services.AddScoped<IProductRepository, ProductRepository>();

            AddCommandValidators(services);
            AddCommandHandlers(services);
            AddQueryHandlers(services);
            AddConverters(services);
        }

        private static void AddCommandValidators(IServiceCollection services)
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
        }

        private static void AddCommandHandlers(IServiceCollection services)
        {
            // attribute
            services.AddScoped<IRequestHandler<CreateAttributeCommand, IAttributeDto>, CreateAttributeHandler>();
            services.AddScoped<IRequestHandler<UpdateAttributeCommand, IAttributeDto>, UpdateAttributeHandler>();
            services.AddScoped<IRequestHandler<DeleteAttributeCommand, IAttributeDto>, DeleteAttributeHandler>();

            // config
            services.AddScoped<IRequestHandler<CreateCurrencyCommand, ICurrencyDto>, CreateCurrencyHandler>();
            services.AddScoped<IRequestHandler<UpdateCurrencyCommand, ICurrencyDto>, UpdateCurrencyHandler>();
            services.AddScoped<IRequestHandler<DeleteCurrencyCommand, ICurrencyDto>, DeleteCurrencyHandler>();

            // product
            services.AddScoped<IRequestHandler<CreateProductCommand, IProductDto>, CreateProductHandler>();
            services.AddScoped<IRequestHandler<UpdateProductCommand, IProductDto>, UpdateProductHandler>();
            services.AddScoped<IRequestHandler<DeleteProductCommand, IProductDto>, DeleteProductHandler>();
        }

        private static void AddQueryHandlers(IServiceCollection services)
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
        }

        private static void AddConverters(IServiceCollection services)
        {
            services.AddScoped<DtoConverter>();
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