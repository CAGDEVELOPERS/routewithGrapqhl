using GraphQL.Core.Entities;
using HotChocolate;
using HotChocolate.Types;

namespace API.Subscriptions
{
    public class Subscription
    {
        [Subscribe]
        public Product ProductChanged([EventMessage] Product product) => product;
    }
}
