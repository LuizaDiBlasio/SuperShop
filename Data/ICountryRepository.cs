using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Data.Entities;
using SuperShop.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        IQueryable GetCountriesWithCities(); //método que devolve países com cidades

        Task<Country> GetCountryWithCitiesAsync(int id); // método que devolve país especificado por id com suas cidades

        Task<City> GetCityAsync(int id); // método que devolve o a cidade específica de um determinado id

        Task AddCityAsync(CityViewModel model); //Método que recebe o modelo e adiciona a cidade

        Task<int> UpdateCityAsync(City city); //Faz update da cidade e devolve o id deste objeto após atualização

        Task<int> DeleteCityAsync(City city);

        IEnumerable<SelectListItem> GetComboCountries(); //preenche combobox dos países

        IEnumerable<SelectListItem> GetComboCities(int countryId); //preenche combobox das cidades em função do país

        Task<Country> GetCountyAsync(City city); //retorna um país em função de uma cidade
    }
}
