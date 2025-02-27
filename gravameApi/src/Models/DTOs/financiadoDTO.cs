namespace gravameApi.src.Models.DTOs
{
    public class financiadoDTO
    {
        public string nome { get; set; }
        public int indTipoDocumento { get; set; }
        public string numDocumento { get; set; }
        public string nomeEndereco { get; set; }
        public string numEndereco { get; set; }
        public string nomeBairroEndereco { get; set; }
        public string siglaUfEndereco { get; set; }
        public int codMunicipioEndereco { get; set; }
        public string numCepEndereco { get; set; }
        public string numDddTelefone { get; set; }

        public string numTelefone { get; set; }
        public string? nomeMunicipioEndereco { get; set; }
    }
}
