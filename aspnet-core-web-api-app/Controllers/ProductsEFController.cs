using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspnetcorewebapiapp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace aspnet_core_web_api_app.Controllers
{
    [Produces("application/json")]
    [Route("api/ProductsEF")]
    public class ProductsEFController : Controller
    {
        private readonly ProductContext _context;

        private IMemoryCache _cache;

        public ProductsEFController(ProductContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        // GET: api/ProductsEF
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            // return _context.Products;

            try
            {
                //Use IEnumerable here will fail to load in cache entry. throw net::ERR_CONNECTION_RESET in the F12 Console.
                IList<Product> cacheEntry;

                // Look for cache key.
                if (!_cache.TryGetValue("ProductKey", out cacheEntry))
                {
                    // Key not in cache, so get data.
                    cacheEntry = _context.Products.ToList();

                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                    // Save data in cache.
                    _cache.Set("ProductKey", cacheEntry, cacheEntryOptions);
                }




                return cacheEntry;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw;
            }

            


        }

        // GET: api/ProductsEF/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);


            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductsEF/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /*  
            {
              "name": "4K TV"
            }
            
        */
        // POST: api/ProductsEF
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return new StatusCodeResult(500);

            }

        }

        // DELETE: api/ProductsEF/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}