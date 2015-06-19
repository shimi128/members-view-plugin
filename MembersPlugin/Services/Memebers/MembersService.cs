using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using MembersPlugin.Models;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace MembersPlugin.Services.Memebers
{
    public class MembersService : IMembersService
    {
        private readonly IMemberService _memberService;
        private readonly string[] _umbracoMembersConvention = {"umbracoMemberPasswordRetrievalAnswer","umbracoMemberPasswordRetrievalQuestion",
                                                          "umbracoMemberComments","umbracoMemberFailedPasswordAttempts","umbracoMemberApproved",
                                                          "umbracoMemberLockedOut","umbracoMemberLastLogin","umbracoMemberLastLockoutDate","umbracoMemberLastPasswordChangeDate"};
        public MembersService()
        {
            _memberService = ApplicationContext.Current.Services.MemberService;
        }


        public CustomMember GetAllMembers(int pageIndex, int pageSize)
        {
            int totalRecords;
            return new CustomMember { Members = Map(_memberService.GetAll(pageIndex, pageSize, out totalRecords)), PageIndex = pageIndex, PageSize = pageSize, TotlalRecords = totalRecords, PropertiesList = MemberProp(_memberService.GetAll(pageIndex, pageSize, out totalRecords)) };
        }

        private IEnumerable<Member> Map(IEnumerable<IMember> members)
        {
            return members.Select(MapMember);
        }

        private Member MapMember(IMember member)
        {
            return member as Member;
        }

        private IList<KeyValuePair<string, string>> MemberProp(IEnumerable<IMember> members)
        {
            var properties = new List<KeyValuePair<string, string>>();
            foreach (var member in members)
            {
                foreach (var prop in member.Properties)
                {
                    if (prop.Value != null && !string.IsNullOrWhiteSpace(prop.Value.ToString()) && !_umbracoMembersConvention.Any(x => String.Equals(x, prop.Alias, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        properties.Add(new KeyValuePair<string, string>(member.PropertyTypes.First(x => x.Alias == prop.Alias).Name, prop.Value.ToString()));
                    }
                }
            }
            if (properties.Count > 5)
                properties.RemoveRange(0, 5);
            return properties;
        }

        public XLWorkbook ExecuteMemberToExcel()
        {
            //int totalRecords;
            //var members = Map(_memberService.GetAll(0, int.MaxValue, out totalRecords));
            //return CreateExcelMembers(members.ToList());
            int totalRecords;
            var members = GetAllMembers(0,int.MaxValue);
            return CreateExcelMembersGeneric(members);
        }

        //public IMember CreateMember(string userName, string email, string name, string firstName = null, string lastName = null, string gender = null, string phone = null, string city = null, DateTime? birthDay = null, bool inNewsLetter = false)
        //{
        //    if (_memberService.Exists(userName))
        //        return null;
        //    var member = _memberService.CreateMemberWithIdentity(userName, email, name, "Member");
        //    SetAttrIfNotEmpty(member, "firstName", firstName);
        //    SetAttrIfNotEmpty(member, "lastName", lastName);
        //    SetAttrIfNotEmpty(member, "gender", gender);
        //    SetAttrIfNotEmpty(member, "phone", phone);
        //    SetAttrIfNotEmpty(member, "city", city);
        //    if (birthDay.HasValue)
        //        member.SetValue("birthday", birthDay);
        //    member.SetValue("inNewsLetter", inNewsLetter);
        //    _memberService.Save(member);

        //    return member;

        //}

        private void SetAttrIfNotEmpty(IMember member, string attName, string attValue)
        {
            if (!string.IsNullOrWhiteSpace(attValue))
                member.SetValue(attName, attValue);

        }

        private XLWorkbook CreateExcelMembersGeneric(CustomMember customMember)
        {


            var workbook = new XLWorkbook();
            if (customMember == null)
            {
                return workbook;
            }
            var members = customMember.Members.ToList();
            var worksheet = workbook.Worksheets.Add("Members Sheet");
            var rngData = worksheet.Range(1, 1, members.Count + 1, 6+(customMember.PropertiesList.Count/2));

            //first row

            rngData.Row(1).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.Aqua).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            int n = 0;
            worksheet.Cell(1, ++n).Value = "ID";
            worksheet.Cell(1, ++n).Value = "Name";
            worksheet.Cell(1, ++n).Value = "User Name";
            worksheet.Cell(1, ++n).Value = "Email";
            worksheet.Cell(1, ++n).Value = "IsApproved";
            worksheet.Cell(1, ++n).Value = "IsLockedOut";

            var propertiesListGroup = customMember.PropertiesList.GroupBy(x => x.Key);
            var listGroup = propertiesListGroup as IList<IGrouping<string, KeyValuePair<string, string>>> ?? propertiesListGroup.ToList();
            for (int i = 0; i < listGroup.ToList().Count; i++)
            {
                worksheet.Cell(1, ++n).Value = listGroup[i].Key;
            }
            //foreach (var item in customMember.PropertiesList)
            //{
            //    worksheet.Cell(1, ++n).Value = item.Key;
            //}

            for (int i = 1; i < members.Count() + 1; i++)
            {
                int rowNumber = i + 1;
                if (i % 2 == 0)
                {
                    rngData.Row(rowNumber).Style.Fill.SetBackgroundColor(XLColor.Azure);
                }
                n = 0;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Id;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Name;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Username;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Email;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].IsApproved;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].IsLockedOut;

                foreach (var item in listGroup)
                {
                    worksheet.Cell(rowNumber, ++n).Value = item.ToList()[i-1].Value;
                }
            }
            
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            return workbook;
        }

        private XLWorkbook CreateExcelMembers(List<Member> members)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Members Sheet");
            var rngData = worksheet.Range(1, 1, members.Count + 1, 10);

            //first row

            rngData.Row(1).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.Aqua).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            int n = 0;
            worksheet.Cell(1, ++n).Value = "ID";
            worksheet.Cell(1, ++n).Value = "Name";
            worksheet.Cell(1, ++n).Value = "User Name";
            worksheet.Cell(1, ++n).Value = "Email";
            worksheet.Cell(1, ++n).Value = "IsApproved";
            worksheet.Cell(1, ++n).Value = "IsLockedOut";

            for (int i = 1; i < members.Count() + 1; i++)
            {
                int rowNumber = i + 1;
                if (i % 2 == 0)
                {
                    rngData.Row(rowNumber).Style.Fill.SetBackgroundColor(XLColor.Azure);
                }
                n = 0;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Id;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Name;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Username;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].Email;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].IsApproved;
                worksheet.Cell(rowNumber, ++n).Value = members[i - 1].IsLockedOut;

            }
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            return workbook;
        }

        private string StructurePhone(string phone)
        {
            if (phone.IndexOf("-", StringComparison.InvariantCultureIgnoreCase) > -1)
                return phone;

            if (phone.Length > 7)
                return string.Concat(phone.Substring(0, phone.Length - 7), "-", phone.Substring(phone.Length - 7));

            return phone;
        }
    }
}
