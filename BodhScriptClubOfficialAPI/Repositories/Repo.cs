
//using BodhScriptClubOfficialAPI.DbLayer;
//using BodhScriptClubOfficialAPI.Model;
//using System.Data; // namespace


//namespace BodhScriptClubOfficialAPI.Repositories
//{
//    public class Repo
//    {
//        private readonly BodhScriptClubOfficialAPI.DbLayer.DbLayer _dbLayer;

//        public Repo(BodhScriptClubOfficialAPI.DbLayer.DbLayer dbLayer)
//        {
//            _dbLayer = dbLayer;
//        }
//        public int Authentication(Logincs logincs)
//        {
//            try
//            {
//                string[] paramNames = { "@Username", "@Password" };
//                string[] paramValues = { logincs.Username, logincs.Password };
//                int output=  _dbLayer.ExecuteNonQuery("SP_BodhScript_Credentials", paramNames, paramValues);
//                return output;
//            }
//            catch (Exception ex)
//            {

//            }
//            return 0;

//        }
//        public int SaveUpdateMembers(Member members)
//        {
//            try
//            {

//                string[] paramNames = { "@Rectype", "@MemberId", "@MemberName", "@MemberStream", "@MemberSession", "@MemberRollNo", "@MembersEmailId", "@MemberContactNo", "@MembersImage" };
//                string[] paramValues = { "Insert",Convert.ToInt32(members.Memberid).ToString(),members.Membername,members.Memberstream,members.MemberSession,members.MemberRollno,members.MemberemailId,members.MembercontactNo,members.MemberPicture };
//                int output = _dbLayer.ExecuteNonQuery("SP_Members", paramNames, paramValues);
//                return output;
//            }
//            catch (Exception ex)
//            {

//            }
//            return 0;

//        }
//        public List<Member> FetchallMembers()
//        {
//            try
//            {
//                string[] paramNames = { "@Rectype", "@Id" };
//                object[] paramValues = { "FETCH", DBNull.Value }; // Use DBNull for unused int

//                DataTable output = _dbLayer.ExecuteDataTable("SP_Fetch", paramNames, paramValues);

//                List<Member> members = new List<Member>();

//                foreach (DataRow row in output.Rows)
//                {
//                    Member member = new Member
//                    {
//                        Memberid = row["MemberId"] != DBNull.Value ? Convert.ToInt32(row["MemberId"]) : 0,
//                        Membername = row["MembersName"]?.ToString(),
//                        Memberstream = row["MembersStream"]?.ToString(),
//                        MemberSession = row["MembersSession"]?.ToString(),
//                        MemberRollno= row["MembersRollNo"]?.ToString(),
//                        MemberemailId= row["MembersEmailId"]?.ToString(),
//                        MembercontactNo= row["MembersContactNo"]?.ToString(),
//                        MemberPicture = row["MembersImage"].ToString(),
//                    };

//                    members.Add(member);
//                }

//                return members;
//            }
//            catch (Exception ex)
//            {

//                return new List<Member>();
//            }
//        }


//        public List<Member> FetchMembersId(int id)
//        {
//            try
//            {
//                string[] paramNames = { "@Rectype", "@Id" };
//                object[] paramValues = { "FETCH_ById", Convert.ToString(id) }; // Use DBNull for unused int

//                DataTable output = _dbLayer.ExecuteDataTable("SP_Fetch", paramNames, paramValues);

//                List<Member> members = new List<Member>();

//                foreach (DataRow row in output.Rows)
//                {
//                    Member member = new Member
//                    {
//                        Memberid = row["MemberId"] != DBNull.Value ? Convert.ToInt32(row["MemberId"]) : 0,
//                        Membername = row["MembersName"]?.ToString(),
//                        Memberstream = row["MembersStream"]?.ToString(),
//                        MemberSession = row["MembersSession"]?.ToString(),
//                        MemberRollno = row["MembersRollNo"]?.ToString(),
//                        MemberemailId = row["MembersEmailId"]?.ToString(),
//                        MembercontactNo = row["MembersContactNo"]?.ToString(),
//                        MemberPicture = row["MembersImage"].ToString(),
//                    };

//                    members.Add(member);
//                }

//                return members;
//            }
//            catch (Exception ex)
//            {

//                return new List<Member>();
//            }
//        }
//        public int Delete(Member member)
//        {
//            try
//            {
//                string[] paramNames = { "@Rectype", "@Id" };
//                string[] paramValues = { "Delete", member.Memberid.ToString() };
//                int output = _dbLayer.ExecuteNonQuery("SP_Fetch", paramNames, paramValues);
//                return output;
//            }
//            catch (Exception ex)
//            {

