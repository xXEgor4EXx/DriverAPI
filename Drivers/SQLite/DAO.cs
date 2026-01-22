using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using System.IO;
using System.Threading;
using System;


namespace MyAPP.Driver;

class DAO
{
    private static DAO? _Instance;
    private SqliteConnection connection;
    private Mutex connectionMutex;
    
    public static void Initialize(DBConfig config)
    {
        _Instance = new DAO(config);
    }
    
    private DAO(DBConfig config)
    {
        // Формируем корректный путь к файлу БД.
        // Поддерживаем 2 варианта: Location+Database или Location как полный путь к файлу.
        string dataSource;
        if (!string.IsNullOrEmpty(config.Database))
        {
            var location = string.IsNullOrEmpty(config.Location) ? "." : config.Location;
            dataSource = Path.Combine(location, config.Database);
        }
        else
        {
            dataSource = config.Location ?? string.Empty;
        }

        // Приводим к абсолютному пути
        if (!string.IsNullOrEmpty(dataSource))
            dataSource = Path.GetFullPath(dataSource);

        // Если указан путь с директорией — убедимся, что директория существует
        var dir = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLite DAO] Failed to create directory '{dir}': {ex.Message}");
                throw;
            }
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dataSource,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        Console.WriteLine($"[SQLite DAO] Using DataSource: '{dataSource}'");

        connection = new SqliteConnection(builder.ConnectionString);
        connectionMutex = new Mutex();
    }
    
    protected DAO Dao => DAO.Instance;
    public static DAO Instance
    {
        get
        {
            if(_Instance != null)
            {
                return _Instance;
            }
            else
            {
                throw new Exception("Must be initialize first!");
            }
        }
    }
    
    private SqliteCommand GetCommand(string query, List<SqliteParameter>? parameters = null)
    {
        SqliteCommand cmd = connection.CreateCommand();
        cmd.CommandText = query;
        if(parameters != null)
        {
            foreach(SqliteParameter p in parameters)
            {
                cmd.Parameters.Add(p);
            }
        }
        return cmd;
    }
    public List<T> ExecuteReader<T>(string query, Func<SqliteDataReader, T> mapper, List<SqliteParameter>? parameters = null)
    {
        var result = new List<T>();
        SqliteCommand cmd = GetCommand(query, parameters);
        
        connectionMutex.WaitOne();
        try
        {
            connection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(mapper(reader));
                }
            }
        }
        finally
        {
            connection.Close();
            connectionMutex.ReleaseMutex();
        }
        
        return result;
    }

    public T? ReadSingle<T>(string query, Func<SqliteDataReader, T> mapper, List<SqliteParameter>? parameters = null) where T : class
    {
        SqliteCommand cmd = GetCommand(query, parameters);
        
        connectionMutex.WaitOne();
        try
        {
            connection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return mapper(reader);
                }
            }
        }
        finally
        {
            connection.Close();
            connectionMutex.ReleaseMutex();
        }
        
        return null;
    }

    public Nullable<T> ReadSingle<T>(string query, List<SqliteParameter>? parameters = null) where T : struct
    {
        T? result = null;
        object? dbValue = null;
        SqliteCommand cmd = GetCommand(query, parameters);
        
        connectionMutex.WaitOne();
        try
        {
            connection.Open();
            dbValue = cmd.ExecuteScalar();
        }
        finally
        {
            connection.Close();
            connectionMutex.ReleaseMutex();
        }
        
        if(dbValue != null && dbValue != DBNull.Value)
        {
            result = (T)Convert.ChangeType(dbValue, typeof(T));
        }
        return result;
    }
    
    public void ExecuteNonQuery(string query, List<SqliteParameter>? parameters = null)
    {
        SqliteCommand cmd = GetCommand(query, parameters);
        connectionMutex.WaitOne();
        try
        {
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connectionMutex.ReleaseMutex();
        }
    }
}
