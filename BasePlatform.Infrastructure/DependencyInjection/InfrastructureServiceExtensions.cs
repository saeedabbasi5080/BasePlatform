using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Application.Features.Admin.Auth.Login;
using BasePlatform.Application.Features.Audit.GetAuditLogs;
using BasePlatform.Application.Features.Auth.ConfirmEmail;
using BasePlatform.Application.Features.Auth.ForgotPassword;
using BasePlatform.Application.Features.Auth.Login;
using BasePlatform.Application.Features.Auth.Logout;
using BasePlatform.Application.Features.Auth.RefreshToken;
using BasePlatform.Application.Features.Auth.Register;
using BasePlatform.Application.Features.Auth.ResetPassword;
using BasePlatform.Application.Features.Files.DeleteFile;
using BasePlatform.Application.Features.Files.GetFileById;
using BasePlatform.Application.Features.Files.UploadFile;
using BasePlatform.Application.Features.Permissions.AssignPermissionsToRole;
using BasePlatform.Application.Features.Permissions.GetAllPermissions;
using BasePlatform.Application.Features.Permissions.GetRolePermissions;
using BasePlatform.Application.Features.Roles.CreateRole;
using BasePlatform.Application.Features.Roles.DeleteRole;
using BasePlatform.Application.Features.Roles.GetAllRoles;
using BasePlatform.Application.Features.Roles.GetRoleById;
using BasePlatform.Application.Features.Roles.UpdateRole;
using BasePlatform.Application.Features.Settings.GetSettingByKey;
using BasePlatform.Application.Features.Settings.GetSettings;
using BasePlatform.Application.Features.Settings.UpsertSetting;
using BasePlatform.Application.Features.Users.AssignRole;
using BasePlatform.Application.Features.Users.ChangePassword;
using BasePlatform.Application.Features.Users.DeactivateUser;
using BasePlatform.Application.Features.Users.GetAllUsers;
using BasePlatform.Application.Features.Users.GetCurrentUser;
using BasePlatform.Application.Features.Users.GetUserById;
using BasePlatform.Application.Features.Users.UpdateProfile;
using BasePlatform.Infrastructure.Authentication;
using BasePlatform.Infrastructure.Authorization;
using BasePlatform.Infrastructure.Dispatcher;
using BasePlatform.Infrastructure.Email;
using BasePlatform.Infrastructure.Identity;
using BasePlatform.Infrastructure.Persistence;
using BasePlatform.Infrastructure.Persistence.Dapper;
using BasePlatform.Infrastructure.Persistence.Repositories;
using BasePlatform.Infrastructure.Queries.Admin;
using BasePlatform.Infrastructure.Queries.Audit;
using BasePlatform.Infrastructure.Queries.Files;
using BasePlatform.Infrastructure.Queries.Permissions;
using BasePlatform.Infrastructure.Queries.Roles;
using BasePlatform.Infrastructure.Queries.Settings;
using BasePlatform.Infrastructure.Queries.Users;
using BasePlatform.Infrastructure.Services;
using BasePlatform.Infrastructure.Storage;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasePlatform.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var connStr = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"[DEBUG] ConnectionString = '{connStr}'");
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IDapperQueryConnection, DapperQueryConnection>();

        // Identity
        services.AddIdentityServices();
        services.AddScoped<IdentitySeeder>();

        // Authorization
        services.AddScoped<IUserClaimsPrincipalFactory<BasePlatform.Domain.Entities.AppUser>,
            PermissionClaimsPrincipalFactory>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // JWT & Token
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IStorageService, LocalStorageService>();

        // Repositories
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<IStoredFileRepository, StoredFileRepository>();


        // Dispatcher
        services.AddScoped<IDispatcher, BasePlatform.Infrastructure.Dispatcher.Dispatcher>();

        // Auth Handlers
        services.AddScoped<ICommandHandler<RegisterCommand, Result<RegisterResponse>>, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCommand, Result<LoginResponse>>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand, Result>, LogoutCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<ConfirmEmailCommand, Result>, ConfirmEmailCommandHandler>();
        services.AddScoped<ICommandHandler<ForgotPasswordCommand, Result>, ForgotPasswordCommandHandler>();
        services.AddScoped<ICommandHandler<ResetPasswordCommand, Result>, ResetPasswordCommandHandler>();

        // Users Handlers
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, Result<UserProfileResponse>>, GetCurrentUserQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, Result<UserProfileResponse>>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllUsersQuery, Result<PaginatedResult<UserSummaryDto>>>, GetAllUsersQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateProfileCommand, Result>, UpdateProfileCommandHandler>();
        services.AddScoped<ICommandHandler<ChangePasswordCommand, Result>, ChangePasswordCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateUserCommand, Result>, DeactivateUserCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRoleCommand, Result>, AssignRoleCommandHandler>();

        // Roles Handlers
        services.AddScoped<IQueryHandler<GetAllRolesQuery, Result<List<RoleSummaryDto>>>, GetAllRolesQueryHandler>();
        services.AddScoped<IQueryHandler<GetRoleByIdQuery, Result<RoleDetailResponse>>, GetRoleByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, Result<Guid>>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRoleCommand, Result>, UpdateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand, Result>, DeleteRoleCommandHandler>();

        // Permissions Handlers
        services.AddScoped<IQueryHandler<GetAllPermissionsQuery, Result<List<PermissionDto>>>, GetAllPermissionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetRolePermissionsQuery, Result<List<PermissionDto>>>, GetRolePermissionsQueryHandler>();
        services.AddScoped<ICommandHandler<AssignPermissionsToRoleCommand, Result>, AssignPermissionsToRoleCommandHandler>();

        // Settings Handlers 
        services.AddScoped<IQueryHandler<GetSettingsQuery, Result<List<AppSettingDto>>>, GetSettingsQueryHandler>();
        services.AddScoped<IQueryHandler<GetSettingByKeyQuery, Result<AppSettingDto>>, GetSettingByKeyQueryHandler>();
        services.AddScoped<ICommandHandler<UpsertSettingCommand, Result>, UpsertSettingCommandHandler>();

        // Files Query Handlers
        services.AddScoped<IQueryHandler<GetFileByIdQuery, Result<StoredFileDto>>, GetFileByIdQueryHandler>();

        // Files Command Handlers
        services.AddScoped<ICommandHandler<UploadFileCommand, Result<UploadFileResponse>>, UploadFileCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteFileCommand, Result>, DeleteFileCommandHandler>();

        // Audit Query Handler
        services.AddScoped<IQueryHandler<GetAuditLogsQuery, Result<PaginatedResult<AuditLogDto>>>, GetAuditLogsQueryHandler>();

        // Admin Auth Handler
        services.AddScoped<ICommandHandler<AdminLoginCommand, Result<AdminLoginResponse>>, AdminLoginCommandHandler>();

        return services;
    }
}