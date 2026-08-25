# Database Access Techniques with Entity Framework Core

#### TL;DR

Applications interact with databases using a `DbContext`. The `DbSet<T>` property represents table collections and supports core CRUD operations (Create, Read, Update, Delete). LINQ (Language Integrated Query) query and method syntax provide powerful querying capabilities directly within Razor `PageModel` handler methods.

---

### DbContext and DbSet in Razor Pages

Part of Entity Framework Core, the `DbContext` class represents a database connection session and provides an API for querying and saving data.

- `DbContext`: Manages database connections, transactions, and entity tracking.
- `DbSet<TEntity>`: Represents an entity collection (mapping to a database table).

Changes made to entities via `DbSet` are persisted to the database when `SaveChangesAsync()` or `SaveChanges()` is invoked on the `DbContext`.

In Razor Pages, the `DbContext` is injected into `PageModel` constructors via Dependency Injection (DI) and queried inside handler methods (`OnGetAsync()`, `OnPostAsync()`).

---

### Key DbSet Methods

`Find(id)` / `FindAsync(id)`
Finds an entity by its primary key. Returns `null` if no entity is found.

`Add(entity)` / `AddAsync(entity)`
Begins tracking a new entity to insert into the database.

`Update(entity)`
Begins tracking an existing entity to update its column values.

`Remove(entity)`
Begins tracking an entity for deletion.

`SaveChangesAsync()`
Asynchronously executes pending `INSERT`, `UPDATE`, and `DELETE` commands against the database.

---

### LINQ Query Syntax vs. Method Syntax

#### 1. Method Syntax (Popular with Lambda Expressions)

```csharp
// Fetch active engineering staff ordered by last name
var engineers = await _context.Staff
    .Where(s => s.Department == "Engineering" && s.IsActive == 1)
    .OrderBy(s => s.LastName)
    .ToListAsync();
```

#### 2. Query Syntax (SQL-Like)

```csharp
var engineers = await (from s in _context.Staff
                       where s.Department == "Engineering" && s.IsActive == 1
                       orderby s.LastName
                       select s).ToListAsync();
```

---

### Common LINQ Extension Methods

| Extension Method | Description |
|---|---|
| `ToListAsync()` | Executes the query and returns a strongly-typed list. |
| `FirstOrDefaultAsync()` | Returns the first element matching a predicate, or `null` if no element matches. |
| `SingleOrDefaultAsync()` | Returns the single matching element, or throws an exception if multiple match. |
| `CountAsync()` | Returns the total count of elements matching a query condition. |
| `AnyAsync()` | Returns `true` if at least one matching element exists. |

---

### Database-First Approach (Scaffolding Models)

If a SQLite or SQL Server database already exists, EF Core CLI tools can scaffold entity models and a `DbContext` automatically:

```bash
dotnet ef dbcontext scaffold "Data Source=data/staff.db;" Microsoft.EntityFrameworkCore.Sqlite -o Models
```

---

### Best Practices in Razor Pages

1. **Use Async Data Methods**: Always prefer `ToListAsync()`, `FirstOrDefaultAsync()`, and `SaveChangesAsync()` inside async handler methods (`public async Task OnGetAsync()`).
2. **Inject DbContext via DI**: Pass `ApplicationDbContext` into `PageModel` constructors rather than instantiating context instances manually.
3. **Read-Only Queries (`AsNoTracking`)**: For GET handlers rendering views without updating data, append `.AsNoTracking()` to improve query performance.