using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Castle.DynamicProxy;
using N2.Definitions.Static;
using N2.Engine;
using N2.Plugin;
using Remotion.Linq.Utilities;

namespace N2.CrossLinks
{
    public class DynamicTypeMapRepository
    {
        private ModuleBuilder moduleBuilder;
        private Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();

        protected virtual ModuleBuilder CreateModuleBuilder()
        {
            var typeFinder = Context.Current.Resolve<ITypeFinder>();
            var types = typeFinder.Find(typeof(ContentItem));

            ModuleBuilder moduleBuilder;
            using (var assemblyPersister = AssemblyPersister.Create())
            {
                foreach (var linkedType in MapTypes(types))
                {
                    typeMap[linkedType.LinkedType] = assemblyPersister.CreateType(
                        linkedType.LinkType,
                        linkedType.LinkedType.Name + "Link");
                }

                //return assemblyPersister.ModuleBuilder;
                moduleBuilder = assemblyPersister.ModuleBuilder;
            }

            foreach (var type in typeMap.Keys.ToList())
            {
                typeMap[type] = Type.GetType(typeMap[type].AssemblyQualifiedName);
            }

            return moduleBuilder;
        }

        public Dictionary<Type, Type> TypeMap
        {
            get
            {
                // Initialize modulebuilder (ugly)
                var moduleBuilder = ModuleBuilder;

                return typeMap;
            }
        }

        public Assembly Assembly
        {
            get { return ModuleBuilder.Assembly; }
        }

        public ModuleBuilder ModuleBuilder
        {
            get { return moduleBuilder ?? (moduleBuilder = CreateModuleBuilder()); }
        }

        public Type Get<T>()
        {
            return Get(typeof(T));
        }

        public Type Get(Type linkedType)
        {
            if (typeMap.ContainsKey(linkedType))
            {
                return typeMap[linkedType];
            }

            throw new ArgumentException(string.Format("{0} has no corresponding link type. Did you add the EditableCrossLinksAttribute to the IList<{0}> property?", linkedType.Name));
        }

        public virtual IEnumerable<MappedType> MapTypes(IEnumerable<Type> types)
        {
            return types.Select(type =>
                                new MappedType
                                    {
                                        LinkedType = type,
                                        LinkType = type
                                    });
        }

        public struct MappedType
        {
            public Type LinkedType;
            public Type LinkType;
        }
    }

    public class AssemblyPersister : IDisposable
    {
        private const string AssemblyName = "CrossLinkTypes";

        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;

        private AssemblyPersister()
        {
            var assemblyName = new AssemblyName { Name = AssemblyName };
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            const string moduleName = AssemblyName + ".DynamicTypes";
            moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll");
        }

        public ModuleBuilder ModuleBuilder
        {
            get { return moduleBuilder; }
        }

        public virtual Type CreateType(Type baseType, string typeName)
        {
            var crossLinkTypeBuilder = moduleBuilder.DefineType(typeName,
                                                                TypeAttributes.Public |
                                                                TypeAttributes.Class |
                                                                TypeAttributes.AutoClass |
                                                                TypeAttributes.AnsiClass |
                                                                TypeAttributes.BeforeFieldInit |
                                                                TypeAttributes.AutoLayout,
                                                                baseType);

            var attributeConstructor = typeof(PartDefinitionAttribute).GetConstructor(new Type[] { });
            if (attributeConstructor == null) throw new NullReferenceException(String.Format("Constructor for {0} was not found", typeof(PartDefinitionAttribute).Name));

            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { });

            crossLinkTypeBuilder.SetCustomAttribute(attributeBuilder);

            return crossLinkTypeBuilder.CreateType();
        }

        public void Dispose()
        {
            assemblyBuilder.Save(AssemblyName + ".dll");
        }

        public static AssemblyPersister Create()
        {
            return new AssemblyPersister();
        }
    }
}