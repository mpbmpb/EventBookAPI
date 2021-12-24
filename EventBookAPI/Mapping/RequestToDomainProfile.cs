using AutoMapper;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Domain;

namespace EventBookAPI.Mapping;

public class RequestToDomainProfile : Profile
{
    public RequestToDomainProfile()
    {
        CreateMap<CreatePageElementRequest, PageElement>();
        CreateMap<UpdatePageElementRequest, PageElement>();
    }
}