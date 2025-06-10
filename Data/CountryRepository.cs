using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext _context;
        public CountryRepository(DataContext context) : base(context)
        {
            _context = context; 
        }

        public async Task AddCityAsync(CityViewModel model)
        {
            var country = await this.GetCountryWithCitiesAsync(model.CountryId); //busca o country pelo CountryId do model

            if (country == null)
            {
                return; //interrompe se não houver country
            }

            country.Cities.Add(new City { Name = model.Name }); //adicionar cidade ao país em memória
            _context.Countries.Update(country); //fazer update
            await _context.SaveChangesAsync();  //Salvar 
        }

        public async Task<int> DeleteCityAsync(City city) //apagar cidade de um determinado país
        {
            var country = await _context.Countries
                        .Where(c => c.Cities.Any(ci => ci.Id == city.Id)) //dentro de todas as cidades de todos os países da DB, buscar a cidade com id específico
                        .FirstOrDefaultAsync();  // Encontra o país que contém a cidade com o ID específico

            if (country == null)
            {
                return 0; //retorna "nulo"  
            }

            _context.Cities.Remove(city); //remover 
            await _context.SaveChangesAsync();  //salvar
            return country.Id;  //retorna id do país para poder retomar o país
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id); //bypass
        }

        public IQueryable GetCountriesWithCities() //retorna uma lista de países com suas cidades
        {
            return _context.Countries
                    .Include(c => c.Cities) //buscar cidades
                    .OrderBy(c => c.Name);  // ordenar por nome do país 
        }

        public async Task<Country> GetCountryWithCitiesAsync(int id) // retorna o país com suas cidades
        {
            return await _context.Countries
                        .Include(c => c.Cities) //buscar cidades
                        .Where(c => c.Id == id) 
                        .FirstOrDefaultAsync(); //para somente um país
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            //busca um country que contenha a cidade para ver se a cidade existe na BD
            var country = await _context.Countries
                         .Where (c => c.Cities.Any(ci => ci.Id == city.Id))
                         .FirstOrDefaultAsync(); 

            //caso não exista
            if (country == null)
            {
                return 0;
            }

            //caso exista
            _context.Cities.Update(city); //fazer update 
            await _context.SaveChangesAsync();  //salvar
            return country.Id;  //retornar id do país para poder voltar ao país
        }

        public async Task<Country> GetCountryAsync(City city)
        {
            //buscar país pelas cidades
            return await _context.Countries
                        .Where(c => c.Cities.Any(c => c.Id == city.Id)) 
                        .FirstOrDefaultAsync();
        }
    }
}
