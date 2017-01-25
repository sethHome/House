using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Service
{
    public interface IBorrowService
    {
        Task<int> BorrowArchive(BorrowInfo Info);

        Task<int> BorrowBook(BorrowInfo Info);

        List<MyArchiveBorrowInfo> GetMyBorrowedArchive(int UserID);

        List<MyArchiveBorrowInfo> GetBorrowedArchive(int BorrowID);

        IHttpActionResult DownloadArchiveFiles(int ID);

        bool LowArchiveCanBorrow(int ID);

        bool HighArchiveCanBorrow(int ID);

        void ApproveDone(int ID);

        void GiveBack(int ID);
    }
}
