using BG.Model.Core;
using System.Linq.Expressions;

namespace BG.Serv.Core
{
    public partial class MechanicServices : IMechanicServices
    {
        private readonly ICoreData _coreData;
        private IMechanicRepository Repository { get { return _coreData.MechanicRepository; } }

        //* Ctor
        public MechanicServices(ICoreData coreData)
        {
            //Console.WriteLine("MechanicServices.ctor");
            _coreData = coreData;
        }

        public async Task<Mechanic?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<Mechanic?> GetByID(int pkID, bool doLoadLists)
        {
            //try
            //{
                Mechanic? mechanic = await this.Repository.GetByID(pkID, doLoadLists);

                return mechanic;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<List<Mechanic>> GetAll()
        {
            //try
            //{
                List<Mechanic> mechanics = await Repository.GetAll();

                return mechanics;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<Mechanic>> Search(string dataToSearch)
        {
            //try
            //{
            List<Mechanic> mechanics = await Repository.Search(dataToSearch);

            return mechanics;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<Mechanic> Insert(Mechanic mechanic)
        {
            //try
            //{
            Mechanic? createdMechanic = await this.Repository.Insert(mechanic);
            mechanic.MechanicID = createdMechanic.MechanicID;

            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return mechanic;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// If the model is not stored at the database, the return value will be -1; > 0 otherwise.
        /// </summary>
        /// <param name="mechanic"></param>
        /// <returns></returns>
        public async Task<int> Update(Mechanic mechanic)
        {
            //try
            //{
            int pkid  = await this.Repository.Update(mechanic);

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
        public async Task<List<Mechanic>> Search(Expression<Func<Mechanic, bool>> wherePredicate)   //* parameter used in EF
        {
            return await this.Repository.Search(wherePredicate);
        }

        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;
    }
}