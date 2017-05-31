using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using aspnetcorewebapiapp.Models;

namespace aspnet_core_web_api_app.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {

        private static List<Product> _products = new List<Product>(new[] {
        new Product() { Id = 1, Name = "Green Peppers" },
        new Product() { Id = 2, Name = "Tacos" },
        new Product() { Id = 3, Name = "Chipotle Sauce" },
    });



        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _products;
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var product = _products.SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        /*  
            {
              "id": 4,
              "name": "4K TV"
            }
            
        */
        // POST: api/Products
        [HttpPost]
        public IActionResult Post([FromBody]Product product)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            _products.Add(product);

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);


        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
