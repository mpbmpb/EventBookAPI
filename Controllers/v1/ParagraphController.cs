using System;
using System.Collections.Generic;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EventBookAPI.Controllers.v1
{
    public class ParagraphController : Controller
    {
        private List<Paragraph> _paragraphs;

        public ParagraphController()
        {
            _paragraphs ??= new ();
            for (var i = 1; i < 6; i++)
            {
                _paragraphs.Add(new Paragraph() {Id = $"{i}-{Guid.NewGuid().ToString()}", Content = "some text..."});
            }
        }
        
        [HttpGet(ApiRoutes.Paragraphs.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_paragraphs);
        }

        [HttpPost(ApiRoutes.Paragraphs.Create)]
        public IActionResult Create([FromBody] CreateParagraphRequest paragraphRequest)
        {
            var paragraph = new Paragraph {Id = paragraphRequest.Id, Content = paragraphRequest.Content};
            
            if (string.IsNullOrEmpty(paragraph.Id))
                paragraph.Id = Guid.NewGuid().ToString();
            
            _paragraphs.Add(paragraph);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";

            var locationUri = baseUrl + "/" + 
                        ApiRoutes.Paragraphs.Get.Replace("{paragraphId}", paragraph.Id);

            var response = new ParagraphResponse {Id = paragraph.Id, Content = paragraph.Content};

            return Created(locationUri, response);
        }
        
        // [HttpGet(ApiRoutes.Paragraphs.Get)]
        // public IActionResult Get([FromRoute] )
    }
}