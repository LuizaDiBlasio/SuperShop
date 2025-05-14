using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        private readonly IUserHelper _userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
           _productRepository = productRepository;
            _userHelper = userHelper;   
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p => p.Name)); // vai à interface IRepository buscar a lista de produtos e manda por paramatro para a view do index
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) // id aqui é nullable
        {
            if (id == null) //sempre checar se produto ainda existe para evitar erro, no meio tempo pode ter sido apagado
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // pelo fato de ser nullable, tenho que passar o id pelo valor, o programa não irá rebentar mesmo que o valor seja nulo
            if (product == null)
            {
                return NotFound();
            }

            return View(product); //manda para a view o produto encontrado 
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View(); 
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model) // recebe o produto (modelo da ProductViewModel) criado na View Create
        {
            if (ModelState.IsValid) //checa se o produto é válido
            {
                var path = string.Empty; // caminho da imagem

                if(model.ImageFile != null && model.ImageFile.Length > 0) //verificar se existe a imagem
                {
                    var guid = Guid.NewGuid().ToString(); //gera uma chave aleatória que depois é passada para string para poder compor a variável file
                    var file = $"{guid}.jpg"; //variável que será o nome do ficheiro para evitar nomes de ficheiros iguais no sistema

                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file); //criar o caminho da imagem: buscar diretorio de onde está agora, acrescentar o caminho dos pastas criadas e o nome do ficheiro

                    using (var stream = new FileStream(path, FileMode.Create)) //criar o ficheiro
                    {
                        await model.ImageFile.CopyToAsync(stream); //busca a imagem e guarda no ficheiro criado
                    }

                    path = $"~/images/products/{file}"; // depois de buscar o caminho pelo diretório corrente e gravar, podemos atualizar o caminho 
                                                                                     //para poder guardar na base de dados apenas o localizador da imagem (o URL)
                }

                //mesmo usando o ProductViewModel, o que será criado e enviado para a base derá o product, então precisa converter o model para product por meio do método ToProduct()
                var product = this.ToProduct(model, path);

                //TODO: Modificar para o user que tiver logado
                product.User = await _userHelper.GetUserByEmailAsync("Luizabandeira90@gmail.com"); //buscar user para colocar na propriedade do produto ao ser criado

                await _productRepository.CreateAsync(product); //adiciona produto em memória pelo repositório

                return RedirectToAction(nameof(Index)); //redireciona para a lista de produtos
            }
            return View(model); // caso não for válido, mostra a mesma view do Create só que com os dados do produto para poderem ser alterados
        }

        private Product ToProduct(ProductViewModel model, string path)
        {
            return new Product //retornar o produto criado
            {
                Id = model.Id,
                ImageURL = path,
                Name = model.Name,
                IsAvailable = model.IsAvailable,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                Price = model.Price,
                Stock = model.Stock,
                User = model.User
            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) //id nullable para se caso não haver id, o programa não irá arrebentar
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); //busca produto pelo repositorio
            if (product == null)
            {
                return NotFound();
            }

            var model = this.ToProductViewModel(product);

            return View(model); //mostra a view do Edit com os dados do produto passado por parametro
        }

        private ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                ImageURL = product.ImageURL,
                Name = product.Name,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model) //recebe o model 
        {

            if (ModelState.IsValid) // validação do modelo OBS: ModelState é um objeto interno do ASP.NET Core MVC que armazena o estado de validação de um modelo
            {
                try //Salva modificações dentro do try catch
                {
                    var path = model.ImageURL;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString(); 
                        var file = $"{guid}.jpg"; 

                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file );

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/products/{file}";
                    }

                    var product = this.ToProduct(model, path);

                    //TODO: Modificar para o user que tiver logado
                    product.User = await _userHelper.GetUserByEmailAsync("Luizabandeira90@gmail.com"); //buscar user para colocar na propriedade do produto ao ser criado

                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException) // caso algo nao corra bem
                {
                    if (! await _productRepository.ExistAsync(model.Id)) //caso o produto não exista, exibir página de notfound
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // se tudo correr bem, redireciona para o index com o produto já editado
            }
            return View(model); //caso o modelo não seja válido, redireciona para a view do Edit com os dados do produto para serem editados novamente
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) // Essa action encaminha o utilizador para a view do Delete para pode deletar o produto
        {
            if (id == null) // caso id não exista, retorna NotFound
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // ir buscar produto 
             

            if (product == null) // caso produto seja nulo, retornar NotFound
            {
                return NotFound();
            }

            return View(product); // enviar produto para a view para ser deletado
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")] //Reencaminhamento, nomeia a ação POST (do Delete) como Delete, para poder ser reconhecida na view quando fizer um submit
        [ValidateAntiForgeryToken]

        //esta action deleta o produto de fato
        public async Task<IActionResult> DeleteConfirmed(int id) // id orbigatório para poder apagar 
        {
            var product = await _productRepository.GetByIdAsync(id); // buscar produto pelo repositorio

            await _productRepository.DeleteAsync(product); //remove produto em memoria pelo repositorio

            return RedirectToAction(nameof(Index)); // volta para a lista de produtos no Index, sem o produto
        }

        //OBS: Não existe PUT, CREATE e DELETE no HTML, esses 3 métodos do HTTP foram substituídos pelo POST, que irá fazer todas as alterações necessárias à base de dados
        // O Html não suporta esses outros 3 métodos,também existe a limitação do navegador por motivo de segurança (ValidateAntiForgeryToken) no caso do DELETE

        //OBS 2: O método GET é um métod generalista que serve tanto para ir buscar dados na DB como para encaminhar o utilizador para uma View
    }
}
