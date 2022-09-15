using GraphQL.Core.Entities;
using GraphQL.Core.Repositories;
using HotChocolate;
using HotChocolate.Subscriptions;
using System.Threading.Tasks;

namespace API.Mutation
{
    public class Mutation
    {
        public async Task<Product> AddProduct(Product product, [Service] IProductRepository productRepository) => await productRepository.InsertProduct(product);

    }
}
