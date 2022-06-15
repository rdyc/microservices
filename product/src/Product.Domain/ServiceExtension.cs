using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Contract.Dtos;
using Product.Contract.Queries;
using Product.Domain.Persistence;
using Product.Domain.Repositories;
using Product.Domain.Handler.Query;
using Product.Contract.Commands;
using Product.Domain.Behaviours;
using Product.Domain.Validator.Command;
using FluentValidation;
using Product.Domain.Converters;
using Product.Domain.Profiles;
using AutoMapper;

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
            services.AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>();
            services.AddScoped<IValidator<UpdateProductCommand>, UpdateProductValidator>();
            services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductValidator>();
        }

        private static void AddCommandHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateProductCommand, IProductDto>, CreateProductHandler>();
            services.AddScoped<IRequestHandler<UpdateProductCommand, IProductDto>, UpdateProductHandler>();
            services.AddScoped<IRequestHandler<DeleteProductCommand, IProductDto>, DeleteProductHandler>();
        }

        private static void AddQueryHandlers(IServiceCollection services)
        {
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