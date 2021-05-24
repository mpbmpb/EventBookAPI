using System;

namespace EventBookAPI.Domain
{
    public class PageElement
    {
        public Guid Id { get; set; }
        public string Content { get; set; }

        public string Classname { get; set; }
    }
}