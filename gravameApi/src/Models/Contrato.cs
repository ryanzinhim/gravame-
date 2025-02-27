using System.Globalization;

namespace gravameApi.src.Models
{
    public class Contrato
    {
        public string numContrato { get; set; }
        public string dtContrato { get; set; }
        public int qteMesesVigenciaContrato { get; set; }
        public int indTipoDocumentoRecebedor { get; private set; }
        public string numDocumentoRecebedor { get; private set; }
        public decimal valPrincipal { get; set; }
        public string dtLiberacao { get; set; }
        public string siglaUfLiberacao { get; private set; }
        public string nomeCidadeLiberacao { get; private set; }
        public string dtVencimentoPrimeiraParcela { get;  set; }
        public string dtVencimentoUltimaParcela { get; set; } 
        public string nomeIndiceCorrecaoUtilizado { get;  private set; }
        public int indMulta { get;  private set; }
        public int indJurosMora { get;  private set; }

        public int indPenalidade { get; private set; }

        public int indComissao { get; private set; }
        public int codTipoApontamento { get; private set; }
        public string CardId { get; set; }

        
    }
}
