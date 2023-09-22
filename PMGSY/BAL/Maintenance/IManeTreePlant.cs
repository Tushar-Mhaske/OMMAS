using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMGSY.Model.Maintenance;
//using PMGSY.ViewModel.Maintenance;

namespace PMGSY.BAL.Maintenance
{
    public interface IManeTreePlantBAL
    {
        ManeTreePlantHeaderViewModel GetHeader(int roadId);
        ManeTreePlantModel  GetPlant(int id);
        List<ManeTreePlantModel> GetAll(int roadId);
        string Delete(int id);
        bool Add(ManeTreePlantModel treeModel);

        string TreePlantVerifyAddBAL(ManeTreePlantVerifyViewModel treeModel);
    }
}

