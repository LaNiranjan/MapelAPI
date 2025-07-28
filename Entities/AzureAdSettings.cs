namespace MapelRestAPI.Entities
{
    public class AzureAdSettings
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RoleId { get; set; }
        public string RedirectUrl { get; set; }
        public string Instance { get; set; }
        public string FrontAppClientId { get; set; }
    }
}
