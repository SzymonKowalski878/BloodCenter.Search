using BloodCenter.Identity.Client;
using BloodCenter.Search.Application.Events;
using BloodCenter.Search.IntegrationTests.Events.Providers;
using BloodCenter.Search.IntegrationTests.Infrastructure;
using BloodCenter.Search.IntegrationTests.MockConfiguration;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloodCenter.Search.IntegrationTests.Events
{
    public class UserAddedEvent : IClassFixture<UserAddedEvent.Fixture>
    {
        private readonly Fixture _fixture;

        public class Fixture : IntegrationTestsFixture<UserAddedEventDataProvider>
        {

        }

        public UserAddedEvent(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task UserAddedEvent_GivenValidRequest_ShouldReturnSuccess()
        {
            var user = _fixture.DataProvider.GetUserByIdResponse;
            var time = DateTimeOffset.UtcNow;
            var context = Substitute.For<ConsumeContext<Identity.Events.UserAddedEvent>>();
            var message = new Identity.Events.UserAddedEvent(user.Id, time);

            context.Configure().Message.Returns(message);
            _fixture.IdentityClientMock.SetupGetUserById(user);

            var consumer = _fixture.Services.GetRequiredService<UserAddedEventHandler>();

            await consumer.Consume(context); 

            await Task.Delay(TimeSpan.FromSeconds(2));

            var document = await _fixture.UserSeeder.FindDocumentById(user.Id.ToString());
            document.ShouldNotBeNull();
            document.Id.ShouldBe(user.Id.ToString());
            document.Role.ShouldBe(user.Role);
            document.FirstName.ShouldBe(user.FirstName.ToString());
            document.LastName.ShouldBe(user.LastName.ToString());
            _fixture.UserSeeder.AddDocumentIdToBeRemoved(user.Id);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
