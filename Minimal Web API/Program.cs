using DocumentEditorCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
builder.Services.AddMvcCore()
        .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var documentEditor = new DocumentEditor();
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapGet("/loadDefault", () =>
{

    return documentEditor.LoadDefault();
});
app.MapPost("/SystemClipboard", (CustomParameter param) =>
{
    return documentEditor.SystemClipboard(param);
});
app.MapPost("/Import", (HttpRequest request) =>
{

    return documentEditor.Import(request.Form);

});
app.MapPost("/RestrictEditing", (CustomRestrictParameter param) =>
{
    return documentEditor.RestrictEditing(param);
});
app.MapPost("/SpellCheck", (SpellCheckJsonData spellChecker) =>
{
    return documentEditor.SpellCheck(spellChecker);
});
app.MapPost("/SpellCheckByPage", (SpellCheckJsonData spellChecker) =>
{
    return documentEditor.SpellCheckByPage(spellChecker);
});
app.MapPost("/MailMerge", (ExportData exportData) =>
{
    return documentEditor.MailMerge(exportData);
});
app.MapPost("/Save", (SaveParameter data) =>
{
   documentEditor.Save(data);
});
app.MapPost("/ExportSFDT", (SaveParameter data) =>
{
    Microsoft.AspNetCore.Mvc.FileStreamResult fileStream = documentEditor.ExportSFDT(data);
    return Results.File(fileStream.FileStream, fileStream.ContentType, fileStream.FileDownloadName);
});
app.MapPost("/Export", (HttpRequest request) =>
{
    Microsoft.AspNetCore.Mvc.FileStreamResult? fileStream = documentEditor.Export(request.Form);
    if (fileStream != null)
    {
        return Results.File(fileStream.FileStream, fileStream.ContentType, fileStream.FileDownloadName);
    }
    return null;

});
app.MapPost("/LoadDocument", (UploadDocument uploadDocument) =>
{
    return documentEditor.LoadDocument(uploadDocument);
});

app.Run("https://localhost:8000/api/documenteditor");

public class CustomParameters 
{
    public IFormCollection? data { get; set; }
}

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

