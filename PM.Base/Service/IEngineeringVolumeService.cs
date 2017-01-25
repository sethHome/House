using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolume 接口
    /// </summary>
    public partial interface IEngineeringVolumeService
    {

        PageSource<EngineeringVolumeInfo> GetVolumePlanPagedList(PageQueryParam PageParam);

        PageSource<EngineeringVolumeInfo> GetVolumeProcssPageList(PageQueryParam PageParam);

        EngineeringVolumeEntity Get(int ID);

        Task<int> Create(int UserID, EngineeringVolumeNewInfo EngineeringVolume);

        void Update(int ID, EngineeringVolumeNewInfo EngineeringVolume);

        void Delete(int ID);

        /// <summary>
        /// 获取专业的卷册列表
        /// </summary>
        /// <param name="EngineeringID"></param>
        /// <param name="SpecialtyID"></param>
        /// <returns></returns>
        List<EngineeringVolumeInfo> GetSpecialtyVolumes(int EngineeringID, long SpecialtyID);
        List<BizObject> GetSpecialtyVolumesV2(int EngineeringID, long SpecialtyID, bool WithTasks = false);
        
        Task<List<EngineeringVolumeEntity>> BatchUpdate(int UserID, int EngineeringID, long SpecialtyID, List<EngineeringVolumeNewInfo> Entitys);

        void Delete(string IDs);

        EngineeringVolumeEntity Finish(int VolumeID);

        bool IsAllVolumeDone(int EngineeringID,long SpecialtyID);

        List<SysAttachFileEntity> GetVolumeFiles(int ID);

        VolumeStatisticsInfo GetVolumeStatisticsInfo(int ID);

        EngineeringVolumeInfo GetVolumeInfo(int TaskID);
    }
}
