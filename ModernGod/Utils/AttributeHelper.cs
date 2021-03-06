﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public static class AttributeHelper
    {
        public static IEnumerable<Type> GetTypesWithHelpAttribute(Type attribute)
        {
            return GetTypesWithHelpAttribute(Assembly.GetCallingAssembly(), attribute);
        }

        public static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly, Type attribute)
        {
            if(attribute == null)
            {
                yield break;
            }

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(attribute, true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetTypeSubclasses(Type baseClass)
        {
            return GetTypeSubclasses(Assembly.GetCallingAssembly(), baseClass);
        }

        public static IEnumerable<Type> GetTypeSubclasses(Assembly assembly, Type baseClass)
        {
            if (baseClass == null)
                yield break;

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(baseClass) && type != baseClass)
                {
                    yield return type;
                }
            }
        }
    }
}
