using Microsoft.AspNetCore.Authorization;

namespace Basics.AuthorizationRequirements
{
    public class CustomRequireClaim : IAuthorizationRequirement
    {
        public CustomRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }

    public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CustomRequireClaim requirement
            )
        {
            if (context.User.Claims.Any(a => a.Type == requirement.ClaimType))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }


    public static class AuthorizationPolicyBuilderExtention
    {
        public static AuthorizationPolicyBuilder CustomAuthorizationPolicyBuilder
            (this AuthorizationPolicyBuilder builder, string claimType)
        {
            builder.AddRequirements(new CustomRequireClaim(claimType));
            return builder;
        }
    }


}
