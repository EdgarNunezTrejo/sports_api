# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run (dev)
dotnet run

# Run tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~MyTestClass"

# Add a NuGet package
dotnet add package <PackageName>
```

## Architecture

.NET 9 Web API using the controller pattern (`--use-controllers`). Entry point is `Program.cs`, which wires up services and middleware using the minimal hosting model. Controllers live in `Controllers/` and inherit from `ControllerBase` with `[ApiController]` + `[Route("[controller]")]`.

OpenAPI is enabled via `Microsoft.AspNetCore.OpenApi` and served only in Development (`app.MapOpenApi()`).

## Conventions (from dotnet-best-practices skill)

- **DI**: Use primary constructor syntax — `public class MyService(IDependency dep)` — not field-assigned constructors.
- **Async**: All I/O methods must be `async Task`/`async Task<T>`; use `ConfigureAwait(false)` in library code.
- **Interfaces**: Prefix with `I` (e.g., `IPlayerService`). Register with appropriate lifetime (Singleton/Scoped/Transient).
- **Namespaces**: Follow `sports_api.{Feature}` structure (e.g., `sports_api.Players`).
- **Config**: Bind settings via strongly-typed classes with data annotations; avoid reading `IConfiguration` directly in business logic.
- **Testing**: MSTest + FluentAssertions + Moq. AAA pattern. Cover both success and null/failure paths.
- **Logging**: Structured logging via `ILogger<T>`; include meaningful scope context.
- **XML docs**: Required on all public types, methods, and properties.
