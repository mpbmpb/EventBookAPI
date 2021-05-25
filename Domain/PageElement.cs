using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookAPI.Domain
{
    public class PageElement
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public string Classname { get; set; }
    }
}