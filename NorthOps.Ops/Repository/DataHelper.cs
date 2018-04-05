using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Data.Linq;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using NorthOps.Ops.ApiRepository;
using NorthOps.Ops.Models;

namespace NorthOps.Ops.Repository
{
    public class DataHelper
    {
        static UnitOfWork unitOfWork = new UnitOfWork();
        static ApiGenericRepository apiRepo = new ApiGenericRepository();
        const string LargeDatabaseDataContextKey = "DXLargeDatabaseDataContext";
        public static NorthOpsEntities DB
        {
            get
            {
                if (HttpContext.Current.Items[LargeDatabaseDataContextKey] == null)
                    HttpContext.Current.Items[LargeDatabaseDataContextKey] = new NorthOpsEntities();
                return (NorthOpsEntities)HttpContext.Current.Items[LargeDatabaseDataContextKey];
            }
        }

        public static IEnumerable<object> GetLocationById(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out int id))
                return null;
            var res = new ApiGenericRepository().GetFetch<IEnumerable<AddressTownCity>>($"api/maintenance/get-location-by-id/{id}");
            return res;
        }

        public static IEnumerable<object> GetLocationRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {

            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;

            /*var ret = (from address in DB.AddressTownCities
                       where (address.Name).Contains(args.Filter)
                       orderby address.Name
                       select new { Id = address.TownCityId, Name = address.Name }
                ).Skip(skip).Take(take).ToList();*/
            var filter = new FilterLocationModel()
            {
                Skip = skip,
                Take = take,
                AddressName = args.Filter
            };
            var ret = new ApiGenericRepository().PostFetch<IEnumerable<AddressTownCity>>($"api/maintenance/get-location-range", filter);
            return ret;
        }

        public static void UsersCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = apiRepo.GetFetch<DataFilterExpressionModel>($"api/maintenance/users-count/{e.FilterExpression}").Count; 
        }

        public static void UsersData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = new ApiGenericRepository().GetFetch<IEnumerable<User>>($"api/maintenance/users-data/{e.StartDataRowIndex}/{e.DataRowCount}/{e.FilterExpression}");
        }
    }

    internal class FilterLocationModel
    {
        public FilterLocationModel()
        {
        }

        public int Skip { get; set; }
        public int Take { get; set; }
        public string AddressName { get; set; }
    }
    internal class DataFilterExpressionModel
    {
        public int Count { get; set; }

    }
}