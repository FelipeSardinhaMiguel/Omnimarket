using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Omnimarket.Api.Utils;

public static class EnumExtensions
{
    /*public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? value.ToString();
    }*/

    public static string GetDisplayName(Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null)
            return enumValue.ToString();

        var attr = field.GetCustomAttribute<DisplayAttribute>();
        return string.IsNullOrWhiteSpace(attr?.Name) ? enumValue.ToString() : attr.Name!;
    }
}
