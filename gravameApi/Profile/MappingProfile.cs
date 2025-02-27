using gravameApi.src.Models;
using gravameApi.src.Models.DTOs;

namespace gravameApi.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Credor, credorDTO>()
                .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.nome))
                .ForMember(dest => dest.numDocumento, opt => opt.MapFrom(src => src.numDocumento))
                .ForMember(dest => dest.nomeEndereco, opt => opt.MapFrom(src => src.nomeEndereco))
                .ForMember(dest => dest.numEndereco, opt => opt.MapFrom(src => src.numEndereco))
                .ForMember(dest => dest.nomeBairroEndereco, opt => opt.MapFrom(src => src.nomeBairroEndereco))
                .ForMember(dest => dest.siglaUfEndereco, opt => opt.MapFrom(src => src.siglaUfEndereco))
                .ForMember(dest => dest.codMunicipioEndereco, opt => opt.MapFrom(src => src.codMunicipioEndereco))
                .ForMember(dest => dest.numCepEndereco, opt => opt.MapFrom(src => src.numCepEndereco))
                .ForMember(dest => dest.numDddTelefone, opt => opt.MapFrom(src => src.numDddTelefone))
                .ForMember(dest => dest.numTelefone, opt => opt.MapFrom(src => src.numTelefone));

            CreateMap<contratoDTO,Contrato>()
                  .ForMember(dest => dest.numContrato, opt => opt.MapFrom(src => src.numContrato))
                  .ForMember(dest => dest.dtContrato, opt => opt.MapFrom(src => src.dtContrato))
                  .ForMember(dest => dest.qteMesesVigenciaContrato, opt => opt.MapFrom(src => src.qteMesesVigenciaContrato))
                  .ForMember(dest => dest.indTipoDocumentoRecebedor, opt => opt.MapFrom(src => src.indTipoDocumentoRecebedor))
                  .ForMember(dest => dest.numDocumentoRecebedor, opt => opt.MapFrom(src => src.numDocumentoRecebedor))
                  .ForMember(dest => dest.valPrincipal, opt => opt.MapFrom(src => src.valPrincipal))
                  .ForMember(dest => dest.dtLiberacao, opt => opt.MapFrom(src => src.dtLiberacao))
                  .ForMember(dest => dest.siglaUfLiberacao, opt => opt.MapFrom(src => src.siglaUfLiberacao))
                  .ForMember(dest => dest.nomeCidadeLiberacao, opt => opt.MapFrom(src => src.nomeCidadeLiberacao))
                  .ForMember(dest => dest.dtVencimentoPrimeiraParcela, opt => opt.MapFrom(src => src.dtVencimentoPrimeiraParcela))
                  .ForMember(dest => dest.dtVencimentoUltimaParcela, opt => opt.MapFrom(src => src.dtVencimentoUltimaParcela))
                  .ForMember(dest => dest.nomeIndiceCorrecaoUtilizado, opt => opt.MapFrom(src => src.nomeIndiceCorrecaoUtilizado))
                  .ForMember(dest => dest.indMulta, opt => opt.MapFrom(src => src.indMulta))
                  .ForMember(dest => dest.indJurosMora, opt => opt.MapFrom(src => src.indJurosMora))
                  .ForMember(dest => dest.indPenalidade, opt => opt.MapFrom(src => src.indPenalidade))
                  .ForMember(dest => dest.indComissao, opt => opt.MapFrom(src => src.indComissao))
                  .ForMember(dest => dest.codTipoApontamento, opt => opt.MapFrom(src => src.codTipoApontamento));
        

            // Mapeamento inverso, se necessário
            CreateMap<Contrato, contratoDTO>();

        }
    }
}
