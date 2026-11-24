using SistemaStock.Models.Entities;
using SistemaStock.Models.ViewModels.Movimiento;
using AutoMapper;

namespace SistemaStock.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movimiento, MovimientoViewModel>();
            CreateMap<MovimientoViewModel, Movimiento>();
        }
    }

}


