﻿using N2.Web;
using NHibernate;

namespace N2.Persistence.NH
{
    /// <summary>
    /// A wrapper class that supports the <see cref="IClosable"/> interface 
    /// which means it will be disposed at the end of the request.
    /// </summary>
    public class StatelessSessionContext : IClosable
    {
        private IStatelessSession session;
        private ISessionProvider provider;

		/// <summary>Gets an existing session or null if no session has been started.</summary>
				public IStatelessSession Session
        {
            get { return session; }
            set { session = value; }
        }

		/// <summary>Gets an existing transaction or null if no transaction is running.</summary>
		public virtual ITransaction Transaction { get; set; }

		public StatelessSessionContext(ISessionProvider provider, IStatelessSession session)
        {
            this.provider = provider;
            this.session = session;
        }

		public System.Data.IDbConnection Connection
		{
			get { return Session.Connection; }
		}

        #region IDisposable Members

        public void Dispose()
        {
            provider.Dispose();
        }

        #endregion
    }
}
