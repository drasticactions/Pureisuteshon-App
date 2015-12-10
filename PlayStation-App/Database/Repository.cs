using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync.Extensions;

namespace PlayStation_App.Database
{
    public class Repository<T> where T : class, new()
    {
        private readonly SQLiteAsyncConnection _db;
        public Repository(SQLiteAsyncConnection db)
        {
            this._db = db;
        }

        public AsyncTableQuery<T> Items => _db.Table<T>();

        public async Task<List<T>> GetAllWithChildren()
        {
            return await _db.GetAllWithChildrenAsync<T>();
        }

        public async Task<int> Create(T newEntity)
        {
            return await _db.InsertAsync(newEntity);
        }

        public async Task<int> CreateAll(List<T> newEntity)
        {
            return await _db.InsertAllAsync(newEntity);
        }

        public async Task CreateWithChildren(T newEntity)
        {
            await _db.InsertWithChildrenAsync(newEntity);
        }

        public async Task RemoveAll()
        {
            await _db.DeleteAllAsync<T>();
        }

        public async Task Remove(T newEntity)
        {
            await _db.DeleteAsync(newEntity);
        }

        public async Task<int> Update(T entity)
        {
            return await _db.UpdateAsync(entity);
        }

        public async Task UpdateWithChildren(T newEntity)
        {
            await _db.UpdateWithChildrenAsync(newEntity);
        }

        public async Task<int> Delete(T entity)
        {
            return await _db.DeleteAsync(entity);
        }
    }
}
