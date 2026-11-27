using Microsoft.AspNetCore.Mvc;
using InventoryRD.Application.Contract;

namespace InventoryRD.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IStockMovementService _stockMovementService;

        public ReportsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IStockMovementService stockMovementService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _stockMovementService = stockMovementService;
        }

        [HttpGet]
        [Route("products")]
        public IActionResult GetProductsReport()
        {
            var result = _productService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("stock")]
        public IActionResult GetStockReport(bool lowStockOnly = false)
        {
            var result = lowStockOnly ? _productService.GetLowStock() : _productService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("categories")]
        public IActionResult GetCategoriesReport()
        {
            var result = _categoryService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("suppliers")]
        public IActionResult GetSuppliersReport()
        {
            var result = _supplierService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("movements")]
        public IActionResult GetMovementsReport(int? productId = null)
        {
            var result = _stockMovementService.GetAll(productId);

            return Ok(result.Data);
        }
    }
}
