using CarDealership.Models.Queries;
using CarDealership.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data.Interfaces
{
    public interface IModelRepository
    {
        IEnumerable<Model> GetAll();
        IEnumerable<ModelByMakeId> GetByMakeId(int makeId); // ModelByMakeId gets MakeName additionally
        IEnumerable<ModelReport> GetReports(); // get models formatted properly to use in admin viewmodel
        void Insert(Model model);
    }
}
