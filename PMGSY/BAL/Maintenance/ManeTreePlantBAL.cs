using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using PMGSY.ViewModel.Maintenance;
using PMGSY.Model.Maintenance;
using PMGSY.DAL.Maintenance;

namespace PMGSY.BAL.Maintenance
{
    public class ManeTreePlantBAL : IManeTreePlantBAL
    {

        IManeTreePlantDAL TreePlantDAL;
        public ManeTreePlantBAL()
        {
            TreePlantDAL = new ManeTreePlantDAL();
        }

        public ManeTreePlantHeaderViewModel GetHeader(int roadId)
        {
            try
            {
                return TreePlantDAL.GetHeader(roadId);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public ManeTreePlantModel GetPlant(int id)
        {
            try
            {
                return TreePlantDAL.Get(id);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public List<ManeTreePlantModel> GetAll(int roadId)
        {
            try
            {
                return TreePlantDAL.GetAll(roadId);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public string Delete(int id)
        {
            try
            {
                return TreePlantDAL.Delete(id);
            }
            catch (Exception ex)
            {
                return "Error Occured on Tree Plant Delete.";
            }
            finally
            {
            }
        }

        public bool Add(ManeTreePlantModel treeModel)
        {
            try
            {
                return TreePlantDAL.Add(treeModel);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
            }
        }

        public string TreePlantVerifyAddBAL(ManeTreePlantVerifyViewModel model)
        {
            try
            {
                return TreePlantDAL.TreePlantVerifyAddDAL(model);
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
            }
        }
    }
}
