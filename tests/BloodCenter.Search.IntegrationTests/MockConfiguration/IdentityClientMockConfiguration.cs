using BloodCenter.Identity.Client.Models;
using BloodCenter.Identity.Client;
using Feree.ResultType.Factories;
using NSubstitute;
using NSubstitute.Extensions;

namespace BloodCenter.Search.IntegrationTests.MockConfiguration
{
    public static class IdentityClientMockConfiguration
    {
        public static void SetupGetUserById(this IIdentityClient identityClient, UserDto response) =>
            identityClient.Configure().GetUserById(Arg.Any<Guid>()).Returns(ResultFactory.CreateSuccessAsync(response));
    }
}
