using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PMGSY.Common
{
    public static class Extensions
    {


        public static void DeleteMany<T>(this DbSet<T> dbset, List<T> ListToDelete, Models.PMGSYEntities dbContext = null) where T : class
        {
            foreach (var item in ListToDelete)
            {
                //if(dbContext != null)
                //dbContext.Entry(item).State = System.Data.EntityState.Deleted;   
                dbset.Remove(item);
            }
        }
    }
}