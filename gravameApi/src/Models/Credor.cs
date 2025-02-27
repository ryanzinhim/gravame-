namespace gravameApi.src.Models
{
    public class Credor
    {
        public string nome { get; private set; }
        public string numDocumento { get; private set; }

        public string nomeEndereco { get; private set; } 
        public string numEndereco { get; private set; } 
        public string nomeBairroEndereco { get; private set; } 

        public string siglaUfEndereco { get; private set; } 

        public int codMunicipioEndereco { get; private set; } 

        public string numCepEndereco { get; private set; } 
        public string numDddTelefone { get; private set; } 

        public string numTelefone { get; private set; } 

    }
}
