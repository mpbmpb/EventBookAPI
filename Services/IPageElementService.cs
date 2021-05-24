using System;
using System.Collections.Generic;
using EventBookAPI.Domain;

namespace EventBookAPI.Services
{
    public interface IPageElementService
    {
        List<PageElement> GetPageElements();

        PageElement GetPageElement(Guid Id);

        void AddPageElement(PageElement pageElement);
    }
}