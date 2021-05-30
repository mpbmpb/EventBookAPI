using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EventBookAPI.Domain
{
    public class PageElement
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public string Classname { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}