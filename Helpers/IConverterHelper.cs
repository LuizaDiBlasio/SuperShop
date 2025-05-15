using SuperShop.Data.Entities;
using SuperShop.Models;

namespace SuperShop.Helpers
{
    public interface IConverterHelper
    {
        Product ToProduct(ProductViewModel model, bool isNew, string path); //bool necessário para saber se o id é inserido automaticamente na tabela (novo produto)
                                                               //ou mantido (produto editado)

        ProductViewModel ToProductViewModel(Product product);
    }
}
