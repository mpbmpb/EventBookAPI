using System;

namespace EventBookAPI.Contracts.v1.Requests
{
    public class CreatePageElementRequest
    {
        public Guid Id { get; set; }
        public string Content { get; set; }

        public string Classname { get; set; }
    }
}