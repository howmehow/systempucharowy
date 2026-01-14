using HotChocolate.Types;
using TournamentApi.GraphQL.Inputs;
using TournamentApi.GraphQL.Types;
using TournamentApi.Services;

namespace TournamentApi.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class UserMutations
{
    public async Task<AuthPayload> Register(RegisterInput input, [Service] IAuthService authService)
    {
        var (token, user) = await authService.RegisterAsync(
            input.FirstName,
            input.LastName,
            input.Email,
            input.Password
        );

        return new AuthPayload
        {
            Token = token,
            User = user
        };
    }

    public async Task<AuthPayload?> Login(LoginInput input, [Service] IAuthService authService)
    {
        var result = await authService.LoginAsync(input.Email, input.Password);

        if (result == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return new AuthPayload
        {
            Token = result.Value.Token,
            User = result.Value.User
        };
    }
}
