using AuthServer.Core.UnitofWork;
using AuthServer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.UnitofWork
{
    public class UnitofWork : IUnitofWork
    {
        private readonly AuthServerDbContext context;

        public UnitofWork(AuthServerDbContext context)
        {
            this.context = context;
        }
        public void Commit()
        {
           context.SaveChanges();
        }

        public async Task CommitAsync()
        {
          await  context.SaveChangesAsync();
        }
    }
}
