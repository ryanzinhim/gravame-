namespace gravameApi.src.Models
{
    public class Veiculo
    {
        public string numChassi { get; set; }
        public int indRemarcacao { get; private set; }
        public string siglaUfPlaca { get; set; }
        public string numRenavam { get; set; }

        public string numPlaca { get; set; }
        public string numAnoFabricacao { get; set; }
        public string numAnoModelo { get; set; }
        public int indVeiculoNovo { get; private set; }
        public string siglaUfLicenciamento { get; private set; }


    }
}

