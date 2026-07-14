# DriverAPI

<p align="center">

![.NET](https://img.shields.io/badge/.NET-9-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=for-the-badge&logo=dotnet)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![MariaDB](https://img.shields.io/badge/MariaDB-003545?style=for-the-badge&logo=mariadb)
![JWT](https://img.shields.io/badge/JWT-Authentication-black?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-success?style=for-the-badge)

</p>

---

##  О проекте

**DriverAPI** — модульный REST API, разработанный на **ASP.NET Core**, поддерживающий несколько драйверов баз данных и позволяющий переключаться между ними без изменения бизнес-логики приложения.

Основной целью проекта являлось создание универсальной архитектуры backend-приложения, пригодной для дальнейшего масштабирования и расширения.

---

#  Возможности

- REST API
- ASP.NET Core
- JWT Authentication
- Access Token
- Refresh Token
- SQLite
- MariaDB
- Swagger
- Driver Pattern
- Dependency Injection
- SOLID
- Middleware
- JSON Configuration
- Масштабируемая архитектура

---

#  Архитектура

```
                  Controllers
                       │
                       ▼
                  Services
                       │
                       ▼
                      DAO
                       │
        ┌──────────────┴──────────────┐
        ▼                             ▼
 SQLite Driver                 MariaDB Driver
        │                             │
        └──────────────┬──────────────┘
                       ▼
                   Database
```

Главная идея проекта — отделить бизнес-логику от конкретной реализации базы данных.

Добавление нового драйвера требует только реализации соответствующих интерфейсов.

---

#  Структура проекта

```
DriverAPI
│
├── Controllers
├── Middleware
├── Models
├── Services
├── Singletons
├── Drivers
│   ├── SQLite
│   └── MariaDB
├── Config
├── Common
└── Program.cs
```

---

#  Поддерживаемые базы данных

В настоящий момент поддерживаются:

- SQLite
- MariaDB

Используемая СУБД выбирается в конфигурации.

```json
{
    "DBType": "SQLite"
}
```

или

```json
{
    "DBType": "MariaDB"
}
```

Никаких изменений бизнес-логики приложения при этом не требуется.

---

#  Авторизация

В проекте реализована полноценная JWT-аутентификация.

Используются:

- Access Token
- Refresh Token

После успешной авторизации клиент получает Access Token для доступа к защищённым маршрутам.

---

# ⚙ Быстрый запуск

## Клонировать проект

```bash
git clone https://github.com/xXEgor4EXx/DriverAPI.git
```

## Перейти в директорию

```bash
cd DriverAPI
```

## Запустить проект

Открыть решение в Visual Studio или Rider.

После запуска автоматически станет доступен Swagger.

---

#  Тестовая учётная запись

Для удобства проверки проекта в репозитории специально оставлена тестовая база данных.

Это позволяет сразу протестировать API и Frontend без необходимости создавать пользователя вручную.

### Email

```
test@test.tt
```

### Пароль

```
123456
```

Используя данный аккаунт можно проверить:

- авторизацию;
- получение JWT;
- защищённые маршруты;
- административную панель;
- взаимодействие Frontend с API.

---

#  Используемые технологии

### Backend

- C#
- .NET
- ASP.NET Core
- REST API
- JWT
- Dependency Injection

### Database

- SQLite
- MariaDB
- SQL

### Other

- Swagger
- JSON
- HTTP
- Git

---

#  API

Проект содержит Swagger UI.

После запуска открыть:

```
https://localhost:XXXX/swagger
```

Через Swagger можно протестировать все методы API.

---

#  Планы развития

- Добавление новых драйверов БД
- Поддержка PostgreSQL
- Redis Cache
- Unit Tests
- Docker
- CI/CD
- Оптимизация производительности
- Расширение административной панели

---

#  Почему появился этот проект

Проект создавался как собственная практика разработки архитектуры backend-приложений.

Основной задачей было не просто реализовать REST API, а создать основу, которую можно масштабировать и использовать с различными СУБД без переписывания бизнес-логики.

Во время разработки особое внимание уделялось:

- архитектуре приложения;
- разделению ответственности;
- модульности;
- расширяемости;
- читаемости кода.

---

# Что можно посмотреть

- Driver Pattern

- Работа нескольких драйверов БД

- JWT Authentication

- REST API

- Работа с несколькими СУБД

- Архитектура приложения

- Dependency Injection

- Конфигурация через JSON

---

#  Скриншоты

### Swagger

> *(будет добавлен)*

### Административная панель

> *(будет добавлена)*

### Архитектура

> *(будет добавлена)*

---

#  Автор

**Егор Сухарев**

GitHub:

https://github.com/xXEgor4EXx

Если проект оказался полезен — поставьте Star.
