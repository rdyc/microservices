using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Contract.Dto;
using Product.Contract.Query;
using Product.Domain.Context;
using Product.Domain.Repository;
using Product.Domain.Handler.Query;
using Product.Contract.Command;
using Product.Domain.Behaviours;
using Product.Domain.Validator.Command;
using FluentValidation;

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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // services.AddScoped<IProductRepository, ProductRepository>();

            AddCommandValidators(services);
            AddCommandHandlers(services);
            AddQueryHandlers(services);
        }

        private static void AddCommandValidators(IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateItemCommand>, CreateItemValidator>();
            services.AddScoped<IValidator<UpdateItemCommand>, UpdateItemValidator>();
            services.AddScoped<IValidator<DeleteItemCommand>, DeleteItemValidator>();
        }
        
        private static void AddCommandHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateItemCommand, IItemDto>, CreateItemHandler>();
            services.AddScoped<IRequestHandler<UpdateItemCommand, IItemDto>, UpdateItemHandler>();
            services.AddScoped<IRequestHandler<DeleteItemCommand, IItemDto>, DeleteItemHandler>();
        }

        private static void AddQueryHandlers(IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetAllItemsQuery, IEnumerable<IItemDto>>, GetAllItemsHandler>();
            services.AddScoped<IRequestHandler<GetListItemsQuery, IEnumerable<IItemDto>>, GetListItemsHandler>();
            services.AddScoped<IRequestHandler<GetItemQuery, IItemDto>, GetItemHandler>();
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