using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;

namespace GalleryServerPro.Data
{
	/// <summary>
	/// An abstract class that interacts with a data store using the repository pattern.
	/// </summary>
	/// <typeparam name="C">An instance of <see cref="DbContext" />.</typeparam>
	/// <typeparam name="T">An entity that maps to a row in a database table.</typeparam>
	public abstract class Repository<C, T> : IRepository<T>
		where T : class
		where C : DbContext, new()
	{

		private C _ctx = new C();
		public C Context
		{
			get { return _ctx; }
			set { _ctx = value; }
		}

		public virtual IQueryable<T> All
		{
			get
			{
				return GetAll();
			}
		}

		public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _ctx.Set<T>();
			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}
			return query;
		}

		//public virtual IQueryable<T> GetAll()
		//{
		//  IQueryable<T> query = _entities.Set<T>();
		//  return query;
		//}

		/// <summary>
		/// An extension method on <see cref="IQueryable" /> that enumerates the results of the query. This is equivalent to calling ToList 
		/// without actually creating the list.
		/// </summary>
		public virtual void Load()
		{
			_ctx.Set<T>().Load();
		}

		/// <summary>
		/// Returns <see cref="ObservableCollection&lt;T&gt;" /> that represents entities of the set that are currently being tracked by the context
		/// and have not been marked as Deleted. Accessing the Local property never causes a query to be sent to the database. 
		/// This property is usually used after a query has already been executed.
		/// </summary>
		/// <value>An instance of <see cref="ObservableCollection&lt;T&gt;" /></value>
		/// <remarks>
		/// This property returns an <see cref="ObservableCollection&lt;T&gt;" /> that contains all Unchanged, Modified, and Added objects that are 
		/// currently tracked by the context for the given DbSet. The returned observable collection stays in sync with the underlying DbSet 
		/// collection and the contents of the context. This means that you can modify the observable collection or add/remove entities to/from 
		/// the underlying DbSet collection (that includes adding entities by executing a query) and both collections will be synchronized.
		/// </remarks>
		public ObservableCollection<T> Local
		{
			get
			{
				return _ctx.Set<T>().Local;
			}
		}

		/// <summary>
		/// Uses the primary key value to attempt to find an entity tracked by the context. If the entity is not in the context then a query 
		/// will be executed and evaluated against the data in the data source, and null is returned if the entity is not found in the context 
		/// or in the data source. Note that the Find also returns entities that have been added to the context but have not yet been saved to the database.
		/// </summary>
		/// <param name="keyValues">The values of the primary key for the entity to be found.</param>
		/// <returns>`1.</returns>
		public virtual T Find(params object[] keyValues)
		{
			return _ctx.Set<T>().Find(keyValues);
		}

		public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _ctx.Set<T>().Where(predicate);

			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}

			return query;
		}

		//public virtual IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		//{
		//  IQueryable<T> query = _entities.Set<T>().Where(predicate);
		//  return query;
		//}

		public virtual void Add(T entity)
		{
			_ctx.Set<T>().Add(entity);
		}

		public virtual void Delete(T entity)
		{
			_ctx.Set<T>().Remove(entity);
		}

		public virtual void Attach(T entity)
		{
			_ctx.Set<T>().Attach(entity);
		}

		public virtual void Edit(T entity)
		{
			_ctx.Entry(entity).State = EntityState.Modified;
		}

		public virtual void Upsert(T entity, Func<T, bool> insertExpression)
		{
			if (insertExpression.Invoke(entity))
			{
				Add(entity);
			}
			else
			{
				Edit(entity);
			}
		}

		/// <summary>
		/// Saves this instance. Saves are done in a "client-wins" manner so that if there is a concurrency exception,
		/// the data store values are overwritten with properties of the current entity.
		/// </summary>
		/// <remarks>For more about concurrency models, see
		/// http://blogs.msdn.com/b/adonet/archive/2011/02/03/using-dbcontext-in-ef-feature-ctp5-part-9-optimistic-concurrency-patterns.aspx
		/// </remarks>
		public virtual void Save()
		{
			bool saveFailed;
			var failCount = 0;

			do
			{
				saveFailed = false;
				try
				{
					_ctx.SaveChanges();
				}
				catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
				{
					saveFailed = true;
					failCount++;

					if (failCount > 10)
					{
						throw; // Give up after 10 failures
					}

					// Update original values from the database
					var entry = ex.Entries.Single();
					if (entry.State == EntityState.Modified)
					{
						entry.OriginalValues.SetValues(entry.GetDatabaseValues());
					}
				}

			} while (saveFailed);
		}

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
				if (disposing)
					_ctx.Dispose();

			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}

	public interface IRepository<T> : IDisposable where T : class
	{
		void Upsert(T entity, Func<T, bool> insertExpression);
		//IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
		IQueryable<T> All { get; }
		T Find(params object[] keyValues);
		IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
		//IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
		IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
		void Add(T entity);
		void Delete(T entity);
		void Edit(T entity);
		void Save();
	}
}