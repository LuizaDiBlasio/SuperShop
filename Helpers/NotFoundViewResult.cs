using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SuperShop.Helpers
{
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName) //classe que recebe no construtor nome da view
        {
            ViewName = viewName;    
            StatusCode = (int)HttpStatusCode.NotFound; //código de erro not found
        }
    }
}
