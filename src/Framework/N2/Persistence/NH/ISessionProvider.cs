﻿using System;
using NHibernate;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Creates and provides access to the NHibernate session in the current 
    /// request's scope.
    /// </summary>
	public interface ISessionProvider: IDisposable
	{
        /// <summary>Returns an already opened session or creates and opens a new one and puts it in the current request.</summary>
        /// <returns>A NHibernate session.</returns>
		SessionContext OpenSession { get; }

		/// <summary>Returns an already opened stateless session or creates and opens a new one and puts it in the current request.</summary>
		/// <returns>A NHibernate session.</returns>
		StatelessSessionContext OpenStatelessSession { get; }

		/// <summary>Returns an already opened session or creates and opens a new one and puts it in the current request.</summary>
		/// <returns>A NHibernate session.</returns>
		ISessionFactory SessionFactory { get; }

    	/// <summary>Persists changes to disk.</summary>
		void Flush();

		/// <summary>Begins a transaction.</summary>
		/// <returns>A disposable transaction wrapper. Call Commit to commit the transaction.</returns>
		ITransaction BeginTransaction();

		/// <summary>Gets an existing transaction or null if no transaction is running.</summary>
		/// <returns>A disposable transaction wrapper.</returns>
		ITransaction GetTransaction();
	}

}
