using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperShop.Helpers
{
    public interface IImageHelper
    {
        //metodo que devolve string com o caminho
        Task<string> UploadImageAsync(IFormFile imageFile, string folder); //recebe o ficheiro da imagem e o caminho da pasta onde está contida

    }
}
