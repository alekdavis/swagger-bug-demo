using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Xml.XPath;

namespace App;

internal static class Extensions
{
#nullable disable
	private const string EmptyNamespace = "";
	internal static XPathNodeIterator SelectChildren(this XPathNavigator navigator, string name)
	{
		return navigator.SelectChildren(name, EmptyNamespace);
	}

	internal static string GetAttribute(this XPathNavigator navigator, string name)
	{
		return navigator.GetAttribute(name, EmptyNamespace);
	}

	internal static XPathNavigator SelectFirstChild(this XPathNavigator navigator, string name)
	{

		return navigator.SelectChildren(name, EmptyNamespace)
				?.OfType<XPathNavigator>()
				.FirstOrDefault();

	}

	internal static XPathNavigator SelectFirstChildWithAttribute(this XPathNavigator navigator, string childName, string attributeName, string attributeValue)
	{
		return navigator.SelectChildren(childName, EmptyNamespace)
				?.OfType<XPathNavigator>()
				.FirstOrDefault(n => n.GetAttribute(attributeName, EmptyNamespace) == attributeValue);
	}
#nullable enable
}

internal class EnumSchemaFilter : ISchemaFilter
{
	private readonly Dictionary<string, XPathNavigator> xmlDocMembers;
	public EnumSchemaFilter(XPathDocument xmlDoc)
	{
		var members = xmlDoc.CreateNavigator()
		   .SelectFirstChild("doc")
		   ?.SelectFirstChild("members")
		   ?.SelectChildren("member")
		   ?.OfType<XPathNavigator>();

		xmlDocMembers = members?.ToDictionary(memberNode => memberNode.GetAttribute("name")) ?? [];
	}

	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		bool isEnumArgument = (context.Type?.GenericTypeArguments?.Length ?? 0) == 1 && context.Type.GenericTypeArguments.All(b => b.IsEnum);
		var isEnumArray = context.Type.IsArray && context.Type.GetElementType().IsEnum;
		if (context.Type.IsEnum || isEnumArgument || isEnumArray)
		{
			var enumType = (context.Type.IsEnum, isEnumArgument, isEnumArray) switch
			{
				(true, _, _) => context.Type,
				(_, true, _) => context.Type.GenericTypeArguments.First(),
				_ => context.Type.GetElementType()
			};
			StringBuilder stringBuilder = new("<p>Members:</p><ul>");
			foreach (var enumValue in Enum.GetValues(enumType))
			{
				if (enumValue is Enum value)
				{
					if (xmlDocMembers.TryGetValue($"F:{enumType.FullName}.{enumValue}", out var xDocValue))
					{
						var summary = xDocValue.SelectFirstChild("summary");
						stringBuilder.Append($"<li>{value} - {summary}</li>");
					}
				}
			}
			schema.Description = stringBuilder.Append("</ul>").ToString();
		}
	}
}
