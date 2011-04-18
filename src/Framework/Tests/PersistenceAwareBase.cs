using System.Data;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using N2.Engine;
using N2.Installation;
using N2.Persistence.NH;
using N2.Security;
using System.Configuration;
using N2.Engine.MediumTrust;
using N2.Definitions;

namespace N2.Tests
{
	public abstract class PersistenceAwareBase : ItemTestsBase
	{
		protected IEngine engine;
		protected SchemaExport schemaCreator;
		protected FakeSessionProvider sessionProvider;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			engine = CreateEngine();
			
			var configurationBuilder = engine.Resolve<IConfigurationBuilder>();
			sessionProvider = (FakeSessionProvider)engine.Resolve<ISessionProvider>();
			schemaCreator = new SchemaExport(configurationBuilder.BuildConfiguration());
			CreateDatabaseSchema();

			engine.Initialize();
		}

		protected virtual ContentEngine CreateEngine()
		{
			return new ContentEngine();
		}

		[TearDown]
		public override void TearDown()
		{
			sessionProvider.CloseConnections();
			base.TearDown();
		}

		protected virtual ContentPersister GetNHibernatePersistenceManager()
		{
			return engine.Persister as ContentPersister;
		}

		protected virtual void CreateDatabaseSchema()
		{
			schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
		}

		protected virtual void DropDatabaseSchema()
		{
			schemaCreator.Execute(false, true, true, sessionProvider.OpenSession.Session.Connection, null);
		}
	}
}