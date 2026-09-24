using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Backend.Test;

public class EndpointTest
{
    [Fact]
    public async Task TrueTest()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var requestBody = new
        {
            email = "mail@for.testing",
            username = "testuUser",
            password = "SafeStrongPassword123"
        };
 
        var response = await client.PostAsJsonAsync("/user/create-user", requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}