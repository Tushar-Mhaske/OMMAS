using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMGSY.Model.Maintenance;
//using PMGSY.ViewModel.Maintenance;

namespace PMGSY.DAL.Maintenance
{
    public interface IManeTreePlantDAL
    {
        bool Add(ManeTreePlantModel treePlant);
        string Delete(int id);
        void Edite(ManeTreePlantModel treePlant);
        ManeTreePlantModel Get(int id);
        ManeTreePlantHeaderViewModel GetHeader(int id);
        List<ManeTreePlantModel> GetAll(int roadId);

        string TreePlantVerifyAddDAL(ManeTreePlantVerifyViewModel model);
    }
}
