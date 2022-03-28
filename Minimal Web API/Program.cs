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


var documentEditorHelper = new DocumentEditorHelper();

/// <summary>
/// Loads a default Word document.
/// </summary>
app.MapGet("/loadDefault", () =>
{

    return documentEditorHelper.LoadDefault();
});
/// <summary>
/// Converts the clipboard data (HTML/RTF) to SFDT format
/// </summary>
app.MapPost("/SystemClipboard", (CustomParameter param) =>
{
    return documentEditorHelper.SystemClipboard(param);
});
/// <summary>
/// Extracts the content from the document (DOCX, DOC, WordML, RTF, HTML) and converts it into SFDT
/// </summary>
app.MapPost("/Import", (HttpRequest request) =>
{

    return documentEditorHelper.Import(request.Form);

});
/// <summary>
/// Generates the hash for protecting a Word document.
/// </summary>
app.MapPost("/RestrictEditing", (CustomRestrictParameter param) =>
{
    return documentEditorHelper.RestrictEditing(param);
});
/// <summary>
/// Performs the spell check validation for words
/// </summary>
app.MapPost("/SpellCheck", (SpellCheckJsonData spellChecker) =>
{
    return documentEditorHelper.SpellCheck(spellChecker);
});
/// <summary>
/// Performs spell check validation page by page while loading the documents in Document editor control.
/// </summary>
app.MapPost("/SpellCheckByPage", (SpellCheckJsonData spellChecker) =>
{
    return documentEditorHelper.SpellCheckByPage(spellChecker);
});
/// <summary>
/// Performs mail merge in a Word document
/// </summary>
app.MapPost("/MailMerge", (ExportData exportData) =>
{
    return documentEditorHelper.MailMerge(exportData);
});
/// <summary>
/// Saves the document in server from SFDT
/// </summary>
app.MapPost("/Save", (SaveParameter data) =>
{
   documentEditorHelper.Save(data);
});
/// <summary>
/// Convert SFDT string to required format and returns the document as FileStreamResult
/// </summary>
app.MapPost("/ExportSFDT", (SaveParameter data) =>
{
    Microsoft.AspNetCore.Mvc.FileStreamResult fileStream = documentEditorHelper.ExportSFDT(data);
    return Results.File(fileStream.FileStream, fileStream.ContentType, fileStream.FileDownloadName);
});
/// <summary>
/// Converts the DOCX document to required format and returns the document as FileStreamResult to client-side
/// </summary>
app.MapPost("/Export", (HttpRequest request) =>
{
    Microsoft.AspNetCore.Mvc.FileStreamResult? fileStream = documentEditorHelper.Export(request.Form);
    if (fileStream != null)
    {
        return Results.File(fileStream.FileStream, fileStream.ContentType, fileStream.FileDownloadName);
    }
    return null;

});
/// <summary>
/// Loads the specified template document that is already stored in the server.
/// </summary>
app.MapPost("/LoadDocument", (UploadDocument uploadDocument) =>
{
    return documentEditorHelper.LoadDocument(uploadDocument);
});

app.Run("https://localhost:8000/api/documenteditor");