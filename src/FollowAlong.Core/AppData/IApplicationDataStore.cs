namespace MrCapitalQ.FollowAlong.Core.AppData;

public interface IApplicationDataStore
{
    object? GetValue(string key);
    void SetValue(string key, object? value);
}
