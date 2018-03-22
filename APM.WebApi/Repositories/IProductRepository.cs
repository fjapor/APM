using APM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APM.WebApi.Repositories
{
    public interface IProductRepository
    {
        Product Create();
        List<Product> Retrieve();
        Product Save(Product product);
    }
}
