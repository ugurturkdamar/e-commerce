using e_Ticaret.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Ticaret.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        protected static DatabaseContext context;
        private static object _lockSync = new object();

        protected RepositoryBase()
        {
            CreateContext();
        }

        private static void CreateContext() // singleton
        {
            if (context == null)
            {
                lock (_lockSync)
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }
                }
            }
        }
    }
}
