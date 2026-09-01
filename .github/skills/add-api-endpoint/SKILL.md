---
name: add-api-endpoint
description: Scaffold a new REST API endpoint in KC.LMS.Server following repo conventions (controller, DTOs, storage abstraction, HTTP samples, changelog).
---

# Skill: Add API Endpoint

Use this skill when adding a new REST endpoint to KC.LMS.Server.

## Steps

1. **Controller** — Create or extend a controller in `KC.LMS.Server/Controllers/`:
   - Attribute routing: `[ApiController]`, `[Route("[controller]")]`.
   - Inject dependencies (e.g., `ILogger<T>`, storage services) via constructor.
   - Every action is `async`, returns `ActionResult<T>`, and accepts a `CancellationToken`.

2. **DTOs** — Define request/response models as C# records in the Server project (or shared models in `KC.LMS.Storage` if persisted).

3. **Storage** — Any data access goes through an interface + implementation in `KC.LMS.Storage`. Register it in `Program.cs` DI.

4. **HTTP samples** — Add example requests to `KC.LMS.Server/KC.LMS.Server.http`.

5. **Changelog** — Add an entry to `KC.LMS.Server/CHANGELOG.md`.

6. **Validate** — Run `dotnet build KC.LMS.slnx`.

## Template

```csharp
namespace KC.LMS.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController(ICourseStore store, ILogger<CoursesController> logger) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetAll(CancellationToken cancellationToken)
	{
		var courses = await store.GetAllAsync(cancellationToken);
		return Ok(courses);
	}
}
```
