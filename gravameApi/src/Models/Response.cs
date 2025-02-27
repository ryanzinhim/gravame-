using System.Text.Json.Serialization;

namespace gravameApi.src.Models
{
    public class Resultado
    {
        [JsonPropertyName("codigoRetorno")]
        public int CodigoRetorno { get; set; }

        [JsonPropertyName("mensagemRetorno")]
        public string MensagemRetorno { get; set; }

        [JsonPropertyName("numApontamento")]
        public long NumApontamento { get; set; }

        public bool IsSuccessful => CodigoRetorno == 30;
    }
    public class ApiResponse
    {
        [JsonPropertyName("data")]
        public Resultado Data { get; set; }
    }
    public class ErroResponse
    {
        [JsonPropertyName("erros")]
        public List<Erro> Erros { get; set; }
    }

    public class Erro
    {
        [JsonPropertyName("codigo")]
        public int Codigo { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("detalhe")]
        public string Detalhe { get; set; }
    }
}
