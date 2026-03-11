using MyAPP.Common;

public interface IRefreshToken
{
    void Add(RefreshToken token);
    RefreshToken? Get(string token);
    void Revoke(int id);
}
