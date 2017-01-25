using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Calendar 接口
    /// </summary>
    public partial interface ICalendarService    
    {    
		
		PageSource<CalendarEntity> GetPagedList(PageQueryParam PageParam);

		CalendarEntity Get(int ID);

		int Add(CalendarInfo Calendar);

		void Update(int ID,CalendarEntity Calendar);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
