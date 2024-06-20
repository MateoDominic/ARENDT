using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.AutoMapper;

namespace PraTest
{
    internal static class MappingProfileProvider
    {
        public static MapperConfiguration InitializeAutoMapper() { 
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });

            return config;
        }
    }
}
