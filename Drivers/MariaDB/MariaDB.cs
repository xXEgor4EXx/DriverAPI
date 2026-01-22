using MyAPP.Common;

namespace MyAPP.Driver.MariaDB;

    public class MariaDBDatabase : IDatabase
    {
        public MariaDBDatabase(DBConfig config)
        {
            DAO.Initialize(config);
        }
        public IDBAccruals Accruals  => new MariaDBAccruals();
        public IDBAccrualsType AccrualsType  => new MariaDBAccrualsType();
        public IDBEmployers Employers  => new MariaDBEmployers();
        public IDBOperations Operations => new MariaDBOperations();
        public IDBPayments Payments => new MariaDBPayments();
        public IDBProfessionEmployers ProfessionEmployers  => new MariaDBProfessionEmployers();
        public IDBProfessions Professions  => new MariaDBProfessions();
        public IDBWorks Work  => new MariaDBWorks();
        public IDBUsers Users => new MariaDBUsers();
        public IRefreshToken RefreshTokens => new MariaDBRefreshTokens();
    }