using AutoMapper;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Domain;

namespace EventBookAPI.Mapping;

public class DomainToResponseProfile : Profile
{
    public DomainToResponseProfile()
    {
        CreateMap<PageElement, PageElementResponse>();
        
    }
}