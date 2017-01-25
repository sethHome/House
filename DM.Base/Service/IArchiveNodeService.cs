using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IArchiveNodeService
    {
        List<ArchiveNodeInfo> GetList();

        List<ArchiveNodeInfo> GetArchiveNodes(string Fonds, string ArchiveType);

        void AddNode(ArchiveNodeInfo Node);

        void UpdateNode(ArchiveNodeInfo Node);

        void DeleteNode(string FondsNumber, string FullKey);

        void DisableANode(string FondsNumber, string FullKey);

        void VisiableNode(string FondsNumber, string FullKey);

        void DeleteNodeByArchiveType(string FondsNumber, string ArchiveType);
        void DisableANodeByArchiveType(string FondsNumber, string ArchiveType);
        void VisiableNodeByArchiveType(string FondsNumber, string ArchiveType);
    }
}
