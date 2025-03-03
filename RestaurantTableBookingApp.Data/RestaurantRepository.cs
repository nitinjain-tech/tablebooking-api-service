using Microsoft.EntityFrameworkCore;
using RestaurantTableBookingApp.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantTableBookingApp.Data
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantTableBookingDbContext _dbContext;

        public RestaurantRepository(RestaurantTableBookingDbContext dbContext) 
        {
            this._dbContext = dbContext;
        }
        public Task<List<RestaurantModel>> GetAllRestaurantsAsync()
        {
            var restaurants =  _dbContext.Restaurants
                .OrderBy(x => x.Name)
                .Select(r=> new RestaurantModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                     Address = r.Address,   
                     Phone = r.Phone,
                     Email = r.Email,
                     ImageUrl = r.ImageUrl,
                }).ToListAsync();

            return restaurants;
        }

        public async Task<IEnumerable<DiningTableWithTimeSlotsModel>> GetDiningTablesByBranchAsync(int branchId, DateTime date)
        {
            var diningTables = await _dbContext.DiningTables
             .Where(dt => dt.RestaurantBranchId == branchId)
             .SelectMany(dt => dt.TimeSlots, (dt, ts) => new
             {
                 dt.RestaurantBranchId,
                 dt.TableName,
                 dt.Capacity,
                 ts.ReservationDay,
                 ts.MealType,
                 ts.TableStatus,
                 ts.Id
             })
             .Where(ts => ts.ReservationDay.Date == date.Date)
             .OrderBy(ts => ts.Id)
             .ThenBy(ts => ts.MealType)
             .ToListAsync();

            return diningTables.Select(dt => new DiningTableWithTimeSlotsModel
            {
                BranchId = dt.RestaurantBranchId,
                ReservationDay = dt.ReservationDay.Date,
                TableName = dt.TableName,
                Capacity = dt.Capacity,
                MealType = dt.MealType,
                TableStatus = dt.TableStatus,
                TimeSlotId = dt.Id
            });
        }

        public async Task<IEnumerable<DiningTableWithTimeSlotsModel>> GetDiningTablesByBranchAsync(int branchId)
        {
            var data = await(
                from rb in _dbContext.RestaurantBranches
                join dt in _dbContext.DiningTables on rb.Id equals dt.RestaurantBranchId
                join ts in _dbContext.TimeSlots on dt.Id equals ts.DiningTableId
                where dt.RestaurantBranchId == branchId && ts.ReservationDay >= DateTime.Now.Date
                orderby ts.Id, ts.MealType
                select new DiningTableWithTimeSlotsModel()
                {
                    BranchId = rb.Id,
                    Capacity = dt.Capacity,
                    TableName = dt.TableName,
                    MealType = ts.MealType,
                    ReservationDay = ts.ReservationDay,
                    TableStatus = ts.TableStatus,
                    TimeSlotId = ts.Id,
                    UserEmailId = (from r in _dbContext.Reservations
                                   join u in _dbContext.Users on r.UserId equals u.Id
                                   where r.TimeSlotId == ts.Id
                                   select u.Email.ToLower()).FirstOrDefault()
                }).ToListAsync();


            return data;
        }

        public async Task<IEnumerable<RestaurantBranchModel>> GetRestaurantBranchsByRestaurantIdAsync(int restaurantId)
        {
            var branches = await _dbContext.RestaurantBranches
                .Where(rb => rb.RestaurantId == restaurantId)
                .Select(rb => new RestaurantBranchModel
                {
                    Id = rb.Id,
                    RestaurantId = rb.RestaurantId,
                    Name = rb.Name,
                    Address = rb.Address,
                    Phone = rb.Phone,
                    Email = rb.Email,
                    ImageUrl = rb.ImageUrl
                })
                .ToListAsync();

            return branches;
        }
    }
}
