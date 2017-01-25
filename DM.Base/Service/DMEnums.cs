using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public enum UserTaskStatus : int
    {
        下达 = 1,
        完成 = 2
    }

    public enum UserTaskSource : int
    {
        档案借阅 = 1,
        图书借阅 = 2,
    }

    public enum BorrowStatus : int
    {
        申请中 = 1,
        审核通过 = 2,
        审核失败 = 3,
        归还 = 4
    }

    public enum ArchiveLogType : int
    {
        系统 = 1,
        用户 = 2

    }

    public enum BookType : int
    {
        图书 = 1,
        期刊 = 2,
        杂志 = 3
    }

    public enum Press : int
    {
        人民出版社 = 1,
        春风出版社 = 2
    }


    public enum BookStatus : int
    {
        正常 = 1,
        借出 = 2,
        销毁 = 3
    }
}
