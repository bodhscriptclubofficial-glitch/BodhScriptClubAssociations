using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BodhScriptClubOfficialUI.Models
{
    public class Member
    {
        private int memberid; public int Memberid { get { return memberid; } set { memberid = value; } }
        private string? membername; public string? Membername { get { return membername; } set { membername = value; } }
        private string? memberstream; public string? Memberstream { get { return memberstream; } set { memberstream = value; } }
        private string? memberSession; public string? MemberSession { get { return memberSession; } set { memberSession = value; } }
        private string? memberrollno; public string? MemberRollno { get { return memberrollno; } set { memberrollno = value; } }
        private string? memberemailid; public string? MemberemailId { get { return memberemailid; } set { memberemailid = value; } }
        private string? membercontactno; public string? MembercontactNo { get { return membercontactno; } set { membercontactno = value; } }

        private string? memberpicture; public string? MemberPicture { get { return memberpicture; } set { memberpicture = value; } }
        [JsonPropertyName("totalMembers")]

        public int TotalMembers { get; set; }
        public IFormFile?  PictureFile { get; set; }

    }
}
