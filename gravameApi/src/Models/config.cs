
namespace gravameApi.Config

{
    public class AuthConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChaveIntegracao { get; set; }
        public string UrlCertificacao { get; set; }
        public string UrlInclusao { get; set; }
        public string UrlAuth { get; set; }
        public CertificadosConfig Certificados { get; set; }

        public class CertificadosConfig
        {
            public string CertificatePath { get; set; }
            public string CertificateKeyPath { get; set; }
            public string CertificateCaPath { get; set; }
        }
    }
}
