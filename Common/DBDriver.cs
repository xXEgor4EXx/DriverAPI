namespace MyAPP.Common;

public class DBDriver
{
    public string Name { get; set; }
    public string LIB { get; set; }
    public DBConfig? DBConfig { get; set; }



    public DBDriver(string name, string lib)
    {
        Name = name;
        LIB = lib;
    }

    public DBDriver()
    {
        Name = string.Empty;
        LIB = string.Empty;
    }
}