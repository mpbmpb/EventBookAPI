using System;
using System.Collections.Generic;
using System.Linq;
using EventBookAPI.Domain;

namespace EventBookAPI.Services
{
    public class PageElementService : IPageElementService
    {
        private List<PageElement> _pageElements;

        public PageElementService()
        {
            _pageElements ??= new ();
            for (var i = 1; i < 6; i++)
            {
                _pageElements.Add(new PageElement()
                {
                    Id = Guid.NewGuid(), 
                    Content = $"some text...{i}", 
                    Classname = "text"
                });
            }
        }
        
        
        public List<PageElement> GetPageElements()
        {
            return _pageElements;
        }

        public PageElement GetPageElement(Guid Id)
        {
            return _pageElements.SingleOrDefault(p => p.Id == Id);
        }

        public void AddPageElement(PageElement pageElement)
        {
            _pageElements.Add(pageElement);
        }
    }
}