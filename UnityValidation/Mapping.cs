using AutoMapper;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityValidation
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.Initialize(
                cfg => {
                    cfg
                    .CreateMap<ValidationResult, ServiceResponse>()
                    .ForMember(
                        d => d.Code,
                        opt => opt.MapFrom(
                            s => s.IsValid ?
                            ResponseCode.Ok : ResponseCode.Error))
                    .ForMember(
                        d => d.Description,
                        opt => opt.MapFrom(s => s.ToString()));
                });
        }
    }
}
