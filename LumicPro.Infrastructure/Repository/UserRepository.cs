using LumicPro.Core.Entities;
using LumicPro.Core.Repository;
using LumicPro.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumicPro.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly LumicProContext _context;

        public UserRepository(LumicProContext context)
        {
            _context = context;
        }
        public AppUser AddNew(AppUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Add(entity);
           var status = _context.SaveChanges();
            if (status > 0)
            {
                return entity;
            }
            return null;
        }

        public AppUser AddNew(AppUser[] entities)
        {
            if (entities.Count() > 0)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _context.AddRange(entities);
            var status = _context.SaveChanges();
            if (status > 0)
            {
                return entities[0];
            }
            return null;
        }

        public bool Delete(AppUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Remove(entity);
            var status = _context.SaveChanges();
            if (status > 0)
            {
                return true;
            }
            return false;
        }

       
        public bool DeleteAll(List<AppUser> entities)
        {
            if (entities.Count() < 1)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _context.RemoveRange(entities);
            var status = _context.SaveChanges();
            if (status > 0)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<AppUser> GetAll()
        {
           return _context.AppUsers.ToList();
        }

        public AppUser GetById(string id)
        {
            return _context.AppUsers.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<AppUser> paginate(List<AppUser> list, int page, int perpage)
        {
            page = page < 1 ? 1 : page;
            perpage = perpage < 1 ? 5 : perpage;

            if (list.Count > 0)
            {
                var paginated = list.Skip((page - 1)  * perpage).Take(perpage);
                return paginated;
            }
            return new List<AppUser>();
        } 

        public AppUser Update(AppUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Update(entity);
            var status = _context.SaveChanges();
            if (status > 0)
            {
                return entity;
            }
            return null;
        }
    }
}
