using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
           _productRepository = productRepository;   
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll()); // vai à interface IRepository buscar a lista de produtos e manda por paramatro para a view do index
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
        public async Task<IActionResult> Create(Product product) // recebe o produto criado na View Create
        {
            if (ModelState.IsValid) //checa se o produto é válido
            {
                await _productRepository.CreateAsync(product); //adiciona produto em memória pelo repositório

                return RedirectToAction(nameof(Index)); //redireciona para a lista de produtos
            }
            return View(product); // caso não for válido, mostra a mesma view do Create só que com os dados do produto para poderem ser alterados
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
            return View(product); //mostra a view do Edit com os dados do produto passado por parametro
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product) //recebe o id do produto alterado e o produto
        {
            if (id != product.Id) // checa se id existe
            {
                return NotFound();
            }

            if (ModelState.IsValid) // validação do modelo OBS: ModelState é um objeto interno do ASP.NET Core MVC que armazena o estado de validação de um modelo
            {
                try //Salva modificações dentro do try catch
                {
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException) // caso algo nao corra bem
                {
                    if (! await _productRepository.ExistAsync(product.Id)) //caso o produto não exista, exibir página de notfound
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
            return View(product); //caso o modelo não seja válido, redireciona para a view do Edit com os dados do produto para serem editados novamente
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
