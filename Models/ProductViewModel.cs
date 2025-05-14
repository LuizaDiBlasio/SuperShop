using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SuperShop.Data.Entities;

namespace SuperShop.Models
{
    public class ProductViewModel : Product //este Model é criado para poder fazer upload de ficheiros, herda de Products pois ainda há necessidade de ter todos as propriedades
                                            //desta entidade. ProductViewModel irá substituir Products nas Views, para que se consiga fazer upload de imagens
                                            //esta distinção é feita para separar o que vai para base de dados e não, neste cso as imagens não irão para a BD, portanto 
                                            //cria-se um ViewModel à parte
    {
        [Display(Name ="Image")]
        public IFormFile ImageFile { get; set; }
    }
}
