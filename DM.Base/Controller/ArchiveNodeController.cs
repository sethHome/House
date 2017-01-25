using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace DM.Base.Controller
{
    public class ArchiveNodeController : ApiController
    {
        [Dependency]
        public IArchiveNodeService _IArchiveNodeService { get; set; }

        [Route("api/v1/archivenode")]
        [HttpGet]
        public List<ArchiveNodeInfo> GetNodes()
        {
            return _IArchiveNodeService.GetList();
        }

        [Route("api/v1/archivenode")]
        [HttpPost]
        public void AddNodes(ArchiveNodeInfo Node)
        {
            _IArchiveNodeService.AddNode(Node);
        }

        [Route("api/v1/archivenode")]
        [HttpPut]
        public void UpdateNodes(ArchiveNodeInfo Node)
        {
            _IArchiveNodeService.UpdateNode(Node);
        }

        [Route("api/v1/archivenode")]
        [HttpDelete]
        public void DeleteNode(string fonds,string key)
        {
            _IArchiveNodeService.DeleteNode(fonds, key);
        }

        [Route("api/v1/archivenode/disable")]
        [HttpDelete]
        public void DisableNode(string fonds, string key)
        {
            _IArchiveNodeService.DisableANode(fonds, key);
        }

        [Route("api/v1/archivenode/visiable")]
        [HttpDelete]
        public void VisiableNode(string fonds, string key)
        {
            _IArchiveNodeService.VisiableNode(fonds, key);
        }

        [Route("api/v1/archivenode")]
        [Route("api/v1/archivenode/disable")]
        [Route("api/v1/archivenode/visiable")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
