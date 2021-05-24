using System;
using System.Collections.Generic;
using System.Linq;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Domain;
using EventBookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventBookAPI.Controllers.v1
{
    public class PageElementController : Controller
    {
        private IPageElementService _pageElementService;

        public PageElementController(IPageElementService pageElementService)
        {
            _pageElementService = pageElementService;
        }

        [HttpGet(ApiRoutes.PageElements.GetAll)]
        public IActionResult GetAll()
        {
            var pageElements = _pageElementService.GetPageElements();
            
            return Ok(pageElements);
        }

        [HttpPost(ApiRoutes.PageElements.Create)]
        public IActionResult Create([FromBody] CreatePageElementRequest pageElementRequest)
        {
            var pageElement = new PageElement {Id = pageElementRequest.Id, Content = pageElementRequest.Content, Classname = pageElementRequest.Classname};
            
            if (pageElement.Id != Guid.Empty)
                pageElement.Id = Guid.NewGuid();
            
            _pageElementService.AddPageElement(pageElement);

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
        public IActionResult Get([FromRoute] Guid pageElementId)
        {
            var pageElement = _pageElementService.GetPageElement(pageElementId);

            if (pageElement is null)
                return NotFound();
            
            return Ok(pageElement);
        }
    }
}