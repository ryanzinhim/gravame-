using Newtonsoft.Json;
using gravameApi.src.DataReq;

namespace gravameApi.src.Models.DTOs
{
    public class contratoDTO
    {
        [JsonIgnore]
        public string numContrato { get; set; }
        public string dtContrato { get; set; }
        public int qteMesesVigenciaContrato { get; set; }
        public int indTipoDocumentoRecebedor { get; set; }
        public string numDocumentoRecebedor { get; set; }

        // Remover validação de data na propriedade valPrincipal
        public decimal valPrincipal { get; set; }

        // Aplicar a validação nas propriedades de data corretamente
        public string dtLiberacao { get; set; }
        public string dtVencimentoPrimeiraParcela { get; set; }
        public string? dtVencimentoUltimaParcela { get; set; }

        public string siglaUfLiberacao { get; set; }
        public string nomeCidadeLiberacao { get; set; }
        public string nomeIndiceCorrecaoUtilizado { get; set; }
        public int indMulta { get; set; }
        public int indJurosMora { get; set; }
        public int indPenalidade { get; set; }
        public int indComissao { get; set; }
        public int codTipoApontamento { get; set; }
        public string CardId { get; set; }
    



    }
}
