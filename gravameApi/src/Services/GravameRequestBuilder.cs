using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using gravameApi.src.DataReq;
using gravameApi.src.Models.DTOs;

namespace gravameApi.src.Services
{
    public class GravameRequestBuilder
    {
        private string AlterarQuintoCaracterePlaca(string numPlaca)
        {
            if (numPlaca.Length < 5) return numPlaca;

            char[] placaArray = numPlaca.ToCharArray();
            Dictionary<char, char> substituicoes = new Dictionary<char, char>
    {
        {'0', 'A'}, {'1', 'B'}, {'2', 'C'}, {'3', 'D'}, {'4', 'E'},
        {'5', 'F'}, {'6', 'G'}, {'7', 'H'}, {'8', 'I'}, {'9', 'J'}
    };

            if (substituicoes.ContainsKey(placaArray[4]))
            {
                placaArray[4] = substituicoes[placaArray[4]];
            }

            return new string(placaArray);
        }

        public object Build(DataRequest dataRequest, bool converterPlaca, string placaLimpa)
        {
            if (dataRequest.Data.Financiado.siglaUfEndereco != "RJ")
            {
                dataRequest.Data.Financiado.siglaUfEndereco = "RJ";
                dataRequest.Data.Financiado.numCepEndereco = "21515000";
                dataRequest.Data.Financiado.codMunicipioEndereco = 6001;
            }

            if (dataRequest.Data.Veiculo.numRenavam.EndsWith(".0"))
            {
                dataRequest.Data.Veiculo.numRenavam = dataRequest.Data.Veiculo.numRenavam.Substring(0, dataRequest.Data.Veiculo.numRenavam.Length - 2);
            }

            if (dataRequest.Data.Veiculo.numRenavam.Length != 11)
            {
                dataRequest.Data.Veiculo.numRenavam = dataRequest.Data.Veiculo.numRenavam.PadLeft(11, '0');
            }
            string placaFinal = converterPlaca ? AlterarQuintoCaracterePlaca(placaLimpa) : placaLimpa;

            static string CleanString(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                string normalizedString = input.Normalize(NormalizationForm.FormD);

                string semAcentos = Regex.Replace(normalizedString, @"[\p{M}]", "");

                return Regex.Replace(semAcentos, @"[^\w\s]", "");
            }

