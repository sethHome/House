using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// EngineeringNote 接口
    /// </summary>
    public partial interface IEngineeringNoteService    
    {
        List<EngineeringNoteEntity> GetEngineeringNote(int EngineeringID);
        
		PageSource<EngineeringNoteInfo> GetPagedList(PageQueryParam PageParam);

        EngineeringNoteInfo Get(int ID);

		int Add(EngineeringNoteInfo EngineeringNote);

		void Update(int ID,EngineeringNoteEntity EngineeringNote);

		void Delete(int ID); 

		void Delete(string IDs);
    } 
}
