﻿using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    public interface IBlobHelper
    {
        //Faz upload da imagem pelo computador, parametros: ficheiro da web e container onde vai ser guardada a imagem, retorna um guid identificador do blob
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);


        //Faz upload da imagem pelo celular, parametros: array de bytes e container onde vai ser guardada a imagem, retorna um guid identificador do blob
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);


        //Faz upload da imagem, parametros: caminho da imagem e container onde vai ser guardada a imagem, retorna um guid identificador do blob
        Task<Guid> UploadBlobAsync(string image, string containerName);
    }
}
