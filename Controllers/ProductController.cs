using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = new List<ProductDto>();
            foreach (var product in products)
            {
                productsDto.Add(_mapper.Map<ProductDto>(product));
            }
            return Ok(productsDto);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProduct(int id)
        {
            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound("El producto con el ID especificado no existe.");
            }
            // Mapeamos el objeto Product a ProductDto utilizando AutoMapper
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet("SearchProductsForCategory/{id:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsForCategory(int id)
        {
            var products = _productRepository.GetProductsForCategory(id);
            if (products.Count == 0)
            {
                return NotFound("Los productos con el ID de categoría especificado no existen.");
            }
            var productsDto = new List<ProductDto>();
            foreach (var product in products)
            {
                productsDto.Add(_mapper.Map<ProductDto>(product));
            }
            return Ok(productsDto);
        }

        [HttpGet("SearchProductsForNameDescription/{searchTerm}", Name = "GetProductsForNameDescription")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsForNameDescription(string searchTerm)
        {
            var products = _productRepository.SearchProducts(searchTerm);
            if (products.Count == 0)
            {
                return NotFound("No existen productos con el término de búsqueda especificado.");
            }
            var productsDto = new List<ProductDto>();
            foreach (var product in products)
            {
                productsDto.Add(_mapper.Map<ProductDto>(product));
            }
            return Ok(productsDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("Custom Error", "El producto ya existe.");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("Custom Error", "La categoría no existe.");
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(createProductDto);

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var createdProductDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new { id = product.ProductId }, createdProductDto);

        }

        [HttpPatch("BuyProduct/{id}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(int id, int quantity)
        {
            if (id <= 0 || quantity <= 0)
            {
                return BadRequest("El id o la cantidad ingresada no son válidos");
            }
            var foundProduct = _productRepository.ProductExists(id);
            if (!foundProduct)
            {
                return NotFound("El producto con el ID especificado no existe.");
            }
            if (!_productRepository.BuyProduct(id, quantity))
            {
                return BadRequest("No se pudo completar la compra del producto.");
            }
            return Ok("Producto comprado exitosamente.");
        }

        [HttpPut("{id:int}", Name="UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (!_productRepository.ProductExists(id))
            {
                ModelState.AddModelError("Custom Error", "El producto no existe.");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("Custom Error", "La categoría no existe.");
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = id;

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El ID del producto no es válido.");
            }

            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound("El producto no existe.");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
