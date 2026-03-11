namespace MyAPP.Common
{
    public interface IDatabase
    {
        IDBAccruals Accruals { get; } 
        IDBAccrualsType AccrualsType { get; }
        IDBEmployers Employers { get; }
        IDBOperations Operations { get; }
        IDBPayments Payments { get; }
        IDBProfessionEmployers ProfessionEmployers { get; }
        IDBProfessions Professions { get; }
        IDBWorks Work { get; }
        IDBUsers Users { get; }
    }
}