            return new
            {
                placaFinal,  // Retorna placaFinal
                placaLimpa,

                data = new DataDTO
                {

                    veiculo = new veiculoDTO
                    {
                        numChassi = dataRequest.Data.Veiculo.numChassi.Substring(0, 17),
                        indRemarcacao = dataRequest.Data.Veiculo.indRemarcacao,
                        numAnoFabricacao = dataRequest.Data.Veiculo.numAnoFabricacao.Trim().Substring(0, 4),
                        numAnoModelo = dataRequest.Data.Veiculo.numAnoModelo.Trim().Substring(0, 4),
                        indVeiculoNovo = dataRequest.Data.Veiculo.indVeiculoNovo,
                        siglaUfLicenciamento = dataRequest.Data.Veiculo.siglaUfLicenciamento,
                        siglaUfPlaca = dataRequest.Data.Veiculo.siglaUfPlaca.Trim().Substring(dataRequest.Data.Veiculo.siglaUfPlaca.Trim().Length - 2),
                        numPlaca = placaFinal,
                        numRenavam = dataRequest.Data.Veiculo.numRenavam
                    },

                    financiado = new financiadoDTO
                    {
                        nome = dataRequest.Data.Financiado.nome.Length > 39 ? dataRequest.Data.Financiado.nome.Substring(0, 39) : dataRequest.Data.Financiado.nome,
                        indTipoDocumento = dataRequest.Data.Financiado.indTipoDocumento,
                        numDocumento = dataRequest.Data.Financiado.numDocumento.Replace(".", "").Replace("-", "").Replace("/", ""),
                        nomeEndereco = CleanString(dataRequest.Data.Financiado.nomeEndereco),
                        numEndereco = CleanString(dataRequest.Data.Financiado.numEndereco),
                        nomeBairroEndereco = CleanString(dataRequest.Data.Financiado.nomeBairroEndereco),
                        siglaUfEndereco = dataRequest.Data.Financiado.siglaUfEndereco,
                        codMunicipioEndereco = (int)dataRequest.Data.Financiado.codMunicipioEndereco,
                        numCepEndereco = dataRequest.Data.Financiado.numCepEndereco,
                        numDddTelefone = dataRequest.Data.Financiado.numTelefone.Replace("+55", "").Trim().Substring(0, 2),
                        numTelefone = dataRequest.Data.Financiado.numTelefone.Replace("+55", "").Replace("-", "").Trim().Substring(3, 9)
                    },
                    credor = new credorDTO()
                    {
                        nome = dataRequest.Data.Credor.nome,
                        numDocumento = dataRequest.Data.Credor.numDocumento.Replace(".", "").Replace("-", "").Replace("/", ""),
                        nomeEndereco = dataRequest.Data.Credor.nomeEndereco,
                        numEndereco = dataRequest.Data.Credor.numEndereco,
                        nomeBairroEndereco = dataRequest.Data.Credor.nomeBairroEndereco,
                        siglaUfEndereco = dataRequest.Data.Credor.siglaUfEndereco,
                        codMunicipioEndereco = dataRequest.Data.Credor.codMunicipioEndereco,
                        numCepEndereco = dataRequest.Data.Credor.numCepEndereco,
                        numDddTelefone = dataRequest.Data.Credor.numDddTelefone,
                        numTelefone = dataRequest.Data.Credor.numTelefone
                    },


                    contrato = new contratoDTO()
                    {
                        numContrato = dataRequest.Data.Contrato.numContrato,
                        dtContrato = DateTime.Parse(dataRequest.Data.Contrato.dtContrato).ToString("yyyy-MM-dd", new CultureInfo("ja-JP")),
                        qteMesesVigenciaContrato = dataRequest.Data.Contrato.qteMesesVigenciaContrato,
                        indTipoDocumentoRecebedor = dataRequest.Data.Contrato.indTipoDocumentoRecebedor,
                        numDocumentoRecebedor = dataRequest.Data.Contrato.numDocumentoRecebedor.Replace(".", "").Replace("-", "").Replace("/", ""),
                        valPrincipal = dataRequest.Data.Contrato.valPrincipal,
                        dtLiberacao = DateTime.Parse(dataRequest.Data.Contrato.dtLiberacao).ToString("yyyy-MM-dd", new CultureInfo("ja-JP")),
                        siglaUfLiberacao = dataRequest.Data.Contrato.siglaUfLiberacao,
                        nomeCidadeLiberacao = dataRequest.Data.Contrato.nomeCidadeLiberacao,
                        dtVencimentoPrimeiraParcela = DateTime.Parse(dataRequest.Data.Contrato.dtVencimentoPrimeiraParcela).ToString("yyyy-MM-dd", new CultureInfo("ja-JP")),
                        dtVencimentoUltimaParcela = DateTime.Parse(dataRequest.Data.Contrato.dtVencimentoUltimaParcela).ToString("yyyy-MM-dd", new CultureInfo("ja-JP")),
                        nomeIndiceCorrecaoUtilizado = dataRequest.Data.Contrato.nomeIndiceCorrecaoUtilizado,
                        indMulta = dataRequest.Data.Contrato.indMulta,
                        indJurosMora = dataRequest.Data.Contrato.indJurosMora,
                        indPenalidade = dataRequest.Data.Contrato.indPenalidade,
                        indComissao = dataRequest.Data.Contrato.indComissao,
                        codTipoApontamento = dataRequest.Data.Contrato.codTipoApontamento,
                        CardId = dataRequest.Data.Contrato.CardId

                    }
                }
            };
        }

    }
}
