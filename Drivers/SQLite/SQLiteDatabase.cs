using MyAPP.Common;
using MyAPP.Driver.DB;
using MyAPP.Driver;

namespace MyAPP.Driver;

public class SQLiteDatabase : IDatabase
{
    public SQLiteDatabase(DBConfig config)
    {
        DAO.Initialize(config);
        CreateTablesIfNotExists();
    }
    private void CreateTablesIfNotExists()
    {
            foreach (var sql in DDL.Defenition)
            {
                DAO.Instance.ExecuteNonQuery(sql);
            }
    }
    public IDBAccruals Accruals => new DBAccruals();
    public IDBAccrualsType AccrualsType => new DBAccrualsType();
    public IDBEmployers Employers => new DBEmployers();
    public IDBOperations Operations => new DBOperations();
    public IDBPayments Payments => new DBPayments();
    public IDBProfessionEmployers ProfessionEmployers => new DBProfessionEmployers();
    public IDBProfessions Professions => new DBProfessions();
    public IDBWorks Work => new DBWorks();
    public IDBUsers Users => new DBUsers();
    public IRefreshToken RefreshTokens => new DBRefreshTokens();
}
