using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string DisplayName) : ICommand<Result>;