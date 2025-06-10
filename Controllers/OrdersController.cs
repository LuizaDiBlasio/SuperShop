using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;
using SuperShop.Models;


namespace SuperShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository; 
        }
        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(this.User.Identity.Name); //buscar order do user 

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _orderRepository.GetDetailTempsAsync(this.User.Identity.Name); //buscar orders temporárias do user 

            return View(model);
        }

        public IActionResult AddProduct()
        {
            var model = new AddItemViewModel
            {
                Quantity = 1, //default
                Products = _productRepository.GetComboProducts() //lista de produtos da combo
            };

            return View(model); //envia modelo com lista de produtos e qauntidade 1 por default
        }

        [HttpPost]
        public async Task <IActionResult> AddProduct(AddItemViewModel model)
        {
           if(ModelState.IsValid)
            {
                await _orderRepository.AddItemToOrderAsync(model, this.User.Identity.Name); //adicionar produto à order
                return RedirectToAction("Create"); //redirecionar para create
            }

            return View(model); //se correr mal vai para view com o modelo
        }

        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null) //buscar order
            {
                return NotFound();
            }

            await _orderRepository.DeleteDetailTempAsync(id.Value); //deleta a order

            return RedirectToAction("Create"); //volta pra view Create atualizada
        }

        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null) //buscar order
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1); //modifica quantidade do produto, o 1 representa o click do botão

            return RedirectToAction("Create"); //volta pra view Create atualizada
        }

        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null) //buscar order
            {
                return NotFound();
            }

            await _orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1); //modifica quantidade do produto, o -1 representa o click do botão

            return RedirectToAction("Create"); //volta pra view Create atualizada
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            var response = await _orderRepository.ConfirmOrderAsync(this.User.Identity.Name); //método confirma a ordeme devolve um true caso tuso corra bem

            if (response)
            {
                return RedirectToAction("Index"); //voltar para index com lista de orders se response = true
            }

            return RedirectToAction("Create"); //se response = false, continuar na view create com as orders ainda temporarias 
        }

        public async Task<IActionResult> Deliver(int? id) //mostra na view o model de DeliverViewModel (mostra a data de entrega)
        {
            if (id ==null)
            {
                return NotFound();
            }

            var order = await _orderRepository.GetOrderAsync(id.Value); 

            if (order == null)
            {
                return NotFound();
            }

            //se o id não for nulo e existir a order, criar o mel para mostrar a data de entrega
            var model = new DeliveryViewModel
            {
                Id = order.Id,
                DeliveryDate = DateTime.Today
            };

            return View(model); //retornar o model para a view

        }

        [HttpPost]
        public async Task<IActionResult> Deliver(DeliveryViewModel model) //faz o submit do Deliver, salva a data de entrega na DB
        {
            if (ModelState.IsValid)
            {
               await _orderRepository.DeliverOrder(model); //salva

                return RedirectToAction("Index");
            }


            return View(); //retornar para a view

        }
    }
}
