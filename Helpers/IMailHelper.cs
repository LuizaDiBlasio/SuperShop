﻿namespace SuperShop.Helpers
{
    public interface IMailHelper
    {
       Response SendEmail(string email, string subject, string body);   
    }
}
