namespace MyAPP.Common;

public class DBConfig
{
    public string? Location { get; set; }
    public string? Database { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public DBConfig(string location, string database, string username, string password)
    {
        Location = location;
        Database = database;
        UserName = username;
        Password = password;
    }

    public DBConfig()
    {
        Location = string.Empty;
        Database = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }
}