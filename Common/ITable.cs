using System.Collections.Generic;

namespace MyAPP.Common
{
    public interface ITable<T>
    {
        List<T> Get();              
        T? Get(int id);             
        void Post(T entity);        
        void Put(int id, T entity); 
        void Delete(int id);   
    }
}