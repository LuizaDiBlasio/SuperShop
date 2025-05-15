using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SuperShop.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder) //tornar o método do controlador de produtos genérico (recebe uma pasta qualquer)
        {
            string guid = Guid.NewGuid().ToString(); //gera uma chave aleatória que depois é passada para string para poder compor a variável file
            string file = $"{guid}.jpg"; //variável que será o nome do ficheiro para evitar nomes de ficheiros iguais no sistema

            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file); //criar o caminho da imagem: buscar diretorio de onde está agora, acrescentar o caminho dos pastas criadas e o nome do ficheiro

            using (FileStream stream = new FileStream(path, FileMode.Create)) //criar o ficheiro
            {
                await imageFile.CopyToAsync(stream); //busca a imagem e guarda no ficheiro criado
            }

           return $"~/images/{folder}/{file}"; // depois de buscar o caminho pelo diretório corrente e gravar, podemos atualizar o caminho 
                                               //para poder guardar na base de dados apenas o localizador da imagem (o URL)
        }
    }
}
