using System;

namespace EventBookAPI.Contracts.v1.Responses;

public class PageElementResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; }

    public string Classname { get; set; }
}