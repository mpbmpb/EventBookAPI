using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventBookAPI.Services
{
    public class PageElementService : IPageElementService
    {
        private readonly DataContext _dataContext;

        public PageElementService(DataContext dataContext)
        {
            _dataContext = dataContext;
            // _dataContext.PageElements ??= new ();
            // for (var i = 1; i < 6; i++)
            // {
            //     _dataContext.PageElements.Add(new PageElement()
            //     {
            //         Id = Guid.NewGuid(), 
            //         Content = $"some text...{i}", 
            //         Classname = "text"
            //     });
            // }
        }
        
        
        public async Task<List<PageElement>> GetPageElementsAsync()
        {
            return await _dataContext.PageElements.ToListAsync();
        }

        public async Task<PageElement> GetPageElementByIdAsync(Guid Id)
        {
            return await _dataContext.PageElements.SingleOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<bool> CreatePageElementAsync(PageElement pageElement)
        {
            await _dataContext.PageElements.AddAsync(pageElement);
            
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> UpdatePageElementAsync(PageElement pageElementToUpdate)
        {
            _dataContext.PageElements.Update(pageElementToUpdate);
            
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0 ;
        }

        public async Task<bool> DeletePageElementAsync(Guid pageElementId)
        {
            var pageElement = await GetPageElementByIdAsync(pageElementId);

            if (pageElement is null)
                return false;
            
            _dataContext.PageElements.Remove(pageElement);
            
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
    }
}