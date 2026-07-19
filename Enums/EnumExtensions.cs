using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;


namespace Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description
                        ?? value.ToString();
        }
    }
}
