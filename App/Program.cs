using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen
(
    options =>
    {
        List<string> xmlFiles = [.. Directory.GetFiles(AppContext.BaseDirectory,"*.xml",SearchOption.TopDirectoryOnly)];

        if (xmlFiles != null && xmlFiles.Count > 0)
        {
            xmlFiles.ForEach(xmlFile =>
            {
                XDocument xmlDoc = XDocument.Load(xmlFile);
                options.IncludeXmlComments(() => new XPathDocument(xmlDoc.CreateReader()), true);
                options.UseAllOfToExtendReferenceSchemas();
             });
        }
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
