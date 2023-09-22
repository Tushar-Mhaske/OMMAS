using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Extensions
{

    public class CommonFilterCollection
    {
        public List<SelectListItem> StateMaster { get; set; }
        public List<SelectListItem> DistrictMaster { get; set; }
        public List<SelectListItem> BlockMaster { get; set; }
        public List<SelectListItem> VillageMaster { get; set; }
        public List<SelectListItem> HabitationMaster { get; set; }
        public List<SelectListItem> StreamMaster { get; set; }
        public List<SelectListItem> BatchMaster { get; set; }
        public List<SelectListItem> YearMaster { get; set; }
        public List<SelectListItem> MonthMaster { get; set; }
        public List<SelectListItem> ProposalTypeMaster { get; set; }
    }


    public class CommonFilterWrapper
    {

        public CommonFilterCollection GetCommonFilters(bool state = false,
                                                        bool district = false,
                                                        bool block = false,
                                                        bool village = false,
                                                        bool habitation = false,
                                                        bool stream = false,
                                                        bool batch = false,
                                                        bool month = false,
                                                        bool year = false,
                                                        bool proposalType = false
                                                        )
        {
            //filters = "state,district,block,village";
            // Use Session variable to get Login Levels

            CommonFilterCollection obj = new CommonFilterCollection();
            //List<SelectListItem> stList = new List<SelectListItem>();
            try
            {
                if (state)
                {
                    // Check User Session Exits or not for Citizen
                    // If Session (Means Application User)
                    // if session[user] = national then         --- Can be used for citizen page
                    //      obj.StateMaster = GetStates("0"); 
                    // else
                    //     obj.StateMaster = GetStates(Specific statecode); 
                    //else (Means Citizen Page)
                    obj.StateMaster = GetStates(0);           // Remove this line afterwards and use above
                }
                else
                {
                    obj.StateMaster = null;
                }


                if (district)
                {
                    // Check User Session Exits or not for Citizen
                    // If Session (Means Application User)
                    // if session[user] = national then 
                    //      obj.StateMaster = GetDistricts(0); 
                    // else if session[user] = state then 
                    //      obj.DistrictMaster = GetDistricts(0, stateCode); 
                    // else if session[user] = district then 
                    //     obj.DistrictMaster = GetDistricts(Specific distcode); 

                    obj.DistrictMaster = GetDistricts(0, 1);       // Remove this line afterwards and use above
                }
                else
                {
                    obj.DistrictMaster = null;
                }


                if (block)
                {
                    // Check User Session Exits or not for Citizen
                    // If Session (Means Application User)
                    // if session[user] = national then 
                    //      obj.BlockMaster = GetBlocks(0, distCode, stateCode); 
                    // else
                    //     obj.BlockMaster = GetBlocks(Specific blockCode); 
                    //else (Means Citizen Page)

                    //Keep Block Code always as 0
                    obj.BlockMaster = GetBlocks(0);
                }
                else
                {
                    obj.BlockMaster = null;
                }


                if (village)
                {
                    //Keep Block Code always as 0
                    obj.VillageMaster = GetVillages(0);
                }
                else
                {
                    obj.VillageMaster = null;
                }


                if (habitation)
                {
                    //Keep Block Code always as 0
                    obj.HabitationMaster = GetHabitations(0);
                }
                else
                {
                    obj.HabitationMaster = null;
                }


                if (stream)
                {
                    obj.StreamMaster = GetStreams();
                }
                else
                {
                    obj.StreamMaster = null;
                }


                if (batch)
                {
                    obj.BatchMaster = GetBatches();
                }
                else
                {
                    obj.BatchMaster = null;
                }


                if (month)
                {
                    obj.MonthMaster = GetMonths();
                }
                else
                {
                    obj.MonthMaster = null;
                }


                if (year)
                {
                    obj.YearMaster = GetYears();
                }
                else
                {
                    obj.YearMaster = null;
                }

                if (proposalType)
                {
                    obj.ProposalTypeMaster = GetProposalTypes();
                }
                else
                {
                    obj.ProposalTypeMaster = null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return obj;
        }


        /// <summary>
        /// If stateCode == 0 then Populate all States 
        /// else populate specific state
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetStates(int stateCode = 0)
        {
            List<SelectListItem> lstStates = new List<SelectListItem>();
            try
            {
                if (stateCode == 0)
                {
                    //lstStates.Add(new SelectListItem() { Selected = false, Text = "Select State", Value = "0" });
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstStates = dbContext.MASTER_STATE.ToList().Where(x => x.MAST_STATE_ACTIVE.Equals("Y")).Select(x => new SelectListItem
                        {
                            Value = x.MAST_STATE_CODE.ToString(),
                            Text = x.MAST_STATE_NAME
                        }).ToList<SelectListItem>();
                    }

                    lstStates.Insert(0, new SelectListItem() { Selected = false, Text = "Select State", Value = "0" });
                }
                else
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstStates = dbContext.MASTER_STATE.ToList().Where(x => x.MAST_STATE_CODE == stateCode).Select(x => new SelectListItem
                        {
                            Value = x.MAST_STATE_CODE.ToString(),
                            Text = x.MAST_STATE_NAME
                        }).ToList<SelectListItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstStates;
        }

        /// <summary>
        /// If districtCode == 0 then Populate all Districts under specific state 
        /// else populate specific District
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDistricts(int districtCode = 0, int stateCode = 0)
        {
            List<SelectListItem> lstDistricts = new List<SelectListItem>();
            try
            {
                if (districtCode == 0 && stateCode == 0)                     // If both zero, only default item will be populated
                {
                    lstDistricts.Add(new SelectListItem() { Selected = false, Text = "Select District", Value = "0" });

                }
                else if (districtCode == 0 && !(stateCode.Equals("0")))             // If state is non zero, district for particular State will be populated
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstDistricts = dbContext.MASTER_DISTRICT.ToList().Where(x => x.MAST_STATE_CODE == stateCode).OrderBy(x => x.MAST_DISTRICT_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_DISTRICT_CODE.ToString(),
                            Text = x.MAST_DISTRICT_NAME
                        }).ToList<SelectListItem>();
                    }

                    lstDistricts.Insert(0, new SelectListItem() { Selected = false, Text = "Select District", Value = "0" });

                }
                else if (districtCode != 0 && !(stateCode.Equals("0")))                                                             // If boths non zero, particular district will be populated
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstDistricts = dbContext.MASTER_DISTRICT.ToList().Where(x => x.MAST_STATE_CODE == stateCode && x.MAST_DISTRICT_CODE == districtCode).OrderBy(x => x.MAST_DISTRICT_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_DISTRICT_CODE.ToString(),
                            Text = x.MAST_DISTRICT_NAME
                        }).ToList<SelectListItem>();
                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstDistricts;
        }

        /// <summary>
        /// If blockCode == 0 then Populate all Blocks under specific state & district 
        /// else populate specific Block
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetBlocks(int blockCode = 0, int districtCode = 0)
        {
            List<SelectListItem> lstBlocks = new List<SelectListItem>();
            try
            {
                if (blockCode == 0 && districtCode == 0)                     // If all zero, only default item will be populated
                {
                    lstBlocks.Add(new SelectListItem() { Selected = false, Text = "Select Block", Value = "0" });

                }
                else if (blockCode == 0 && districtCode != 0)              // If district is non zero, district for particular district will be populated      
                {

                    using (var dbContext = new PMGSYEntities())
                    {
                        lstBlocks = dbContext.MASTER_BLOCK.ToList().Where(x => x.MAST_DISTRICT_CODE == districtCode).OrderBy(x => x.MAST_BLOCK_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_BLOCK_CODE.ToString(),
                            Text = x.MAST_BLOCK_NAME
                        }).ToList<SelectListItem>();
                    }

                    lstBlocks.Insert(0, new SelectListItem() { Selected = false, Text = "Select Block", Value = "0" });

                }
                else if (blockCode != 0 && districtCode != 0)               // If boths non zero, particular block will be populated
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstBlocks = dbContext.MASTER_BLOCK.ToList().Where(x => x.MAST_DISTRICT_CODE == districtCode && x.MAST_BLOCK_CODE == blockCode).OrderBy(x => x.MAST_BLOCK_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_BLOCK_CODE.ToString(),
                            Text = x.MAST_BLOCK_NAME
                        }).ToList<SelectListItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstBlocks;
        }


        /// <summary>
        /// If villageCode == 0 then Populate all Villages under specific state & district & blocks 
        /// else populate specific Village
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="villageCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetVillages(int villageCode = 0, int blockCode = 0)
        {
            List<SelectListItem> lstVillages = new List<SelectListItem>();
            try
            {
                if (villageCode == 0 && blockCode == 0)
                {
                    lstVillages.Add(new SelectListItem() { Selected = false, Text = "Select Village", Value = "0" });

                }
                else if (villageCode == 0 && blockCode != 0)
                {

                    using (var dbContext = new PMGSYEntities())
                    {
                        lstVillages = dbContext.MASTER_VILLAGE.ToList().Where(x => x.MAST_BLOCK_CODE == blockCode).OrderBy(x => x.MAST_VILLAGE_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_VILLAGE_CODE.ToString(),
                            Text = x.MAST_VILLAGE_NAME
                        }).ToList<SelectListItem>();
                    }

                    lstVillages.Insert(0, new SelectListItem() { Selected = false, Text = "Select Village", Value = "0" });

                }
                else if (villageCode != 0 && blockCode != 0)
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstVillages = dbContext.MASTER_VILLAGE.ToList().Where(x => x.MAST_BLOCK_CODE == blockCode && x.MAST_VILLAGE_CODE == villageCode).OrderBy(x => x.MAST_VILLAGE_NAME).Select(x => new SelectListItem
                        {
                            Value = x.MAST_VILLAGE_CODE.ToString(),
                            Text = x.MAST_VILLAGE_NAME
                        }).ToList<SelectListItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return lstVillages;
        }


        /// <summary>
        /// If HabCode == 0 then Populate all Habs under specific state & district & blocks & village 
        /// else populate specific Hab
        /// </summary>
        /// <param name="habCode"></param>
        /// <param name="villageCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetHabitations(int habCode = 0, int villageCode = 0)
        {
            List<SelectListItem> lstHabs = new List<SelectListItem>();
            try
            {
                if (habCode == 0 && villageCode == 0)
                {
                    lstHabs.Add(new SelectListItem() { Selected = false, Text = "Select Habitation", Value = "0" });

                }
                else if (habCode == 0 && villageCode != 0)
                {

                    using (var dbContext = new PMGSYEntities())
                    {
                        lstHabs = dbContext.MASTER_HABITATIONS.ToList().Where(x => x.MAST_VILLAGE_CODE == villageCode).OrderBy
                                                                            (x => x.MAST_HAB_NAME).Select(x => new SelectListItem
                                                                            {
                                                                                Value = x.MAST_HAB_CODE.ToString(),
                                                                                Text = x.MAST_HAB_NAME
                                                                            }).ToList<SelectListItem>();
                    }

                    lstHabs.Insert(0, new SelectListItem() { Selected = false, Text = "Select Habitation", Value = "0" });

                }
                else if (habCode != 0 && villageCode != 0)
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        lstHabs = dbContext.MASTER_HABITATIONS.ToList().Where(x => x.MAST_VILLAGE_CODE == villageCode && x.MAST_HAB_CODE == habCode).OrderBy
                                                                            (x => x.MAST_HAB_NAME).Select(x => new SelectListItem
                                                                            {
                                                                                Value = x.MAST_HAB_CODE.ToString(),
                                                                                Text = x.MAST_HAB_NAME
                                                                            }).ToList<SelectListItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstHabs;
        }



        /// <summary>
        /// Get All Streams, irrespective of conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStreams()
        {
            List<SelectListItem> lstStreams = new List<SelectListItem>();
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    lstStreams = dbContext.MASTER_STREAMS.ToList().Select(x => new SelectListItem
                    {
                        Value = x.MAST_STREAM_CODE.ToString(),
                        Text = x.MAST_STREAM_NAME

                    }).ToList<SelectListItem>();
                }

                lstStreams.Insert(0, new SelectListItem() { Selected = false, Text = "All Streams", Value = "0" });
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstStreams;
        }


        /// <summary>
        /// Get All Batches, irrespective of conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetBatches()
        {
            List<SelectListItem> lstBatch = new List<SelectListItem>();
            try
            {
                //using (var dbContext = new PMGSYEntities())
                //{
                //    lstBatch = dbContext.MASTER_BATCH.ToList().Select(x => new SelectListItem
                //    {
                //        Value = x.BATCH_ID.ToString(),
                //        Text = x.BATCH_VALUE

                //    }).ToList<SelectListItem>();
                //}
                lstBatch.Insert(0, new SelectListItem() { Selected = false, Text = "All Batches", Value = "0" });
            }
            catch (Exception ex)
            {
                return null;
            }

            return lstBatch;
        }


        /// <summary>
        /// Get All Years, irrespective of conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetYears()
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            try
            {

                using (var dbContext = new PMGSYEntities())
                {
                    //lstYears = dbContext.MASTER_YEAR.ToList().Select(x => new SelectListItem
                    //{
                    //    Value = x.YEAR_ID.ToString(),
                    //    Text = x.YEAR_VALUE
                    //}).ToList<SelectListItem>();
                }

                lstYears.Insert(0, new SelectListItem() { Selected = false, Text = "All Years", Value = "-1" });
                lstYears.Insert(0, new SelectListItem() { Selected = false, Text = "Select Year", Value = "0" });

            }
            catch (Exception ex)
            {
                return null;
            }

            return lstYears;
        }


        /// <summary>
        /// Get All Months, irrespective of conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetMonths()
        {
            List<SelectListItem> lstMonths = new List<SelectListItem>();

            try
            {
                //using (var dbContext = new PMGSYEntities())
                //{
                //    lstMonths = dbContext.MASTER_MONTH.ToList().Select(x => new SelectListItem
                //    {
                //        Value = x.MONTH_ID.ToString(),
                //        Text = x.MONTH_NAME
                //    }).ToList<SelectListItem>();
                //}

                lstMonths.Insert(0, new SelectListItem() { Selected = false, Text = "All Months", Value = "-1" });
                lstMonths.Insert(0, new SelectListItem() { Selected = false, Text = "Select Month", Value = "0" });

            }
            catch (Exception ex)
            {
                return null;
            }

            return lstMonths;
        }


        /// <summary>
        /// Get Proposal Types, irrespective of conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetProposalTypes()
        {
            List<SelectListItem> lstPropTypes = new List<SelectListItem>();
            try
            {

                //using (var dbContext = new PMGSYEntities())
                //{
                //    lstPropTypes = dbContext.MASTER_PROPOSAL_TYPE.ToList().Select(x => new SelectListItem
                //    {
                //        Value = x.PROPOSAL_TYPE_ID.ToString(),
                //        Text = x.PROPOSAL_TYPE
                //    }).ToList<SelectListItem>();
                //}

                lstPropTypes.Insert(0, new SelectListItem() { Selected = false, Text = "Both", Value = "0" });

            }
            catch (Exception ex)
            {
                return null;
            }

            return lstPropTypes;
        }

    }//class CommonFilterWrapper Ends here





}