using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository;
using System.Linq.Expressions;
namespace Samwel.DataAccess.Repository.IRepository; 

public class Repository<T> : IRepository<T> where T : class 
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> dbSet;
    public Repository(AppDbContext _context)
    {
        this._context = _context;  
        this.dbSet = _context.Set<T>();
    }
    public void Add(T entity)
    {
        dbSet.Add(entity);   
        _context.SaveChanges();
    }

    public T? Get(Expression<Func<T, bool>> filer , string? include = null , bool tracked = false)
    {
        IQueryable<T> Query;
        if (tracked)
        {
             Query = dbSet ;
        }
        else
        {
            Query = dbSet.AsNoTracking();
        }
        Query = Query.Where(filer);
        if (!string.IsNullOrEmpty(include))
        {
            foreach (var prop in include.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                Query = Query.Include(prop);
            }
        }
        return Query.FirstOrDefault();
    }
    //ctrl shift f 
    public IEnumerable<T> GetAll(Expression<Func<T, bool>> ?filer, string ? include = null)
    {
        IQueryable<T> Query = dbSet.AsNoTracking();
        if (filer != null)
        {
            Query = Query.Where(filer);
      
        }
        if (!string.IsNullOrEmpty(include))
        {
            foreach (var prop in include.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                Query = Query.Include(prop);
            }
        }

        return Query.ToList();  
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
        _context.SaveChanges();
    }

    public void RemoveRange(IEnumerable<T> values)
    {
        dbSet.RemoveRange(values);
    }
}
