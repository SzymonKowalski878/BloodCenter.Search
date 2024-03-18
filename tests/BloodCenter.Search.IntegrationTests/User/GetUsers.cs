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
    public class GetUsers : IClassFixture<GetUsers.Fixture>
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

        public GetUsers(Fixture fixture)
        {
            _fixture = fixture;
        }

        
        [Fact]
        public async Task GetUsersByQuery_GivenWorkerRole_ShouldReturnResult()
        {
            var roles = new string[] { "Worker" };

            var userId = _fixture.DataProvider.UserId;

            var response = await _fixture.ApiClient.GetUsersByQuery(new Client.Models.GetUsersRequestDto(null, roles));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count().ShouldBeGreaterThan(0);
            foreach(var item in result)
            {
                roles.Contains(item.Role).ShouldBeTrue();
            }
        }

        [Fact]
        public async Task GetUsersByQuery_GivenAdminRole_ShouldReturnResult()
        {
            var roles = new string[] { "Admin" };

            var userId = _fixture.DataProvider.UserId;

            var response = await _fixture.ApiClient.GetUsersByQuery(new Client.Models.GetUsersRequestDto(null, roles));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count().ShouldBeGreaterThan(0);
            foreach (var item in result)
            {
                roles.Contains(item.Role).ShouldBeTrue();
            }
        }

        [Fact]
        public async Task GetUsersByQuery_GivenRandomRole_ShouldReturnEmptyResult()
        {
            var roles = new string[] { "Random" };

            var userId = _fixture.DataProvider.UserId;

            var response = await _fixture.ApiClient.GetUsersByQuery(new Client.Models.GetUsersRequestDto(null, roles));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetUsersByQuery_GivenEmptyQueryString_ShouldReturnAll()
        {
            var data = _fixture.DataProvider.Documents;

            var response = await _fixture.ApiClient.GetUsersByQuery(new(null, null));

            var result = response.ShouldBeOfType<Success<IReadOnlyList<UserDocumentDto>>>().Payload;

            result.Count.ShouldBeGreaterThanOrEqualTo(data.Count());

            foreach(var item in data)
            {
                var findMatchingItem = data.FirstOrDefault(x => x.Id == item.Id);
                findMatchingItem.ShouldNotBeNull();
                findMatchingItem.Role.ShouldBe(item.Role);
                findMatchingItem.FirstName.ShouldBe(item.FirstName);
                findMatchingItem.Email.ShouldBe(item.Email);
                findMatchingItem.LastName.ShouldBe(item.LastName);

            }
        }
    }
}
