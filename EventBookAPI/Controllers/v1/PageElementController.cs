using System;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Domain;
using EventBookAPI.Extensions;
using EventBookAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookAPI.Controllers.v1
{
    public class PageElementController : Controller
    {
        private readonly IPageElementService _pageElementService;

        public PageElementController(IPageElementService pageElementService)
        {
            _pageElementService = pageElementService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.PageElements.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePageElementRequest pageElementRequest)
        {
            var pageElement = new PageElement
            {
                Content = pageElementRequest.Content,
                Classname = pageElementRequest.Classname,
                UserId = HttpContext.GetUserId()
            };

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

        [HttpGet(ApiRoutes.PageElements.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _pageElementService.GetPageElementsAsync());
        }

        [HttpGet(ApiRoutes.PageElements.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid pageElementId)
        {
            var pageElement = await _pageElementService.GetPageElementByIdAsync(pageElementId);

            if (pageElement is null)
                return NotFound();

            return Ok(pageElement);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(ApiRoutes.PageElements.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid pageElementId,
            [FromBody] UpdatePageElementRequest request)
        {
            var pageElement = await _pageElementService.GetPageElementByIdAsync(pageElementId);
            var userOwnsPageElement =
                _pageElementService.UserOwnsPageElement(pageElement, HttpContext.GetUserId());

            if (userOwnsPageElement is false)
                return NotFound();

            pageElement.Content = request.Content;
            pageElement.Classname = request.Classname;

            var updated = await _pageElementService.UpdatePageElementAsync(pageElement);

            if (updated)
                return Ok(pageElement);

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete(ApiRoutes.PageElements.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid pageElementId)
        {
            var pageElement = await _pageElementService.GetPageElementByIdAsync(pageElementId);
            var userOwnsPageElement =
                _pageElementService.UserOwnsPageElement(pageElement, HttpContext.GetUserId());

            if (userOwnsPageElement is false)
                return NotFound();

            var deleted = await _pageElementService.DeletePageElementAsync(pageElement);

            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}