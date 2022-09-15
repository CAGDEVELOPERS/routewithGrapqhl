using GraphQL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.Core.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetById(string id);
    }
}
