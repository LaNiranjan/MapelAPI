using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MapelRestAPI.Entities;
using MapelRestAPI.Interfaces;

namespace MapelRestAPI.Identity;

public class GraphUserService : IUserService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tenantId;
    private readonly string _roletId;
    private readonly string _redirectUrl;

    public GraphUserService(IConfiguration config)
    {
        var azureAd = config.GetSection("AzureAd");
        _clientId = azureAd["ClientId"]!;
        _clientSecret = azureAd["ClientSecret"]!;
        _tenantId = azureAd["TenantId"]!;
        _roletId = azureAd["RoleId"]!;
        _redirectUrl = azureAd["RedirectUrl"]!; 
    }

    private GraphServiceClient GetClient()
    {
        var credential = new ClientSecretCredential(
            _tenantId,
            _clientId,
            _clientSecret
        );

        return new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task<List<string>> CreateUsersAsync(List<AppUser> users)
    {
        var client = GetClient();
        var results = new List<string>();

        foreach (var user in users)
        {
            try
            {
                var newUser = new User
                {
                    DisplayName = user.DisplayName,
                    UserPrincipalName = $"{Guid.NewGuid().ToString()}@maplebg.onmicrosoft.com",
                    MailNickname = user.Email.Split('@')[0],
                    AccountEnabled = true,
                    PasswordProfile = new PasswordProfile
                    {
                        Password = user.Password,
                        ForceChangePasswordNextSignIn = true
                    },
                    Mail = user.Email,
                    OtherMails = new List<string> { user.Email }
                };

                var resultCreate = await client.Users.PostAsync(newUser);
                results.Add($"Created: {resultCreate?.UserPrincipalName}");

                if (!string.IsNullOrEmpty(resultCreate?.Id))
                {
                    var servicePrincipal = await client.ServicePrincipals
                        .GetAsync(requestConfig =>
                            requestConfig.QueryParameters.Filter = $"appId eq '{_clientId}'");

                    var resudltRole = await client.Users[resultCreate?.Id]
                        .AppRoleAssignments
                        .PostAsync(new AppRoleAssignment
                        {
                            PrincipalId = Guid.Parse(resultCreate?.Id!),     // user ID
                            ResourceId = Guid.Parse(servicePrincipal?.Value?[0].Id!),       // service principal ID of the target app
                            AppRoleId = Guid.Parse(_roletId)             // ID of the app role
                        });

                }
            }
            catch (Exception ex)
            {
                results.Add($"Failed: {user.Email} - {ex.Message}");
            }
        }

        return results;
    }

    public async Task<string> InviteExternalUserAsync(string email)
    {
        var client = GetClient();
        try
        {
            var invitation = new Invitation
            {
                InvitedUserEmailAddress = email,
                InviteRedirectUrl = _redirectUrl,
                SendInvitationMessage = true,
                InvitedUserDisplayName = email.Split('@')[0] // Optional
            };

            var result = await client.Invitations.PostAsync(invitation);

            return $"Invitation sent successfully to: {result?.InvitedUserEmailAddress}";
        }
        catch (Exception ex)
        {
            return $"Error sending invitation: {ex.Message}";
        }
    }
}
