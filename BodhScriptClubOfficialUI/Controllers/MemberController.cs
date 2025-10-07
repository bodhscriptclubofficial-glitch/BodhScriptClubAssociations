using BodhScriptClubOfficialUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static BodhScriptClubOfficialUI.GlobalService.GlobalService;

namespace BodhScriptClubOfficialUI.Controllers
{
   

    public class MemberController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceDeclareMethods _serviceDeclare;
        public string BaseUrl = "";
        public MemberController(IConfiguration configuration, ServiceDeclareMethods serviceDeclare)
        {
            _configuration = configuration;
            _serviceDeclare = serviceDeclare;
            BaseUrl = _configuration["Baseurl"];
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            try
            {
                string urlParameters = "GetAllMembers";

                // Call API
                string responseJson = await _serviceDeclare.GetMethod(BaseUrl, urlParameters);

                // Deserialize into list
                List<Member> members = JsonSerializer.Deserialize<List<Member>>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assign to ViewBag
                ViewBag.Members = members;
            }
            catch (Exception ex)
            {
                ViewBag.Members = new List<Member>(); // fallback
            }

            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> Index([FromForm] Member member)
        {
            try
            {
                // Default to existing image path
                string imagePath = member.MemberPicture;

                // If a new file is uploaded, save it and replace the path
                if (member.PictureFile != null && member.PictureFile.Length > 0)
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MembersImage");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(member.PictureFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await member.PictureFile.CopyToAsync(stream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(member.MemberPicture))
                    {
                        string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", member.MemberPicture.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    imagePath = "/MembersImage/" + fileName;
                }

                // Update member object
                member.MemberPicture = imagePath;

                // Call your service with member data and file (if any)
                var output = await _serviceDeclare.PostAsync(member, BaseUrl, "Members", member.PictureFile);
                if (member.Memberid==0 || member.Memberid==null)
                {
                    await _serviceDeclare.SendEmailAsync(member.MemberemailId, "Member Updated",
                        $"Hello {member.Membername},<br/>Your member record has been saved successfully.Welcome To BodhScriptClub.");
                }

                return Json(new { success = output != "0", picturePath = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int Memberid)
        {
            try
            {
                string Urlparameters = "GetById";
                string output = await _serviceDeclare.GetByIdMethod(BaseUrl, Urlparameters, Memberid);

                if (string.IsNullOrEmpty(output))
                {
                    return NotFound("Member not found.");
                }

                var members = JsonSerializer.Deserialize<List<Member>>(output, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var member = members.FirstOrDefault();

                if (member == null)
                {
                    return NotFound("Member not found.");
                }

                return Ok(member);
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message for debugging
                return StatusCode(500, $"Error fetching member: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task <JsonResult> Delete(Member member)
        {
            try
            {
                string urlParams = "MembersDelete";
               var output = await _serviceDeclare.PostMethod(member, BaseUrl, urlParams);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
