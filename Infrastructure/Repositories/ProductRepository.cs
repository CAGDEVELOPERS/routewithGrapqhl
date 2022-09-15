using GraphQL.Core.Entities;
using GraphQL.Core.Repositories;
using HotChocolate;
using HotChocolate.Subscriptions;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext catalogContext;
        private readonly ITopicEventSender sender;
        public ProductRepository(ICatalogContext catalogContext, [Service] ITopicEventSender _sender)
        {
            this.sender = _sender;
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await this.catalogContext.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var filter = Builders<Product>.Filter.Eq(_ => _.Id, id);

            return await this.catalogContext.Products.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Product> InsertProduct(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(_ => _.Id, product.Id);
            var update = Builders<Product>.Update.ApplyMultiFields(product);
            var result = await catalogContext.Products.UpdateOneAsync(filter, update);
            await sender.SendAsync("ProductChanged", await GetByIdAsync(product.Id));
            return await GetByIdAsync(product.Id);

        }

    }
    public static class Extensions
    {
        public static UpdateDefinition<T> ApplyMultiFields<T>(this UpdateDefinitionBuilder<T> builder, T obj)
        {
            var properties = obj.GetType().GetProperties();
            List<UpdateDefinition<T>> definition = new List<UpdateDefinition<T>>();
            var updateBuilder = Builders<T>.Update;
            foreach (var property in properties)
            {
                if (property.GetValue(obj) != null && property.Name != "Id")
                {
                    definition.Add(builder.Set(property.Name, property.GetValue(obj)));
                }
            }

            return updateBuilder.Combine(definition);
        }
    }
}
