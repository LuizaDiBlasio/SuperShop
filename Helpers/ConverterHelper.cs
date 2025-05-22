using System;
using System.IO;
using SuperShop.Data.Entities;
using SuperShop.Models;

namespace SuperShop.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Product ToProduct(ProductViewModel model, bool isNew, Guid imageId)
        {
            return new Product //retornar o produto criado
            {
                Id = isNew ? 0 : model.Id,  //se o produto for novo, colocar id = 0 para a BD preencher aumotaticamente, se não, manter o Id antigo
                ImageId = imageId,
                Name = model.Name,
                IsAvailable = model.IsAvailable,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                Price = model.Price,
                Stock = model.Stock,
                User = model.User
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                ImageId = product.ImageId,
                Name = product.Name,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }
    }
}
