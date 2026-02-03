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

    string pathForCreateUser = "/users/add";
    string expressionForSendByName = @"^/users/byName/.+$";
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
        await SendUsersPage(request, response);
    }

    // Получение пользователя по ID
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

    // Получение пользователя по Имени
    else if (Regex.IsMatch(path, expressionForSendByName) && request.Method == HttpMethods.Get)
    {
        string? targetName = path.Split("/")[3];
        if (targetName is null)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Не удалось распознать Имя пользователя" });
            return;
        }

        await SendUserByName(response, targetName);
    }

    // Создание пользователя
    else if(path == pathForCreateUser && request.Method == HttpMethods.Post)
    {
        await CreateUser(request, response);
    }

    // Изменение пользователя по ID
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == HttpMethods.Put)
    {
        string? idString = path.Split("/")[2];
        if (idString is null)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Не удалось распознать Id" });
            return;
        }

        Guid id = Guid.Parse(idString);
        await UpdateUser(request, response, id);
    }

    // Удаление пользователя по ID
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == HttpMethods.Delete)
    {
        string? idString = path.Split("/")[2];
        if (idString is null)
        {
            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = "Не удалось распознать Id" });
            return;
        }

        Guid id = Guid.Parse(idString);
        await DeleteUser(request, response, id);
    }
});




// === Методы управления ===
async Task SendUsersPage(HttpRequest request, HttpResponse response)
{
    bool sortByName = Convert.ToBoolean(request.Query["name"]);
    bool sortByAge = Convert.ToBoolean(request.Query["age"]);
    int page = Convert.ToInt32(request.Query["page"]);

    User[] result = userManager.GetPage(page, sortByName, sortByAge);

    await response.WriteAsJsonAsync(result);
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
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Не удалось найти пользователя по имени" });
        return;
    }

    await response.WriteAsJsonAsync(targetUser);
}

async Task CreateUser(HttpRequest request, HttpResponse response)
{

    User? newUser = await request.ReadFromJsonAsync<User>();
    if (newUser is null)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Не удалось распознать данные нового пользователя" });
        return;
    }

    userManager.Create(newUser);
    await SendUsersPage(request, response);
}

async Task UpdateUser(HttpRequest request, HttpResponse response, Guid id)
{
    User? updatedUser = await request.ReadFromJsonAsync<User>();
    if (updatedUser is null)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Не удалось распознать данные изменяемого пользователя" });
        return;
    }

    bool result = userManager.Update(id, updatedUser);
    if(result == false)
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь с указанным ID не найден" });
        return;
    }

    await SendUsersPage(request, response);
}

async Task DeleteUser(HttpRequest request, HttpResponse response, Guid id)
{
    bool result = userManager.Delete(id);
    if (result == false)
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь с указанным ID не найден" });
        return;
    }

    await SendUsersPage(request, response);
}

app.Run();
