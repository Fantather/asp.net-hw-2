using asp.net_hw_2.UserManager;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

UserManager userManager = new UserManager();

app.Run(async (HttpContext context) =>
{
    HttpRequest request = context.Request;
    HttpResponse response = context.Response;

    string? path = request.Path.Value;
    string expressionForName = @"^/users/byName/*+$";
    string expressionForGuid = @"^/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

    // Проверка
    if(path is null)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Не удалось получить запрос" });
        return;
    }

    // Парсинг
    if (path == "/users" && request.Method == HttpMethods.Get)
    {
        await SendAllUsers(response);
    }

    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == HttpMethods.Get)
    {
        string? idString = path.Split("/")[2];
        if(idString is null)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Не удалось распознать Id" });
            return;
        }

        Guid id = Guid.Parse(idString);
        await SendUserById(response, id);
    }

    else if (Regex.IsMatch(path, expressionForName) && request.Method == HttpMethods.Get)
    {
        string? idString = path.Split("/")[2];
        if (idString is null)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Не удалось распознать Id" });
            return;
        }

        Guid id = Guid.Parse(idString);
        await SendUserById(response, id);
    }

    else if (path == "/users" && request.Method == HttpMethods.Post)
    {

    }
});

async Task SendAllUsers(HttpResponse response)
{
    await response.WriteAsJsonAsync(userManager.GetAll());
}

async Task SendUserById(HttpResponse response, Guid id)
{
    User? targetUser = userManager.GetById(id);
    if(targetUser is null)
    {
        await response.WriteAsJsonAsync(new { message = "Не удалось найти пользователя с указанным id" });
        return;
    }

    await response.WriteAsJsonAsync(targetUser);
}

async Task SendUserByName(HttpResponse response, string targetName)
{
    User? targetUser = userManager.GetByName(targetName);
    if(targetUser is null)
    {
        await response.WriteAsJsonAsync(new { message = "Не удалось найти пользователя по имени" });
        return;
    }

    await response.WriteAsJsonAsync(targetUser);
}

async Task CreateUser(HttpResponse response, string name, string city, int age)
{
    User user = new User(name, city, age);
    userManager.Create(user);
    await SendAllUsers(response);
}

async Task UpdateUser(HttpResponse response, Guid id, string newName, string newCity, int newAge)
{
    userManager.Update(id, new User(newName, newCity, newAge));
    await SendAllUsers(response);
}

async Task DeleteUser(HttpResponse response, Guid id)
{
    userManager.Delete(id);
    await SendAllUsers(response);
}

app.Run();
