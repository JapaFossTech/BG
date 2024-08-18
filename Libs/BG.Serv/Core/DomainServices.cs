using BG.Model.Core;
using System.Linq.Expressions;
using Infrastructure.Extensions;

namespace BG.Serv.Core
{
    public partial class DomainServices : IDomainServices
    {
        private readonly ICoreData _coreData;
        private IDomainRepository Repository { get { return _coreData.DomainRepository; } }

        //* Ctor
        public DomainServices(ICoreData coreData)
        {
            //Console.WriteLine("DomainServices.ctor");
            _coreData = coreData;
        }

        public async Task<Domain?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<Domain?> GetByID(int pkID, bool doLoadLists)
        {
            //try
            //{
            Domain? domain = await this.Repository.GetByID(pkID, doLoadLists);

            return domain;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<List<Domain>> GetAll()
        {
            //try
            //{
            List<Domain> domains = await Repository.GetAll();

            return domains;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<Domain>> GetAll_Paged_Sorted_andFiltered(
            int pageIndex = 0, int pageSize = 20
            , string? sortColumn = null, string sortOrder = "ASC"
            , string? filterQuery = null)
        {
            //try
            //{
            List<Domain> domains = await Repository
                .GetAll_Paged_Sorted_andFiltered(
                    pageIndex, pageSize, sortColumn, sortOrder, filterQuery);

            return domains;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<int> GetAllRecordCount()
        {
            //try
            //{
            int count = await Repository.GetAllRecordCount();

            return count;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<Domain>> Search(string dataToSearch)
        {
            //try
            //{
            List<Domain> domains = await Repository.Search(dataToSearch);

            return domains;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<Domain> Insert(Domain domain)
        {
            //try
            //{
            Domain? createdDomain = await this.Repository.Insert(domain);
            domain.DomainID = createdDomain.DomainID;

            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return domain;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// If the model is not stored at the database, the return value will be -1; > 0 otherwise.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task<int> Update(Domain domain)
        {
            //try
            //{
            int pkid = await this.Repository.Update(domain);

            return pkid;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<bool> Delete(int pkID)
        {
            //try
            //{
            bool isDeleted = await this.Repository.Delete(pkID);

            this.Deleted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return isDeleted;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<Domain>> Search(Expression<Func<Domain, bool>> wherePredicate)   //* parameter used in EF
        {
            return await this.Repository.Search(wherePredicate);
        }

        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;
    }
}