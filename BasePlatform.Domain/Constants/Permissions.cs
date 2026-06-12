namespace BasePlatform.Domain.Constants;

public static class Permissions
{
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersEdit = "users.edit";
    public const string UsersDelete = "users.delete";

    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesEdit = "roles.edit";
    public const string RolesDelete = "roles.delete";
    public const string RolesAssign = "roles.assign";

    public const string PermissionsView = "permissions.view";
    public const string PermissionsManage = "permissions.manage";

    public const string SettingsView = "settings.view";
    public const string SettingsUpdate = "settings.update";

    public const string FilesUpload = "files.upload";
    public const string FilesDelete = "files.delete";

    public const string AuditView = "audit.view";

    public const string AdminAccess = "admin.access";

    public static readonly IReadOnlyList<string> All =
    [
        UsersView, UsersCreate, UsersEdit, UsersDelete,
        RolesView, RolesCreate, RolesEdit, RolesDelete, RolesAssign,
        PermissionsView, PermissionsManage,
        SettingsView, SettingsUpdate,
        FilesUpload, FilesDelete,
        AuditView,
        AdminAccess
    ];
}