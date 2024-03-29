﻿using AutoMapper;
using DevExpress.Xpo;
using ExampleAPI.Contracts.V1;
using ExampleAPI.Models.ExampleXPOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Mappers
{
    public class V1Mappers : Profile
    {

        public V1Mappers()
        {
            CreateMap<ExampleObject, ExampleObjectResponse>();
            CreateMap<ExampleObjectCreate, ExampleObject>().ForMember(dest => dest.Oid, opt => opt.Ignore()).DisableCtorValidation();
            CreateMap<ExampleObjectUpdate, ExampleObject>().ForMember(dest => dest.Oid, opt => opt.Ignore()).DisableCtorValidation().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
