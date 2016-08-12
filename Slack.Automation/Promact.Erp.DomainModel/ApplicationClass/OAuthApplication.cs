namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class OAuthApplication
    {
        /// <summary>
        /// OAuth App ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// OAuth App ClientSecret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// OAuth App RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// OAuth App ReturnUrl
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
