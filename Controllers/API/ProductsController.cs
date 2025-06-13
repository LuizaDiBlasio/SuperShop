using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;

namespace SuperShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]

    //única maneira de Autenticar a entrada numa api é por token
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //bloqueia a API caso não tenha o token
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_productRepository.GetAllWithUsers()); //busca todos os produtos via repositório e coloca a resposta dentro de um Json que retorna status OK
        }


    }
}
