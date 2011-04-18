﻿using System;
using System.Configuration;
using N2.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using N2.Edit;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;
using N2.Edit.Workflow;
using N2.Persistence.Proxying;
using NHibernate;
using N2.Definitions.Static;

namespace N2.Tests
{
    public static class TestSupport
    {
        public static void Setup(out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out FakeSessionProvider sessionProvider, out ItemFinder finder, out SchemaExport schemaCreator, out InterceptingProxyFactory proxyFactory, params Type[] itemTypes)
        {
			var participators = new ConfigurationBuilderParticipator[0];
			FakeWebContextWrapper context = new Fakes.FakeWebContextWrapper();
			DatabaseSection config = (DatabaseSection)ConfigurationManager.GetSection("n2/database");
			Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, context, config, participators, itemTypes);
        }

		public static void Setup(out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out FakeSessionProvider sessionProvider, out ItemFinder finder, out SchemaExport schemaCreator, out InterceptingProxyFactory proxyFactory, IWebContext context, DatabaseSection config, ConfigurationBuilderParticipator[] participators, params Type[] itemTypes)
		{
			IDefinitionProvider[] definitionProviders;
			Setup(out definitionProviders, out definitions, out activator, out notifier, out proxyFactory, itemTypes);

			var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
			var configurationBuilder = new ConfigurationBuilder(definitionProviders, new ClassMappingGenerator(), new ThreadContext(), participators, config, connectionStrings);
			var configurationSource = new ConfigurationSource(configurationBuilder);

			sessionProvider = new FakeSessionProvider(configurationSource, new NHInterceptor(proxyFactory, configurationSource, notifier), context);
			sessionProvider.CurrentSession = null;

			finder = new ItemFinder(sessionProvider, new DefinitionMap());

			schemaCreator = new SchemaExport(configurationSource.BuildConfiguration());
		}

		public static IDefinitionManager SetupDefinitions(params Type[] itemTypes)
		{
			IItemNotifier notifier;
			IDefinitionProvider[] definitionProviders;
			IDefinitionManager definitions;
			InterceptingProxyFactory proxyFactory;
			ContentActivator activator;
			Setup(out definitionProviders, out definitions, out activator, out notifier, out proxyFactory, itemTypes);
			return definitions;
		}

		public static void Setup(out IDefinitionProvider[] definitionProviders, out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out InterceptingProxyFactory proxyFactory, params Type[] itemTypes)
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(itemTypes[0].Assembly, itemTypes);

			DefinitionBuilder definitionBuilder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new EngineSection());
			notifier = new ItemNotifier();
			proxyFactory = new InterceptingProxyFactory();
			activator = new ContentActivator(new N2.Edit.Workflow.StateChanger(), notifier, proxyFactory);
			definitionProviders = new IDefinitionProvider[] { new DefinitionProvider(definitionBuilder) };
			definitions = new DefinitionManager(definitionProviders, new ITemplateProvider[0], activator, new StateChanger());
			((DefinitionManager)definitions).Start();
		}

		public static T Stub<T>()
			where T: class
		{
			return Rhino.Mocks.MockRepository.GenerateStub<T>();
		}

        public static void Setup(out N2.Edit.IEditManager editor, out IVersionManager versions, IDefinitionManager definitions, IPersister persister, IItemFinder finder)
        {
            var changer = new N2.Edit.Workflow.StateChanger();
			versions = new VersionManager(persister.Repository, finder, changer, new N2.Configuration.EditSection());
			editor = new EditManager(definitions, persister, versions, new SecurityManager(new ThreadContext(), new EditSection()), null, null, null, changer, new EditableHierarchyBuilder(new SecurityManager(new ThreadContext(), new EditSection()), new EngineSection()), null);
        }

        public static void Setup(out ContentPersister persister, ISessionProvider sessionProvider, N2.Persistence.IRepository<int, ContentItem> itemRepository, INHRepository<int, ContentDetail> linkRepository, ItemFinder finder, SchemaExport schemaCreator)
        {
            persister = new ContentPersister(itemRepository, linkRepository, finder);

            schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
        }

        internal static void Setup(out ContentPersister persister, FakeSessionProvider sessionProvider, ItemFinder finder, SchemaExport schemaCreator)
        {
            IRepository<int, ContentItem> itemRepository = new ContentItemRepository(sessionProvider);
            INHRepository<int, ContentDetail> linkRepository = new NHRepository<int, ContentDetail>(sessionProvider);

            Setup(out persister, sessionProvider, itemRepository, linkRepository, finder, schemaCreator);
        }
    }
}
