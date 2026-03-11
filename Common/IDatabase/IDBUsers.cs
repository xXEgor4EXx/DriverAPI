namespace MyAPP.Common;

public interface IDBUsers: ITable<User>
{
    User? GetByEmail(string email);
}
