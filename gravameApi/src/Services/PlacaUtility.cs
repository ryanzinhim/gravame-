namespace gravameApi.src.Services
{
    public class PlacaUtility
    {
        private static readonly Dictionary<char, char> Substituicoes = new Dictionary<char, char>
    {
        {'0', 'A'}, {'1', 'B'}, {'2', 'C'}, {'3', 'D'}, {'4', 'E'},
        {'5', 'F'}, {'6', 'G'}, {'7', 'H'}, {'8', 'I'}, {'9', 'J'}
    };

        public static string AlterarQuintoCaractere(string placa)
        {
            if (placa.Length < 5) return placa;

            var placaArray = placa.ToCharArray();
            if (Substituicoes.TryGetValue(placaArray[4], out char novoCaractere))
            {
                placaArray[4] = novoCaractere;
            }

            return new string(placaArray);
        }
    }
}
