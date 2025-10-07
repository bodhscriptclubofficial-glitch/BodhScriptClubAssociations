using BodhScriptClubOfficialAPI.Model;
using BodhScriptClubOfficialAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BodhScriptClubOfficialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly Repo _repo;

        public AuthenticationController(Repo repo)
        {
            _repo = repo;
        }

        [HttpPost("Credentials")]
        public IActionResult Credentials([FromBody] Logincs logincs)
        {
            if (logincs == null) return BadRequest("Invalid request");

            int result = _repo.Authentication(logincs);
            return Ok(result);
        }
        [HttpPost("Members")]
        public IActionResult Members([FromForm] Member g1)
        {
            try
            {
                if (g1 == null)
                    return BadRequest("Invalid request: member is null");

                // Save the uploaded file if present
                if (g1.PictureFile != null)
                {
                    // Build absolute path to the UI project's wwwroot/MembersImage
                    string uiRoot = Path.Combine(
                        Directory.GetParent(Directory.GetCurrentDirectory()).FullName,  // parent folder
                        "BodhScriptClubOfficialUI",                                     // your UI project folder
                        "wwwroot",
                        "MembersImage"
                    );

                    // Ensure folder exists
                    if (!Directory.Exists(uiRoot))
                        Directory.CreateDirectory(uiRoot);

                    // Generate unique filename and save
                    string fileName = Guid.NewGuid() + Path.GetExtension(g1.PictureFile.FileName);
                    string filePath = Path.Combine(uiRoot, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        g1.PictureFile.CopyTo(stream);
                    }

                    // Save relative path in DB
                    g1.MemberPicture = "/MembersImage/" + fileName;
                }


                int result = _repo.SaveUpdateMembers(g1);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAllMembers")]
        public ActionResult<List<Member>> GetAllMembers()
        {
            try
            {
                var members = _repo.FetchAllMembers();

                if (members == null || members.Count == 0)
                {
                    return NotFound("No members found.");
                }

                return Ok(members);
            }
            catch (Exception ex)
            {
               //Log error here if needed
                return StatusCode(500, "An error occurred while fetching members.");
            }

        }
        [HttpGet("GetById/{Id}")]
        public ActionResult<List<Member>> GetMembersById(int Id)
        {
            try
            {
                var members = _repo.FetchMemberById(Id);

                if (members == null )
                {
                    return NotFound("No members found.");
                }

                return Ok(members);
            }
            catch (Exception ex)
            {
                //Log error here if needed
                return StatusCode(500, "An error occurred while fetching members.");
            }

        }
        [HttpPost("MembersDelete")]
        public IActionResult MembersDelete([FromBody] Member member)
        {
            if (member.Memberid == null) return BadRequest("Invalid request");

            int result = _repo.Delete(member);
            return Ok(result);
        }
        [HttpGet("TotalMembers")]
        public IActionResult GetTotalMembersCount()
        {
            try
            {
                int totalMembers = _repo.GetTotalMembersCount();
                return Ok(new { TotalMembers = totalMembers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching member count.", error = ex.Message });
            }
        }
    }
}

