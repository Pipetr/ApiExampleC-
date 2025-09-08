
namespace ApiExampleC_;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99M },
            new Product { Id = 2, Name = "Smartphone", Category = "Electronics", Price = 699.99M },
            new Product { Id = 3, Name = "Desk Chair", Category = "Furniture", Price = 89.99M },
            new Product { Id = 4, Name = "Book: C# Programming", Category = "Books", Price = 29.99M }
        };

        app.MapGet("/api/products", async (HttpContext context) =>
        {
            string? category = context.Request.Query["category"];
            decimal? maxPrice = null;
            if (decimal.TryParse(context.Request.Query["maxPrice"], out var price))
            {
                maxPrice = price;
            }

            var query = products.AsEnumerable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // LINQ
            var results = query.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
            }).ToList();

            if (results.Count == 0)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("No products found with the specified criteria");
            }
            else
            {
                await context.Response.WriteAsJsonAsync(results);
            }
        });

        app.MapGet("/api/error", () =>
        {
            return Results.Problem("An unexpected error ocurred on the server.", statusCode: StatusCodes.Status500InternalServerError);
        });
        app.Run();
    } 
        
}
