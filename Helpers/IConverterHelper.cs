using SuperShop.Data.Entities;
using SuperShop.Models;
using System;

namespace SuperShop.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel model, bool isNew, Guid imageId); //bool necessário para saber se o id é inserido automaticamente na tabela (novo produto)
                                                                             //ou mantido (produto editado)

        ProductViewModel ToProductViewModel(Product product);
    }
}
