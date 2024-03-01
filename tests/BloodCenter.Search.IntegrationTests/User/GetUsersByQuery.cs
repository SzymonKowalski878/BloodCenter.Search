using BloodCenter.Identity.Client;
using BloodCenter.Search.Client;
using BloodCenter.Search.Client.Models;
using BloodCenter.Search.IntegrationTests.Infrastructure;
using BloodCenter.Search.IntegrationTests.User.Providers;
using Feree.ResultType.Results;
using MediatR;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloodCenter.Search.IntegrationTests.User
{
    public class GetUsersByQuery : IClassFixture<GetUsersByQuery.Fixture>
    {
        private readonly Fixture _fixture;

        public class Fixture : IntegrationTestsFixture<GetUsersByQueryDataProvider, SearchClient>
        {
            public override async Task InitializeAsync()
            {
                await UserSeeder.Seed(DataProvider.Documents);

                await base.InitializeAsync();
            }
        }

        public GetUsersByQuery(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetUsersByQuery_GivenIdQueryString_ShouldReturnResult()
        {
            var userId = _fixture.DataProvider.UserId;

            var response = await _fixture.ApiClient.GetUsersByQuery(new Client.Models.GetUsersByQueryRequestDto(userId.ToString()));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count.ShouldBe(1);
            result.First().Id.ShouldBe(userId);
        }

        [Fact]
        public async Task GetUsersByQuery_GivenEmptyQueryString_ShouldReturnAll()
        {
            var data = _fixture.DataProvider.Documents;

            var response = await _fixture.ApiClient.GetUsersByQuery(new(null));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count.ShouldBeGreaterThanOrEqualTo(data.Count());

            foreach(var item in result)
            {
                var findMatchingItem = data.FirstOrDefault(x => x.Id == item.Id.ToString());
                findMatchingItem.ShouldNotBeNull();
                findMatchingItem.Role.ShouldBe(item.Role);
                findMatchingItem.Role.ShouldBe(item.FirstName);
                findMatchingItem.Role.ShouldBe(item.Email);
                findMatchingItem.Role.ShouldBe(item.LastName);

            }
        }
    }
}
