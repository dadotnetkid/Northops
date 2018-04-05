using DevExpress.Web.Mvc;
using NorthOps.Ops.ApiRepository;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Models.ViewModels
{
    public class UserViewModel
    {
        private ApiGenericRepository apiRepo = new ApiGenericRepository();
        public GridViewModel UserGridViewModel(GridViewColumnState column = null, bool reset = false, GridViewPagerState pager = null, GridViewFilteringState filteringState = null)
        {
            var viewModel = GridViewExtension.GetViewModel("UserGridView");
            if (viewModel == null)
            {
                viewModel = new GridViewModel();
                viewModel.KeyFieldName = "Id";
                viewModel.Columns.Add("Email");
                viewModel.Columns.Add("FullName");
                viewModel.Columns.Add("MemberRoles");
            }


            if (pager != null)
                viewModel.ApplyPagingState(pager);
            if (filteringState != null)
                viewModel.ApplyFilteringState(filteringState);
            if (column != null)
                viewModel.ApplySortingState(column, reset);

            viewModel.ProcessCustomBinding(
            e =>
            {
                e.DataRowCount = apiRepo.GetFetch<int>($"api/maintenance/users-count/{e.FilterExpression}");
            },
            e =>
            {
                e.Data = apiRepo.PostFetch<IEnumerable<User>>($"api/maintenance/users-data/{e.StartDataRowIndex}/{e.DataRowCount}/{e.FilterExpression}", e.State.SortedColumns);

            });
            return viewModel;
        }
    }
}