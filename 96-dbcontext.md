# Database Access Techniques

#### TL;DR

Extracting data can be done various ways. Applications need a `DbContext` for database queries. The `DbSet` property of a `DbContext` allows for core CRUDing (Creating, Reading, Updating, Deleting). Extension methods provide a nice shorthand for most situations. LINQ (Language Integrated Query) query and method syntax give a full range of query options.

### DbContext

Part of the Entity Framework Core, the `DbContext` class represents a session with a database and provides an API for working with the database.

The `DbContext` class has methods for Adding, Modifying and Deleting data. For extracting (querying) data, use the `DbSet` property of `DbContext`.

Although Adding, Modifying and Deleting can be done directly via the `DbContext` it is most commonly done in Web Applications via the `DbSet` class to raise awareness of the table been affected.

Part of the Entity Framework Core, the `DbSet` class represents a collection for a given entity.

Various `DbSet` methods can be used to add, modifiy, delete and query the data via manipulation of the entity.

Whether data is manipulated directly with `DbContext` or via `DbSet`, the changes are applied through the `SaveChanges()` method of the `DbContext`.

### Dbset

The DbSet class represents an entity set that can be used for CRUDing (Creating, Reading, Updating, Deleting) operations.

Core methods of the DbSet class are:

`Find(int)`

Uses the primary key to find an entity and return it. A `null` value is returned if no entity found or the entity is not in context.

`Update(entity)`

Begins tracking the given entity to update with the values provided. No changes applied to the database until the `SaveChanges()` is called.

`Remove(entity)`

Begins tracking the given entity to remove it from the database. No changes applied to the database until the `SaveChanges()` is called.

`SaveChanges()`

Saves all changes made in this context to the database.

### LINQ Extension Methods

LINQ (Language Integrated Query) extension methods cover standard query operations.

List and examples [here](https://www.entityframeworktutorial.net/querying-entity-graph-in-entity-framework.aspx)

`First()`

Returns the first element of a sequence.

`FirstOrDefault()`

Returns the first element of a sequence, or a default value if no element is found.

`Single()`

Returns the only element of a sequence that satisfies a specified condition.

`SingleOrDefault()`

Returns the only element of a sequence, or a default value if the sequence is empty.

`ToList()`

Returns a strongly typed list of objects.

`Count()`

Returns the number of elements in a sequence.

`Min()`

Returns the minimum value in a sequence of values.

`Max()`

Returns the minimum value in a sequence of values.

`Last()`

Returns the last element of a sequence.

`LastOrDefault()`

Returns the last element of a sequence, or a default value if no element is found.

`Average()`

Computes the average of a sequence of numeric values.

These are derived from `Dbset` which itself is derived from `IQueryable` which itself is derived from `IEnumerable` which is where all these [methods](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=netcore-3.1#methods) can be found.

### LINQ Query Syntax

Similar to SQL Query but with a construct of from something In someCollection Select something

Consider the basic SQL `SELECT` of:

SELECT \* FROM Films

With LINQ Query Syntax this would be written as:

LINQ Query Syntax

var myFilms = from f in \_db.Films select f;

As with SQL `from` is used to indicate the collection / table to query. In the above `f` is a range variable. The `select` indicates the range variable (the something) to be extracted.

Between the `from` and `select` we can add a `where` clause.

In SQL a `WHERE` would appear as:

SELECT \* FROM Films WHERE FilmCertificate = '12'

With LINQ Query Syntax this would be written as:

LINQ Query Syntax

var myFilms = from f in \_db.Films where f.FilmCertificate == '12' select f;

Wildcard 'contains' SQL queries appear as:

SELECT \* FROM Films WHERE FilmTitle LIKE '%star%';

In Linq the `Contains` keyword removes the need for the wildcards `%..%` and can be written as:

LINQ Query Syntax

films = from f in \_db.Films where f.FilmTitle.Contains("star") select f;

Results can also be ordered for example:

LINQ Query Syntax

var filmsByPrice = from f in \_db.Films where f.FilmPrice > 2 orderby f.FilmPrice select f;

### LINQ Method Syntax

LINQ Method Syntax uses Lambda expressions to define the condition for the query. This shorthand is popular for simple queries but is harder to understand for with more complex logic.

With this techniqe a SQL query of:

SELECT \* FROM Films WHERE FilmTitle LIKE '%star%';

... can be written as:

LINQ Method Syntax

films = films.WHERE(f => f.FilmTitle.Contains("star"));

Note: A Lambda Expression is syntactic sugar (as with Javascript Arrow Functions) for a function or method with the 'good to go' arrow `=>` operator.

### Database-First approach / Reverse Engineer the Models

A database constructed first can be reverse engineered into Models.

Use the console to run the `Scaffold-DbContext` command.

Tools > nuget Package Manager > Package Management Console

An example reverse engineering a database of `MyCourses` would be:

Scaffold-DbContext "Server=(localdb)\\MSSQLLocalDB;Database=MyCourses;Trusted\_Connection=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models

Specific tables can be targeted with the `-Tables` flag.

Scaffold-DbContext "Server=(localdb)\\MSSQLLocalDB;Database=MyCourses;Trusted\_Connection=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Tables MyTable

See the [Microsoft Documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/scaffolding?tabs=dotnet-core-cli) for more details.

### Resources

The [Query Gallery](mvc-query-gallery.php) provides examples based on the tutorial's `Films` database.

See also:

*   [Different Ways to Write LINQ Queries](https://dotnettutorials.net/lesson/different-ways-to-write-linq-query/)
*   [MS Docs on LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/introduction-to-linq-queries)
*   [Learn Entity Framework Core](https://www.learnentityframeworkcore.com/dbset)
*   [Entity Framework Extension Methods](https://www.entityframeworktutorial.net/querying-entity-graph-in-entity-framework.aspx)