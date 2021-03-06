﻿namespace ABV_Invest.Common.Mapping
{
    using ABV_Invest.Common.Mapping.Contracts;
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AutoMapperConfig
    {
        private static bool initialised;

        public static void RegisterMappings(params Assembly[] assemblies)
        {
            if (initialised)
            {
                return;
            }

            initialised = true;

            var types = assemblies.SelectMany(a => a.GetExportedTypes()).ToList();
            Mapper.Initialize(configuration =>
            {
                // IMapFrom<>
                foreach (var map in GetFromMaps(types))
                {
                    configuration.CreateMap(map.Source, map.Destination);
                }

                // ICustomMappings
                foreach (var map in GetCustomMappings(types))
                {
                    map.CreateMappings(configuration);
                }
            });
        }

        private static IEnumerable<TypesMap> GetFromMaps(
            IEnumerable<Type> types)
        {
            var fromMaps = from t in types
                           from i in t.GetTypeInfo().GetInterfaces()
                           where i.GetTypeInfo().IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                                 !t.GetTypeInfo().IsAbstract &&
                                 !t.GetTypeInfo().IsInterface
                           select new TypesMap
                           {
                               Source = i.GetTypeInfo().GetGenericArguments()[0],
                               Destination = t,
                           };

            return fromMaps;
        }

        private static IEnumerable<ICustomMap> GetCustomMappings(IEnumerable<Type> types)
        {
            var customMaps = from t in types
                             from i in t.GetTypeInfo().GetInterfaces()
                             where typeof(ICustomMap).GetTypeInfo().IsAssignableFrom(t) &&
                                   !t.GetTypeInfo().IsAbstract &&
                                   !t.GetTypeInfo().IsInterface
                             select (ICustomMap)Activator.CreateInstance(t);

            return customMaps;
        }

        private class TypesMap
        {
            public Type Source { get; set; }

            public Type Destination { get; set; }
        }
    }
}