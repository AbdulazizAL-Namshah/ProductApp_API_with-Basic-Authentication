using ProductApp_API.Authorization;
using ProductApp_API.Data;
using ProductApp_API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class ProductsController:ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext dbContext, ILogger<ProductsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [HttpGet]
        [Route("")]
        [Authorize(Policy = "AgeGreaterThan25")]
        public ActionResult<IEnumerable<Product>> Get()
        {
            
            var results=_dbContext.Set<Product>().ToList();
            return Ok(results);
        }
        [HttpGet]
        [Route("{Id}")]
        //[Authorize(Roles="Admin")]
        [LogSensitiveAction]

        public ActionResult<Product> GetById([FromRoute(Name ="key")] int id)
        {
            _logger.LogDebug("Getting product #", id);
            var results = _dbContext.Set<Product>().Find(id);
            if (results == null)
                _logger.LogWarning("Product #{id} was not found", id);
            return results==null? NotFound(): Ok(results);
        }
        [HttpPost]
        [Route("")]
        public ActionResult<int> createProduct([FromQuery()] Product product,[FromQuery(Name="P2")]Product product2)
        {
            product.Id = 0;
            _dbContext.Set<Product>().Add(product);
            _dbContext.SaveChanges();
            return Ok(product.Id);

        }
        [HttpPut]
        [Route("")]
        public ActionResult UpdateRecord(Product product) 
        {
            var result=_dbContext.Set<Product>().Find(product.Id);
            result.Name = product.Name;
            result.Sku = product.Sku;
            _dbContext.Set<Product>().Update(result);
            _dbContext.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteRecord(int id) 
        {
            var result= _dbContext.Set<Product>().Find(id);
            _dbContext.Set<Product>().Remove(result);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
