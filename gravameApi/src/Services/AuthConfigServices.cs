using System.Text.Json.Serialization;

namespace gravameApi.src.Services
{

    public class AuthConfigServices
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChaveIntegracao { get; set; }
        public string UrlInclusao { get; set; }
        public string UrlAuth { get; set; }
        public string Senha { get; set; }


        public CertificadosConfig Certificados { get; set; }

        public class CertificadosConfig
        {
            public string CertificatePath { get; set; }
            public string CertificateCaPath { get; set; }
        }
public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }
}

    

}



