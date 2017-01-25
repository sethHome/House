using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Car 接口
    /// </summary>
    public partial interface ICarService    
    {    
		
		PageSource<CarInfo> GetPagedList(PageQueryParam PageParam);

        List<CarEntity> GetMyUsedCar(int UserID);

        CarEntity Get(int ID);

		int Add(CarInfo Car);

		void Update(int ID,CarEntity Car);

        void ChangeStatus(int ID, CarStatus Status);

        void Delete(int ID); 

		void Delete(string IDs);
    } 
}
