using System;
using System.ComponentModel;

public static class EnumExtensions
{
    public static string Description<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (descriptionAttributes != null && descriptionAttributes.Length > 0)
        {
            return descriptionAttributes[0].Description;
        }

        return enumValue.ToString();
    }
}
