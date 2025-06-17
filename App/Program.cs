using App;
using Swashbuckle.AspNetCore.Filters;
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
		// See "Add custom serializer to Swagger in my .Net core API":
		// https://stackoverflow.com/questions/59902076/add-custom-serializer-to-swagger-in-my-net-core-api#answer-64812850
		//options.ExampleFilters();

        List<string> xmlFiles = [.. Directory.GetFiles(AppContext.BaseDirectory,"*.xml",SearchOption.TopDirectoryOnly)];

        if (xmlFiles != null && xmlFiles.Count > 0)
        {
			XDocument fullXmlDoc = new(new XElement(XName.Get("doc")));
			var fullXmlMembers = new XElement(XName.Get("members"));

			if (fullXmlDoc.Root is not null)
			{
				xmlFiles.ForEach(xmlFile =>
				{
					var xDocument = XDocument.Load(xmlFile);
					var assemblyMembers = xDocument.Root?.Element(fullXmlMembers.Name);
					if (assemblyMembers is not null)
					{
						foreach (var assemblymember in assemblyMembers.Elements())
						{
							fullXmlMembers.Add(assemblymember);
						}
					}
				});

				fullXmlDoc.Root.Add(fullXmlMembers);
			}

			var doc = new XPathDocument(fullXmlDoc.CreateReader());
			options.IncludeXmlComments(() => doc, true);
			options.UseAllOfToExtendReferenceSchemas();
			options.AddSchemaFilterInstance(new EnumSchemaFilter(doc));
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
