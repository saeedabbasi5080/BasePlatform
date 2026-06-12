using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;

namespace BasePlatform.Application.Features.Audit.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    int Page = 1,
    int PageSize = 20,
    string? ActorEmail = null,
    string? Action = null,
    string? TargetEntityType = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null) : IQuery<Result<PaginatedResult<AuditLogDto>>>;