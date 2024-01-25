using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IRepository <T> where T :  class  
    {
        // T means any class model in your app 

        IEnumerable<T> GetAll (Expression<Func<T, bool>> ?filer , string? include = null );
        T ? Get(Expression<Func<T, bool>> filer , string? include = null , bool tracked = false);

        void Add(T entity);
  
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> values);
        
        // We not Add Update (T)
        // Because the Logic for update
        // not Generic for all  Entitey type 
    }
}
