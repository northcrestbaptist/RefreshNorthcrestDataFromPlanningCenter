using Microsoft.AspNetCore.Authorization;

namespace NBC.Authorization
{
    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy CanAccessSermonNotes()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("mediaTeam")
                .Build();
        }
    }
}