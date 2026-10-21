# FlowMCP

Скелет локального MCP-сервера для Google Flow на C# / .NET 10.
Запланований спосіб запуску — окремий ASP.NET Core сервер, до якого Codex
підключатиметься через HTTPS і MCP Streamable HTTP.

## Поточний стан

Створено структуру solution, залежності, конфігурацію та базову точку входу.
Сервер має HTTPS-host, MCP endpoint без прикладних tools і перевірку стану
процесу. Браузерна автоматизація, операції, авторизація та зберігання даних
ще не реалізовані.

## Структура

| Проєкт | Призначення |
| --- | --- |
| `src/FlowMCP.Server` | ASP.NET Core host, MCP tools, конфігурація, фонові служби, перевірка доступу |
| `src/FlowMCP.Core` | Сценарії, моделі, стани операцій та інтерфейси |
| `src/FlowMCP.Automation` | Playwright, Chrome-профіль та інтеграція з Google Flow |
| `src/FlowMCP.Infrastructure` | SQLite, файли результатів та діагностика |
| `tests` | Місце для майбутніх тестів |

Залежності між проєктами:

- Server → Core, Automation, Infrastructure.
- Automation → Core.
- Infrastructure → Core.
- Core не має зовнішніх пакетів і залежностей від інших проєктів.

## Залежності

Версії централізовано визначені в `Directory.Packages.props`:

| Пакет | Версія | Проєкт |
| --- | --- | --- |
| [ModelContextProtocol.AspNetCore](https://www.nuget.org/packages/ModelContextProtocol.AspNetCore/2.3.0) | 2.3.0 | Server |
| [Microsoft.Playwright](https://www.nuget.org/packages/Microsoft.Playwright/1.63.0) | 1.63.0 | Automation |
| [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/10.0.13) | 10.0.13 | Infrastructure |

ASP.NET Core надається Web SDK через shared framework.
`packages.lock.json` фіксують повний граф залежностей кожного проєкту.
Браузери Playwright на цьому етапі не встановлюються.

## SDK і збірка

Потрібен .NET SDK 10.0.202 або новіший стабільний SDK гілки 10.0,
згідно з `global.json`.

З кореня репозиторію:

```powershell
dotnet restore FlowMCP.slnx --locked-mode
dotnet build FlowMCP.slnx --no-restore
```

Після навмисної зміни залежностей треба виконати `dotnet restore FlowMCP.slnx`
та зберегти оновлені lock-файли.

## HTTPS

У `appsettings.json` налаштована лише HTTPS-адреса `https://localhost:5123`.
Профіль запуску вмикає середовище Development; адреса задається в Kestrel,
щоб уникнути дублювання налаштувань.

| Маршрут | Призначення |
| --- | --- |
| `/mcp` | MCP Streamable HTTP; прикладні tools ще не додано |
| `/health/live` | Перевірка роботи процесу; не перевіряє Google Flow чи SQLite |

Для локальної розробки використовуватиметься стандартний HTTPS-сертифікат
ASP.NET Core. Перед першим запуском реалізованого сервера перевір або створи
його та додай до довірених:

```powershell
dotnet dev-certs https --check --trust
dotnet dev-certs https --trust
```

Друга команда потрібна, якщо перевірка не знайшла чинний довірений сертифікат.
Сертифікат і його приватний ключ не зберігаються в репозиторії.
Не потрібно вимикати перевірку TLS у MCP-клієнті.

Документація: [HTTPS в ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-10.0).

Після налаштування сертифіката:

```powershell
dotnet run --project src/FlowMCP.Server --no-restore
```

Сервер працює незалежно від Codex. Для завершення натисни `Ctrl+C` у його
терміналі. Підключення MCP-клієнта має використовувати
`https://localhost:5123/mcp`.
