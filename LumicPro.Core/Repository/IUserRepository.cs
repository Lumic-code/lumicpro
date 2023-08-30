using LumicPro.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumicPro.Core.Repository
{
    public interface IUserRepository
    {
        AppUser AddNew(AppUser entity);
        AppUser GetById(string id);
        AppUser Update(AppUser entity);
        bool Delete(AppUser entity);
        IEnumerable<AppUser> GetAll();
        public bool DeleteAll(List<AppUser> entities);
        IEnumerable<AppUser> paginate(List<AppUser> list, int page, int perpage);
    }
}
