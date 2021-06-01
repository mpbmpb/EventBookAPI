using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBookAPI.Domain;

namespace EventBookAPI.Services
{
    public interface IPageElementService
    {
        Task<List<PageElement>> GetPageElementsAsync();

        Task<PageElement> GetPageElementByIdAsync(Guid Id);

        Task<bool> CreatePageElementAsync(PageElement pageElement);

        Task<bool> UpdatePageElementAsync(PageElement pageElementToUpdate);

        Task<bool> DeletePageElementAsync(PageElement pageElement);

        bool UserOwnsPageElement(PageElement pageElement, string userId);
    }
}