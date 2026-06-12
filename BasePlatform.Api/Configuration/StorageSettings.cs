namespace BasePlatform.Api.Configuration;

public sealed class StorageSettings
{
    public LocalStorageOptions Local { get; init; } = new();
}

public sealed class LocalStorageOptions
{
    public string BasePath { get; init; } = "./uploads";
}