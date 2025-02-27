using System.Text.Json.Serialization;
using gravameApi.src.Models.DTOs;

namespace gravameApi.src.Models
{
    public class Data
    {
        public Veiculo Veiculo { get; set; }

        public Credor Credor { get; set; }

        public Financiado Financiado { get; set; }

        public Contrato Contrato {  get; set; }
       
    }
}
