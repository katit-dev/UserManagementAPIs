using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.DTOs;
using UserManagementApi.Models;
//using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // get all deleted == false
        [HttpGet("GetAllUsersLinq")]
        public async Task<IActionResult> GetAllUsersLinq()
        {
            List<User> users = await _context.Users
                .Where(p => p.Deleted == false).ToListAsync();
                return Ok(new ResponseTypeDTO<object>
                {
                    StatusCode = 200,
                Message = "Lay danh sach user thanh cong",
                Content = users
                });
        }

        // get user paging
         [HttpGet("GetUserPagingLinq")]
        public async Task<ActionResult> GetUserPagingLinq(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5)
        {
            if (pageIndex <= 0)
            {
                return BadRequest(new ResponseTypeDTO<object>
                {
                    StatusCode = 400,
                    Message = "pageIndex phai lon hon 0",
                    Content = null
                });
            }

            if (pageSize <= 0)
            {
                return BadRequest(new ResponseTypeDTO<object>
                {
                    StatusCode = 400,
                    Message = "pageSize phai lon hon 0",
                    Content = null
                });
            }

            int totalItems = await _context.Users
                .Where(u => u.Deleted == false)
                .CountAsync();

            List<User> users = await _context.Users
                .Where(u => u.Deleted == false)
                .OrderBy(u => u.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var content = new
            {
                TotalItems = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Data = users
            };

            return Ok(new ResponseTypeDTO<object>
            {
                StatusCode = 200,
                Message = "Lay danh sach user phan trang thanh cong",
                Content = content
            });
        }

        // get using by sorting
         [HttpGet("GetUserSortLinq")]
        public async Task<ActionResult> GetUserSortLinq(
            [FromQuery] string sortBy = "Id",
            [FromQuery] string sortOrder = "asc")
        {
            var query = _context.Users
                .Where(u => u.Deleted == false);

            if (sortBy == "Name" || sortBy == "name")
            {
                if (sortOrder == "desc")
                {
                    query = query.OrderByDescending(u => u.Name);
                }
                else
                {
                    query = query.OrderBy(u => u.Name);
                }
            }
            else if (sortBy == "CreatedAt" || sortBy == "createdAt" || sortBy == "createdat")
            {
                if (sortOrder == "desc")
                {
                    query = query.OrderByDescending(u => u.CreatedAt);
                }
                else
                {
                    query = query.OrderBy(u => u.CreatedAt);
                }
            }
            else
            {
                if (sortOrder == "desc")
                {
                    query = query.OrderByDescending(u => u.Id);
                }
                else
                {
                    query = query.OrderBy(u => u.Id);
                }
            }

            List<User> users = await query.ToListAsync();

            return Ok(new ResponseTypeDTO   <object>
            {
                StatusCode = 200,
                Message = "Sap xep danh sach user thanh cong",
                Content = users
            });
        }

        // search
        [HttpGet("GetUserSearchLinq")]
        public async Task<ActionResult> GetUserSearchLinq([FromQuery] string? keyword = "")
        {
            var query = _context.Users
                .Where(u => u.Deleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.Name.Contains(keyword));
            }

            List<User> users = await query.ToListAsync();

            return Ok(new ResponseTypeDTO<object>
            {
                StatusCode = 200,
                Message = "Tim kiem user thanh cong",
                Content = users
            });
        }

        // get user by id
         [HttpGet("GetUserByIdLinq/{id}")]
        public async Task<ActionResult> GetUserByIdLinq([FromRoute] int id)
        {
            User? user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id && u.Deleted == false);

            if (user == null)
            {
                return NotFound(new ResponseTypeDTO<object>
                {
                    StatusCode = 404,
                    Message = $"Khong tim thay user voi ID {id}",
                    Content = null
                });
            }

            return Ok(new ResponseTypeDTO<object>
            {
                StatusCode = 200,
                Message = "Lay thong tin user thanh cong",
                Content = user
            });
        }

        // insert user
        [HttpPost("InsertUserLinq")]
        public async Task<ActionResult> InsertUserLinq([FromBody] UserCreateDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseTypeDTO<object>
                {
                    StatusCode = 400,
                    Message = "Du lieu khong hop le",
                    Content = ModelState
                });
            }

            User userModel = new User
            {
                Name = newUser.Name,
                Email = newUser.Email,
                Description = newUser.Description,
                Age = newUser.Age,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false
            };

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ResponseTypeDTO<object>
            {
                StatusCode = 201,
                Message = "Them user thanh cong",
                Content = userModel
            });
        }

        // update user
        [HttpPut("UpdateUserFromLinq/{id}")]
        public async Task<ActionResult> UpdateUserFromLinq(
            [FromRoute] int id,
            [FromBody] UserUpdateDTO userUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseTypeDTO<object>
                {
                    StatusCode = 400,
                    Message = "Du lieu khong hop le",
                    Content = ModelState
                });
            }

            User? userModel = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id && u.Deleted == false);

            if (userModel == null)
            {
                return NotFound(new ResponseTypeDTO<object>
                {
                    StatusCode = 404,
                    Message = $"Khong tim thay user voi ID {id}",
                    Content = null
                });
            }

            userModel.Name = userUpdate.Name;
            userModel.Email = userUpdate.Email;
            userModel.Description = userUpdate.Description;
            userModel.Age = userUpdate.Age;
            userModel.UpdatedAt = DateTime.Now;

            _context.Users.Update(userModel);
            await _context.SaveChangesAsync();

            return Ok(new ResponseTypeDTO<object>
            {
                StatusCode = 200,
                Message = "Cap nhat user thanh cong",
                Content = userModel
            });
        }

        // delete user
        [HttpDelete("DeleteUserFromLinq/{id}")]
        public async Task<ActionResult> DeleteUserFromLinq([FromRoute] int id)
        {
            User? userModel = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == id && u.Deleted == false);

            if (userModel == null)
            {
                return NotFound(new ResponseTypeDTO<object>
                {
                    StatusCode = 404,
                    Message = $"Khong tim thay user voi ID {id}",
                    Content = null
                });
            }

            userModel.Deleted = true;
            userModel.UpdatedAt = DateTime.Now;

            _context.Users.Update(userModel);
            await _context.SaveChangesAsync();

            return Ok(new ResponseTypeDTO<object>
            {
                StatusCode = 200,
                Message = "Xoa mem user thanh cong",
                Content = null
            });
        }
      
    }
}