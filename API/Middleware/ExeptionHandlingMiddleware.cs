using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using MyAPP.API.Models;

namespace MyAPP.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "SQLite error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleSqliteException(context, ex);
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MariaDB error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleMySqlException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteError(context, HttpStatusCode.InternalServerError,
                "Внутренняя ошибка сервера. Попробуйте позже.");
        }
    }
    private static async Task HandleSqliteException(HttpContext context, SqliteException ex)
    {
        // SQLite Extended Error Codes: https://www.sqlite.org/rescode.html
        // 787  = SQLITE_CONSTRAINT_FOREIGNKEY
        // 2067 = SQLITE_CONSTRAINT_UNIQUE
        // 1299 = SQLITE_CONSTRAINT_NOTNULL
        // 19   = SQLITE_CONSTRAINT (общая)

        var msg = ex.Message ?? "";

        if (ex.SqliteExtendedErrorCode == 787 || msg.Contains("FOREIGN KEY"))
        {
            var table = ExtractTableFromMessage(msg);
            var humanMsg = table != null
                ? $"Невозможно выполнить операцию: существуют связанные записи в таблице «{table}». Сначала удалите их."
                : "Невозможно выполнить операцию: существуют связанные записи в другой таблице. Сначала удалите их.";
            await WriteError(context, HttpStatusCode.Conflict, humanMsg);
            return;
        }
        if (ex.SqliteExtendedErrorCode == 2067 || msg.Contains("UNIQUE"))
        {
            var field = ExtractSqliteUniqueField(msg);
            var humanMsg = field != null
                ? $"Запись с таким значением «{field}» уже существует."
                : "Запись с такими данными уже существует.";
            await WriteError(context, HttpStatusCode.Conflict, humanMsg);
            return;
        }
        if (ex.SqliteExtendedErrorCode == 1299 || msg.Contains("NOT NULL"))
        {
            var field = ExtractSqliteNotNullField(msg);
            var humanMsg = field != null
                ? $"Поле «{field}» обязательно для заполнения."
                : "Одно из обязательных полей не заполнено.";
            await WriteError(context, HttpStatusCode.BadRequest, humanMsg);
            return;
        }
        if (ex.SqliteErrorCode == 19 || msg.Contains("constraint"))
        {
            await WriteError(context, HttpStatusCode.BadRequest,
                "Нарушение ограничения базы данных. Проверьте введённые данные.");
            return;
        }
        await WriteError(context, HttpStatusCode.InternalServerError, "Ошибка базы данных. Попробуйте позже.");
    }
    private static async Task HandleMySqlException(HttpContext context, MySqlException ex)
    {
        // MySQL/MariaDB Error Codes: https://dev.mysql.com/doc/mysql-errors/8.0/en/server-error-reference.html
        // 1451 = ER_ROW_IS_REFERENCED_2 — нельзя удалить родителя, есть дочерние строки
        // 1452 = ER_NO_REFERENCED_ROW_2 — нельзя вставить/обновить: нет такого родителя
        // 1062 = ER_DUP_ENTRY — дублирующийся уникальный ключ
        // 1048 = ER_BAD_NULL_ERROR — NOT NULL нарушение
        // 1406 = ER_DATA_TOO_LONG — данные слишком длинные

        var msg = ex.Message ?? "";

        switch (ex.Number)
        {
            case 1451:
            {
                var table = ExtractTableFromMessage(msg);
                var humanMsg = table != null
                    ? $"Невозможно удалить: существуют связанные записи в таблице «{table}». Сначала удалите их."
                    : "Невозможно удалить: существуют связанные записи в другой таблице. Сначала удалите их.";
                await WriteError(context, HttpStatusCode.Conflict, humanMsg);
                break;
            }
            case 1452:
            {
                var table = ExtractTableFromMessage(msg);
                var humanMsg = table != null
                    ? $"Невозможно сохранить: запись в таблице «{table}» с указанным ID не существует."
                    : "Невозможно сохранить: связанная запись не найдена.";
                await WriteError(context, HttpStatusCode.Conflict, humanMsg);
                break;
            }
            case 1062:
            {
                var field = ExtractMySqlUniqueField(msg);
                var humanMsg = field != null
                    ? $"Запись с таким значением «{field}» уже существует."
                    : "Запись с такими данными уже существует.";
                await WriteError(context, HttpStatusCode.Conflict, humanMsg);
                break;
            }
            case 1048: 
            {
                var field = ExtractMySqlNotNullField(msg);
                var humanMsg = field != null
                    ? $"Поле «{field}» обязательно для заполнения."
                    : "Одно из обязательных полей не заполнено.";
                await WriteError(context, HttpStatusCode.BadRequest, humanMsg);
                break;
            }
            case 1406:
                await WriteError(context, HttpStatusCode.BadRequest, "Введённые данные слишком длинные.");
                break;
            default:
                await WriteError(context, HttpStatusCode.InternalServerError, "Ошибка базы данных. Попробуйте позже.");
                break;
        }
    }
    private static readonly Dictionary<string, string> TableNames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Works",               "Работы" },
        { "Accruals",            "Начисления" },
        { "Payments",            "Выплаты" },
        { "ProfessionEmployers", "Профессиональные назначения" },
        { "Employers",           "Сотрудники" },
        { "Operations",          "Операции" },
        { "Professions",         "Профессии" },
        { "AccrualsType",        "Типы начислений" },
    };

    private static readonly Dictionary<string, string> FieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "FullName",     "ФИО" },
        { "Phone",        "Телефон" },
        { "Title",        "Название" },
        { "Description",  "Описание" },
        { "RatePerUnit",  "Расценка за единицу" },
        { "WorkDate",     "Дата работы" },
        { "Quantity",     "Количество" },
        { "AccrualTotal", "Сумма начисления" },
        { "AmountToPay",  "Сумма к выдаче" },
        { "Name",         "Название" },
        { "DateOfStart",  "Дата начала" },
        { "Email",        "Email" },
        { "Token",        "Токен" },
    };

    private static string? ExtractTableFromMessage(string message)
    {
        foreach (var kv in TableNames)
            if (message.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                return kv.Value;
        return null;
    }

    private static string? ExtractSqliteUniqueField(string message)
    {
        var idx = message.IndexOf("UNIQUE constraint failed:", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;
        var after = message[(idx + "UNIQUE constraint failed:".Length)..].Trim();
        var dotIdx = after.IndexOf('.');
        var rawField = dotIdx >= 0 ? after[(dotIdx + 1)..].Split(' ')[0] : after.Split(' ')[0];
        return FieldNames.TryGetValue(rawField, out var human) ? human : rawField;
    }

    private static string? ExtractSqliteNotNullField(string message)
    {
        var idx = message.IndexOf("NOT NULL constraint failed:", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;
        var after = message[(idx + "NOT NULL constraint failed:".Length)..].Trim();
        var dotIdx = after.IndexOf('.');
        var rawField = dotIdx >= 0 ? after[(dotIdx + 1)..].Split(' ')[0] : after.Split(' ')[0];
        return FieldNames.TryGetValue(rawField, out var human) ? human : rawField;
    }

    private static string? ExtractMySqlUniqueField(string message)
    {
        foreach (var kv in FieldNames)
            if (message.Contains(kv.Key, StringComparison.OrdinalIgnoreCase))
                return kv.Value;
        return null;
    }
    private static string? ExtractMySqlNotNullField(string message)
    {
        var match = Regex.Match(message, @"Column '(\w+)' cannot be null", RegexOptions.IgnoreCase);
        if (match.Success && FieldNames.TryGetValue(match.Groups[1].Value, out var human))
            return human;
        return null;
    }

    private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message, string? detail = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        var payload = new ApiErrorResponse { Error = message, Detail = detail };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}