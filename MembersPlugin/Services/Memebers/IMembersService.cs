using ClosedXML.Excel;
using MembersPlugin.Models;

namespace MembersPlugin.Services.Memebers
{
    public interface IMembersService
    {
        CustomMember GetAllMembers(int pageIndex, int pageSize);
        XLWorkbook ExecuteMemberToExcel();
    }
}