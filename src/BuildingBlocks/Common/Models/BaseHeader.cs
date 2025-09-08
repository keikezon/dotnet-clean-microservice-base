namespace Common.Models;

    /// <summary>
    ///     Base dos headers
    /// </summary>
    public class BaseHeader
    {
        /// <summary>
        ///     Construtor Base Header
        /// </summary>
        public BaseHeader()
        {
        }

        /// <summary>
        ///     Construtor recebendo string de autorzação
        /// </summary>
        /// <param name="authorization"></param>
        public BaseHeader(string authorization) : this(authorization, authorization, authorization)
        {
        }

        /// <summary>
        ///     Construtor
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="apiKey"></param>
        /// <param name="appAuth"></param>
        /// <param name="contentType"></param>
        /// <param name="authorizationHeader"></param>
        /// <param name="apiKeyHeader"></param>
        /// <param name="applicationAuthorizationHeader"></param>
        /// <param name="contentTypeHeader"></param>
        public BaseHeader(string authorization, string apiKey, string appAuth, string contentType = null,
            string authorizationHeader = null, string apiKeyHeader = null, string applicationAuthorizationHeader = null,
            string contentTypeHeader = null)
        {
            Authorization = authorization;
            ApiKey = apiKey;
            ContentType = contentType;
            AppAuthorization = appAuth;


            AuthorizationHeader = authorizationHeader ?? AuthorizationHeader;
            ApiKeyHeader = apiKeyHeader ?? ApiKeyHeader;
            AppAuthorizationHeader = applicationAuthorizationHeader ?? AppAuthorizationHeader;
            ContentTypeHeader = contentTypeHeader ?? ContentTypeHeader;
        }

        /// <summary>
        ///     Header de autorização
        /// </summary>
        public string AuthorizationHeader { get; set; } = "Authorization";

        /// <summary>
        ///     Header da chave da API
        /// </summary>
        public string ApiKeyHeader { get; set; } = "x-api-key";

        /// <summary>
        ///     Header da autorização do app
        /// </summary>
        public string AppAuthorizationHeader { get; set; } = "App-Authorization";

        /// <summary>
        ///     Header do tipo de conteúdo
        /// </summary>
        public string ContentTypeHeader { get; set; } = "Content-Type";

        /// <summary>
        ///     Header do código do canal
        /// </summary>
        public string ChannelCodeHeader { get; set; } = "channel-code";

        /// <summary>
        ///     Header da versão do canal
        /// </summary>
        public string ChannelVersionHeader { get; set; } = "channel-version";

        /// <summary>
        ///     Header de autorização
        /// </summary>
        /// <example>PSKXAP(S0cicps09fud9asfjfodfMPOdOLkOIoikLAM>j9p8dsd()diu-90adsladlma</example>
        public string Authorization { get; set; }

        /// <summary>
        ///     Chave da API
        /// </summary>
        /// <example>J9a9d9aiJÇsijdi2hhuif7s875*9adhanchucodt6t676GSAHUUOAsdiu9909220199a8</example>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Autorização do app
        /// </summary>
        /// <example>J9a9d9aiJÇsijdi2hhuif7s875*9adhanchucodt6t676GSAHUUOAsdiu9909220199a8</example>
        public string AppAuthorization { get; set; }

        /// <summary>
        ///     Tipo de conteúdo
        /// </summary>
        /// <example>application/json</example>
        public string ContentType { get; set; }

        /// <summary>
        ///     Código do canal
        /// </summary>
        /// <example>1</example>
        public string ChannelCode { get; set; }

        /// <summary>
        ///     Versão do canal
        /// </summary>
        /// <example>1.0.0</example>
        public string ChannelVersion { get; set; }

        /// <summary>
        ///     Buscar headers
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>
            {
                { AuthorizationHeader, Authorization },
                { ApiKeyHeader, ApiKey },
                { ContentTypeHeader, string.IsNullOrWhiteSpace(ContentType) ? "application/json" : ContentType },
                { AppAuthorizationHeader, AppAuthorization },
                { ChannelCodeHeader, ChannelCode },
                { ChannelVersionHeader, ChannelVersion }
            };
        }
    }