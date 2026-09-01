---
applyTo: "**/*.cs"
---

# C# Coding Rules (KC.LMS)

- Target .NET 10; use modern C# features (primary constructors, collection expressions) where they improve clarity.
- File-scoped namespaces only.
- Nullable reference types are enabled — never suppress with `!` unless justified with a comment.
- Async methods must accept a `CancellationToken` and pass it through to downstream calls.
- Use `ILogger<T>` constructor injection for logging; no `Console.WriteLine`.
- Prefer records for DTOs and immutable models.
- Keep controllers thin: validation + delegation. Business/data logic goes in services or `KC.LMS.Storage`.
