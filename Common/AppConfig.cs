namespace MyAPP.Common;

public class AppConfig
{
    public string DBType { get; }
    public List<DBDriver> DBDriver { get; set; }


    public AppConfig(string dbType)
    {
        DBType = dbType;
        DBDriver = new List<DBDriver>();
    }
}