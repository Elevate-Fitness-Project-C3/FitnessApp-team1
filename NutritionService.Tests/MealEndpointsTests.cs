using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class MealEndpointsTests : IClassFixture<NutritionWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly NutritionWebApplicationFactory _factory;

    public MealEndpointsTests(NutritionWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateAuthenticatedClient(Guid userId, string role = "User")
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestTokens.Create(userId, role: role));
        return client;
    }

    [Fact]
    public async Task MutatingEndpoints_WithoutAuthorization_Return401Unauthorized()
    {
        _factory.ResetDatabase();
        var mealId = Guid.NewGuid();
        var client = _factory.CreateClient();

        var postResponse = await client.PostAsJsonAsync(
            "/api/nutrition/meals",
            new CreateMealRequest("Oatmeal", MealType.Breakfast, 350, 12));

        var putResponse = await client.PutAsJsonAsync(
            $"/api/nutrition/meals/{mealId}",
            new UpdateMealRequest("Updated Oatmeal", MealType.Breakfast, 400, 15));

        var deleteResponse = await client.DeleteAsync($"/api/nutrition/meals/{mealId}");

        Assert.Equal(HttpStatusCode.Unauthorized, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, putResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task MutatingEndpoints_AsRegularUser_Return403ForbiddenAndLeaveDatabaseUnchanged()
    {
        _factory.ResetDatabase();
        var existingMeal = Meal.Create("Grilled Chicken Salad", MealType.Lunch, 450, 42, DateTime.UtcNow);
        _factory.Seed(existingMeal);

        var client = CreateAuthenticatedClient(Guid.NewGuid(), role: "User");

        var postResponse = await client.PostAsJsonAsync(
            "/api/nutrition/meals",
            new CreateMealRequest("Protein Shake", MealType.Snack, 220, 30));

        var putResponse = await client.PutAsJsonAsync(
            $"/api/nutrition/meals/{existingMeal.Id}",
            new UpdateMealRequest("Mutated Name", MealType.Dinner, 999, 99));

        var deleteResponse = await client.DeleteAsync($"/api/nutrition/meals/{existingMeal.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);

        using var db = _factory.CreateContext();
        var meals = db.Meals.ToList();
        Assert.Single(meals);
        var persisted = meals[0];
        Assert.Equal(existingMeal.Id, persisted.Id);
        Assert.Equal("Grilled Chicken Salad", persisted.Name);
        Assert.Equal(MealType.Lunch, persisted.Type);
        Assert.Equal(450, persisted.Calories);
        Assert.Equal(42, persisted.ProteinGrams);
        Assert.True(persisted.IsActive);
        Assert.Null(persisted.UpdatedAtUtc);
    }

    [Fact]
    public async Task MutatingEndpoints_AsAdmin_SucceedAndPersistChanges()
    {
        _factory.ResetDatabase();
        var client = CreateAuthenticatedClient(Guid.NewGuid(), role: "Admin");

        // Create (POST)
        var createRequest = new CreateMealRequest("Salmon Bowl", MealType.Dinner, 620, 46);
        var createResponse = await client.PostAsJsonAsync("/api/nutrition/meals", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdPayload = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var createdId = createdPayload.GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, createdId);
        Assert.Contains($"/api/nutrition/meals/{createdId}", createResponse.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);

        // Update (PUT)
        var updateRequest = new UpdateMealRequest("Grilled Salmon Bowl", MealType.Lunch, 580, 48);
        var updateResponse = await client.PutAsJsonAsync($"/api/nutrition/meals/{createdId}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using (var db = _factory.CreateContext())
        {
            var updated = db.Meals.Single(m => m.Id == createdId);
            Assert.Equal("Grilled Salmon Bowl", updated.Name);
            Assert.Equal(MealType.Lunch, updated.Type);
            Assert.Equal(580, updated.Calories);
            Assert.Equal(48, updated.ProteinGrams);
            Assert.True(updated.IsActive);
            Assert.NotNull(updated.UpdatedAtUtc);
        }

        // Soft-deactivate (DELETE)
        var deleteResponse = await client.DeleteAsync($"/api/nutrition/meals/{createdId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using (var db = _factory.CreateContext())
        {
            var deactivated = db.Meals.Single(m => m.Id == createdId);
            Assert.False(deactivated.IsActive);
        }
    }

    [Fact]
    public async Task MealDiscovery_ExcludesDeactivatedMealsAndSupportsSearchAndTypeFilters()
    {
        _factory.ResetDatabase();
        var now = DateTime.UtcNow;

        var activeChickenLunch = Meal.Create("Chicken Quinoa Bowl", MealType.Lunch, 520, 44, now);
        var activeChickenDinner = Meal.Create("Roasted Chicken Breast", MealType.Dinner, 480, 50, now);
        var activeOatsBreakfast = Meal.Create("Protein Oats", MealType.Breakfast, 390, 24, now);
        var toBeDeactivated = Meal.Create("Chicken Caesar Wrap", MealType.Lunch, 510, 35, now);

        var preDeactivated = Meal.Create("Inactive Chicken Lunch", MealType.Lunch, 400, 30, now);
        preDeactivated.Deactivate(now);

        _factory.Seed(activeChickenLunch, activeChickenDinner, activeOatsBreakfast, toBeDeactivated, preDeactivated);

        // Soft-deactivate via DELETE endpoint as Admin
        var adminClient = CreateAuthenticatedClient(Guid.NewGuid(), role: "Admin");
        var deleteResponse = await adminClient.DeleteAsync($"/api/nutrition/meals/{toBeDeactivated.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var userClient = CreateAuthenticatedClient(Guid.NewGuid(), role: "User");

        // Browse all active meals
        var browseResponse = await userClient.GetAsync("/api/nutrition/meals");
        Assert.Equal(HttpStatusCode.OK, browseResponse.StatusCode);
        var allActive = await browseResponse.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(allActive);
        Assert.Equal(3, allActive.TotalCount);
        Assert.Equal(3, allActive.Items.Count);
        Assert.DoesNotContain(allActive.Items, m => m.Id == preDeactivated.Id);
        Assert.DoesNotContain(allActive.Items, m => m.Id == toBeDeactivated.Id);

        // GetById returns 200 for active, 404 for pre-deactivated and endpoint-deactivated
        var activeGetResponse = await userClient.GetAsync($"/api/nutrition/meals/{activeChickenLunch.Id}");
        Assert.Equal(HttpStatusCode.OK, activeGetResponse.StatusCode);
        var activeDto = await activeGetResponse.Content.ReadFromJsonAsync<MealDto>(JsonOptions);
        Assert.NotNull(activeDto);
        Assert.Equal(activeChickenLunch.Id, activeDto.Id);

        var preDeactivatedGetResponse = await userClient.GetAsync($"/api/nutrition/meals/{preDeactivated.Id}");
        Assert.Equal(HttpStatusCode.NotFound, preDeactivatedGetResponse.StatusCode);

        var softDeactivatedGetResponse = await userClient.GetAsync($"/api/nutrition/meals/{toBeDeactivated.Id}");
        Assert.Equal(HttpStatusCode.NotFound, softDeactivatedGetResponse.StatusCode);

        // Search filter (?search=Chicken) returns only matching active meals
        var searchResponse = await userClient.GetAsync("/api/nutrition/meals?search=Chicken");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
        var searchBody = await searchResponse.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(searchBody);
        Assert.Equal(2, searchBody.TotalCount);
        Assert.All(searchBody.Items, m => Assert.Contains("Chicken", m.Name, StringComparison.OrdinalIgnoreCase));

        // Type filter (?type=Lunch) returns only matching active meals
        var typeResponse = await userClient.GetAsync("/api/nutrition/meals?type=Lunch");
        Assert.Equal(HttpStatusCode.OK, typeResponse.StatusCode);
        var typeBody = await typeResponse.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(typeBody);
        Assert.Equal(1, typeBody.TotalCount);
        Assert.Single(typeBody.Items);
        Assert.Equal(activeChickenLunch.Id, typeBody.Items[0].Id);
    }

    [Fact]
    public async Task BrowseMeals_PaginatesActiveMealsCorrectly()
    {
        _factory.ResetDatabase();
        var now = DateTime.UtcNow;

        var activeMeals = new[]
        {
            Meal.Create("Meal Alpha", MealType.Breakfast, 300, 20, now),
            Meal.Create("Meal Bravo", MealType.Lunch, 400, 30, now),
            Meal.Create("Meal Charlie", MealType.Dinner, 500, 40, now),
            Meal.Create("Meal Delta", MealType.Snack, 200, 15, now),
            Meal.Create("Meal Echo", MealType.Lunch, 450, 35, now)
        };
        var inactiveMeal = Meal.Create("Meal Inactive", MealType.Dinner, 600, 50, now);
        inactiveMeal.Deactivate(now);

        _factory.Seed([.. activeMeals, inactiveMeal]);

        var client = CreateAuthenticatedClient(Guid.NewGuid(), role: "User");

        // Page 1
        var page1Response = await client.GetAsync("/api/nutrition/meals?page=1&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);
        var page1 = await page1Response.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(page1);
        Assert.Equal(5, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(1, page1.Page);
        Assert.Equal(2, page1.PageSize);

        // Page 2
        var page2Response = await client.GetAsync("/api/nutrition/meals?page=2&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page2Response.StatusCode);
        var page2 = await page2Response.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(page2);
        Assert.Equal(5, page2.TotalCount);
        Assert.Equal(2, page2.Items.Count);
        Assert.Equal(2, page2.Page);
        Assert.Equal(2, page2.PageSize);

        var page1Ids = page1.Items.Select(m => m.Id).ToHashSet();
        var page2Ids = page2.Items.Select(m => m.Id).ToHashSet();
        Assert.Empty(page1Ids.Intersect(page2Ids));

        // Page 3
        var page3Response = await client.GetAsync("/api/nutrition/meals?page=3&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, page3Response.StatusCode);
        var page3 = await page3Response.Content.ReadFromJsonAsync<PaginatedMealsResponse>(JsonOptions);
        Assert.NotNull(page3);
        Assert.Equal(5, page3.TotalCount);
        Assert.Single(page3.Items);
        Assert.Equal(3, page3.Page);
        Assert.Equal(2, page3.PageSize);
    }
}
