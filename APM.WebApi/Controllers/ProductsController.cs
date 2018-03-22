using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.OData;
using APM.WebApi.Models;
using System.Web.Http.Description;
using System;
using APM.WebApi.Repositories;
using APM.WebApi.Services;
using APM.WebApi.MultiCulture;

namespace APM.WebApi.Controllers
{


    [EnableCors("http://localhost:7972", "*", "*")]
    [RoutePrefix("api/Product")]
    public class ProductsController : ApiController
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICurrencyConversionService CurrencyConversionService;
        public ProductsController(IProductRepository productRepository, ICurrencyConversionService currencyConversionService)
        {
            ProductRepository = productRepository;
            CurrencyConversionService = currencyConversionService;
        }

        // GET: api/Products
        [EnableQuery()]
        [ResponseType(typeof(Product))]
        public IHttpActionResult Get()
        {
            try
            {
                var products = ProductRepository.Retrieve().AsQueryable();
                products.ToList().ForEach(p => CurrencyConversionService.ConvertProductCurrency(p)); 

                return Ok(products);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [ResponseType(typeof(Product))]
        // GET: api/Products/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                Product product;

                if (id > 0)
                {
                    var products = ProductRepository.Retrieve();
                    product = products.FirstOrDefault(p => p.ProductId == id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    CurrencyConversionService.ConvertProductCurrency(product);
                    return Ok(product);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        // POST: api/Products
        public IHttpActionResult Post([FromBody]Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newProduct = ProductRepository.Save(product);

                if (newProduct == null)
                {
                    return Conflict();
                }

                return Created<Product>(Request.RequestUri + newProduct.ProductId.ToString(), newProduct);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        // PUT: api/Products/5
        public IHttpActionResult Put(int id, [FromBody]Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var updatedProduct = ProductRepository.Save(product);

                if (updatedProduct == null)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        // DELETE: api/Products/5
        public void Delete(int id)
        {
        }
    }
}
