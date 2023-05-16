using AutoMapper;
using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Batch, BatchDto>();
            CreateMap<Job, JobDto>();
            CreateMap<Status, StatusDto>();
        }
    }
}
