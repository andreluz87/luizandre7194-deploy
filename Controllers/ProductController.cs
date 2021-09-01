using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backoffice.Controllers
{
    [Route("products")]
    public class ProductController : Controller
    {
        [HttpGet]
        [Route("")]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            //Include: Join no select, onde: JOIN Product.CategoryId = Category.Id
            var products = await context.Products.Include(prod => prod.Category).AsNoTracking().ToListAsync();
            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        //[AllowAnonymous]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {
            var product = await context.Products.Include(prod => prod.Category).AsNoTracking().FirstOrDefaultAsync(prod => prod.Id == id);
            return product;
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var products = await context.Products.Include(prod => prod.Category).AsNoTracking().Where(prod => prod.CategoryId == id).ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        //[Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody]Product model)
        {
            if (ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}