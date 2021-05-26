using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Domain;
using EventBookAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookAPI.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PageElementController : Controller
    {
        private IPageElementService _pageElementService;

        public PageElementController(IPageElementService pageElementService)
        {
            _pageElementService = pageElementService;
        }

        [HttpGet(ApiRoutes.PageElements.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _pageElementService.GetPageElementsAsync());
        }

        [HttpPost(ApiRoutes.PageElements.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePageElementRequest pageElementRequest)
        {
            var pageElement = new PageElement {
                Content = pageElementRequest.Content, 
                Classname = pageElementRequest.Classname};

            await _pageElementService.CreatePageElementAsync(pageElement);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + 
                              ApiRoutes.PageElements.Get.Replace("{pageElementId}", pageElement.Id.ToString());

            var response = new PageElementResponse
            {
                Id = pageElement.Id, 
                Content = pageElement.Content, 
                Classname = pageElement.Classname
            };

            return Created(locationUri, response);
        }

        [HttpGet(ApiRoutes.PageElements.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid pageElementId)
        {
            var pageElement = await _pageElementService.GetPageElementByIdAsync(pageElementId);

            if (pageElement is null)
                return NotFound();
            
            return Ok(pageElement);
        }

        [HttpPut(ApiRoutes.PageElements.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid pageElementId, [FromBody] UpdatePageElementRequest pageElementRequest)
        {
            var pageElement = await _pageElementService.GetPageElementByIdAsync(pageElementId);

            if (pageElement is null)
                return NotFound();
            
            pageElement.Content = pageElementRequest.Content;
            pageElement.Classname = pageElementRequest.Classname;

            var updated = await _pageElementService.UpdatePageElementAsync(pageElement);

            if (updated)
                return Ok(pageElement);
            
            return NotFound(); 
        }
        
        [HttpDelete(ApiRoutes.PageElements.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid pageElementId)
        {        
            var deleted = await _pageElementService.DeletePageElementAsync(pageElementId);
            
            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}