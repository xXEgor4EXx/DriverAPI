using System.Reflection;
using MyAPP.Singletons;
using MyAPP.Common;

public sealed class DAO
{
    private static readonly Lazy<IDatabase> lazyInstance = new(() =>
    {
        var config = MyAppConfig.Instance;
        if (string.IsNullOrEmpty(config.DBType))
            throw new Exception("DBType not specified in config");
        var driver = config.DBDriver.FirstOrDefault(d =>
            d.Name.Equals(config.DBType, StringComparison.OrdinalIgnoreCase));
        if (driver == null)
            throw new Exception($"Driver for DBType '{config.DBType}' not found");
        string assemblyPath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
            driver.LIB));
        if (!File.Exists(assemblyPath))
        {
            var projectRoot = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                "..", "..", "..", ".."));
            assemblyPath = Path.Combine(projectRoot, "Drivers", driver.Name, "bin", "Debug", "net9.0", $"{driver.Name}.dll");
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException($"DLL not found: {assemblyPath}");
        }
        var asm = Assembly.LoadFrom(assemblyPath);
        var type = asm.GetTypes().FirstOrDefault(t =>
            t.IsClass && !t.IsAbstract &&
            (t.Name.EndsWith("Database", StringComparison.OrdinalIgnoreCase) ||
            t.GetInterfaces().Any(i => i.FullName == typeof(IDatabase).FullName))
        );
        if (type == null)
        throw new Exception("Database class not found");
        
         try
  {
      var dbConfig = new DBConfig( 
            driver.DBConfig?.Location ?? "",
            driver.DBConfig?.Database ?? "",
            driver.DBConfig?.UserName ?? "",
            driver.DBConfig?.Password ?? ""
        );
      return (IDatabase)Activator.CreateInstance(type, dbConfig)!;
  }
  catch (TargetInvocationException tie)
  {
      Console.WriteLine("=== DAO inner exception ===");
      Console.WriteLine(tie.InnerException?.ToString() ?? tie.ToString());
      throw;
  }
  catch (Exception ex)
  {
      Console.WriteLine("=== DAO create instance failed ==="); 
      Console.WriteLine(ex.ToString());
      throw;
  }});
    public static IDatabase Instance => lazyInstance.Value;
    private DAO() { }
}