//            }
//            return 0;

//        }

//        public int GetTotalMembersCount()
//        {
//            try
//            {
//                string[] paramNames = { "@Rectype", "@Id" };
//                object[] paramValues = { "COUNT", DBNull.Value };

//                DataTable output = _dbLayer.ExecuteDataTable("SP_Fetch", paramNames, paramValues);

//                if (output.Rows.Count > 0)
//                {
//                    return Convert.ToInt32(output.Rows[0]["TotalMembers"]);
//                }

//                return 0;
//            }
//            catch (Exception ex)
//            {
//                // log error here
//                return 0;
//            }
//        }
//    }
//}
using BodhScriptClubOfficialAPI.DbLayer;
using BodhScriptClubOfficialAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace BodhScriptClubOfficialAPI.Repositories
{
    public class Repo
    {
        private readonly BodhScriptClubOfficialAPI.DbLayer.DbLayer _dbLayer;

        public Repo(BodhScriptClubOfficialAPI.DbLayer.DbLayer dbLayer)
        {
            _dbLayer = dbLayer;
        }

        // Login authentication using PostgreSQL function
        public int Authentication(Logincs logincs)
        {
            string[] paramNames = { "p_Username", "p_Password" };
            string[] paramValues = { logincs.Username, logincs.Password };
            return _dbLayer.ExecuteFunction("sp_bodhscript_credentials", paramNames, paramValues);
        }

        // Save or update members
        public int SaveUpdateMembers(Member members)
        {
            string[] paramNames = { "p_Rectype", "p_MemberId", "p_MemberName", "p_MemberStream", "p_MemberSession", "p_MemberRollNo", "p_MembersEmailId", "p_MemberContactNo", "p_MembersImage" };
            string[] paramValues = { "Insert", members.Memberid.ToString(), members.Membername, members.Memberstream, members.MemberSession, members.MemberRollno, members.MemberemailId, members.MembercontactNo, members.MemberPicture };
            return _dbLayer.ExecuteFunction("sp_members", paramNames, paramValues);
        }

        // Fetch all members
        public List<Member> FetchAllMembers()
        {
            string[] paramNames = { "p_Rectype", "p_Id" };
            object[] paramValues = { "FETCH", DBNull.Value };

            DataTable dt = _dbLayer.ExecuteProcedure("sp_fetch", paramNames, paramValues);
            List<Member> members = new List<Member>();

            foreach (DataRow row in dt.Rows)
            {
                members.Add(new Member
                {
                    Memberid = Convert.ToInt32(row["MemberId"]),
                    Membername = row["MembersName"].ToString(),
                    Memberstream = row["MembersStream"].ToString(),
                    MemberSession = row["MembersSession"].ToString(),
                    MemberRollno = row["MembersRollNo"].ToString(),
                    MemberemailId = row["MembersEmailId"].ToString(),
                    MembercontactNo = row["MembersContactNo"].ToString(),
                    MemberPicture = row["MembersImage"].ToString()
                });
            }

            return members;
        }

        // Delete member
        public int Delete(Member member)
        {
            string[] paramNames = { "p_Rectype", "p_Id" };
            object[] paramValues = { "Delete", member.Memberid };
            return _dbLayer.ExecuteProcedure("sp_fetch", paramNames, paramValues).Rows.Count;
        }
        // Fetch member by ID
        public Member FetchMemberById(int id)
        {
            string[] paramNames = { "p_Rectype", "p_Id" };
            object[] paramValues = { "FETCH_ById", id };

            DataTable dt = _dbLayer.ExecuteProcedure("sp_fetch", paramNames, paramValues);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];
            return new Member
            {
                Memberid = Convert.ToInt32(row["MemberId"]),
                Membername = row["MembersName"].ToString(),
                Memberstream = row["MembersStream"].ToString(),
                MemberSession = row["MembersSession"].ToString(),
                MemberRollno = row["MembersRollNo"].ToString(),
                MemberemailId = row["MembersEmailId"].ToString(),
                MembercontactNo = row["MembersContactNo"].ToString(),
                MemberPicture = row["MembersImage"].ToString()
            };
        }
        // Get total member count
        public int GetTotalMembersCount()
        {
            try
            {
                string[] paramNames = { "p_Rectype", "p_Id" };
                object[] paramValues = { "COUNT", DBNull.Value };

                DataTable dt = _dbLayer.ExecuteProcedure("sp_fetch", paramNames, paramValues);

                if (dt.Rows.Count > 0 && dt.Columns.Contains("TotalMembers"))
                {
                    return Convert.ToInt32(dt.Rows[0]["TotalMembers"]);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }


    }
}

