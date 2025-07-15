using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Models;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace SuperShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        private readonly IFlashMessage _flashMessage;

        public CountriesController(ICountryRepository countryRepository, IFlashMessage flashMessage)
        {
            _countryRepository = countryRepository;
            _flashMessage = flashMessage;   
        }

        public IActionResult Index()
        {
            return View(_countryRepository.GetCountriesWithCities()); //lista de todos os países com todas as cidades 
        }

        public async Task<IActionResult> DeleteCity(int? id) //id da cidade
        {
            if (id == null) //Verificar id
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value); //buscar a cidade
            
            if(city == null) // se não encontrada
            {
                return NotFound();  
            }

            var countryId = await _countryRepository.DeleteCityAsync(city); //buscar o countryId através do metodo de deletar a cidade

            //forma correta de direcionar para a View possui o nome da Action do Controller como parametro e um objeto, com values. Nesse caso é um objeto anonimo, só interessa o value que contém o countryId
            return this.RedirectToAction("Details", new {id = countryId}); //usar countryID para redirecionar para a View
        }


        //GET do Edit, para ir para a View
        public async Task<IActionResult> EditCity(int? id)
        {
            if(id == null) //Verificar id
            {
                return NotFound();  
            }

            var city = await _countryRepository.GetCityAsync(id.Value); //buscar city

            if(city == null) // se não encontrada
            {
                return NotFound();  
            }        

            return View(city); //mandar city encontrada para a View
        }


        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (this.ModelState.IsValid)//confere se modelo é válido
            {
                var countryId = await _countryRepository.UpdateCityAsync(city); //faz update da city

                if(countryId != 0) //caso tenha corrido bem 
                {
                    return this.RedirectToAction("Details", new { id = countryId }); //voltar para a View do country (forma correta de passar, action do controller e objeto com values)
                }
            }

            return this.View(city); //caso corra mal, voltar para a View do Edit com o model
        }

        //GET do AddCity
        public async Task<IActionResult> AddCity(int? id) //vai para a view para poder add city
        {
            if(id == null) //Verificar id
            {
                return NotFound();  
            }

            var country = await _countryRepository.GetByIdAsync(id.Value); //buscar country
            if(country == null)
            {
                return NotFound();  
            }

            //caso encontrado, prosseguir com adição
            var model = new CityViewModel { CountryId = country.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model) //adiciona de fato a city 
        {
            if (this.ModelState.IsValid) //verifica model
            {
                await _countryRepository.AddCityAsync(model); //add city
                return RedirectToAction("Details", new { id = model.CountryId }); //usa o Id do country para ir para a View do Details do country da city
            }

            return View(model); 
        } 

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) //verificar id 
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value); //buscar cidades para compor a lista que será mostrada na View

            if(country == null) //se não encontrado
            {
                return NotFound();  
            }

            return View(country); //retorna para a View dos datalhes do país com a lista de cidades
        }

        //GET do Create do Country
        public IActionResult Create()
        {
            return View(); //ir para a view Create para criar um novo país
        }


        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Create(Country country)//cria país de fato
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.CreateAsync(country); //criar

                    return RedirectToAction(nameof(Index));  //voltar para a Index
                }
                catch (Exception)
                {
                    _flashMessage.Danger("This country already exists");
                }

                return View(country); //permanecer na View
            }

            return View(country);   //retornar View com country caso não dê certo a criação
        }

        //GET do Edit Country
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) //verificar id
            {
                return NotFound();
            }
            var country = await _countryRepository.GetByIdAsync(id.Value); //buscar country

            if (country == null)//se não encontrado
            {
                return NotFound();
            }
            
            //caso encontrado, retonar para a view do Edit
            return View(country);
        }

        //Post do Edit Country
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid) //se model é válido
            {
                await _countryRepository.UpdateAsync(country); //fazer update
                return RedirectToAction(nameof(Index)); //redirecionar para view da lista da países
            }
            return View(country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) //verificar id
            {
                return NotFound();
            }
            var country = await _countryRepository.GetByIdAsync(id.Value);// buscar country

            if (country == null) //se não encontrado
            {
                return NotFound();
            }
            await _countryRepository.DeleteAsync(country); //deletar
            return RedirectToAction(nameof(Index)); //redirecionar para view da lista da países
        }
    }
}
