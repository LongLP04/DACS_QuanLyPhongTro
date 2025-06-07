// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text;
using DACS_QuanLyPhongTro.Models;
using System.Threading;
using System.Threading.Tasks;
using DACS_QuanLyPhongTro.Areas.ChuTroArea.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DACS_QuanLyPhongTro.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Họ tên")]
            public string HoTen { get; set; }

            [Required]
            [Display(Name = "Giới tính")]
            public string Gioitinh { get; set; }

            [Required]
            [StringLength(12)]
            [Display(Name = "CCCD")]
            public string CCCD { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string SoDienThoai { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string? Role { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync(SD.Role_ChuTro))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_ChuTro));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_KhachThue));
            }

            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    HoTen = Input.HoTen,
                    GioiTinh = Input.Gioitinh,
                    CCCD = Input.CCCD,
                    SoDienThoai = Input.SoDienThoai,
                    PhoneNumber = Input.SoDienThoai
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Gán vai trò
                    if (!string.IsNullOrEmpty(Input.Role))
                    {
                        var roleExists = await _roleManager.RoleExistsAsync(Input.Role);
                        if (!roleExists)
                        {
                            ModelState.AddModelError(string.Empty, "Vai trò không hợp lệ.");
                            return Page();
                        }

                        await _userManager.AddToRoleAsync(user, Input.Role);

                        // Thêm vào bảng tương ứng
                        if (Input.Role == "ChuTro")
                        {
                            var chuTro = new DACS_QuanLyPhongTro.Models.ChuTro
                            {
                                HoTen = user.HoTen,
                                Gioitinh = user.GioiTinh,
                                CCCD = user.CCCD,
                                Email = user.Email,
                                SoDienThoai = user.SoDienThoai,
                                ApplicationUserId = user.Id
                            };
                            _context.ChuTros.Add(chuTro);
                        }
                        else if (Input.Role == "KhachThue")
                        {
                            var khachThue = new KhachThue
                            {
                                HoTen = user.HoTen,
                                Gioitinh = user.GioiTinh,
                                CCCD = user.CCCD,
                                Email = user.Email,
                                SoDienThoai = user.SoDienThoai,
                                ApplicationUserId = user.Id
                            };
                            _context.KhachThues.Add(khachThue);
                        }

                        await _context.SaveChangesAsync();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Kiểm tra nếu vai trò là "ChuTro" và không yêu cầu xác nhận email
                    if (Input.Role == "ChuTro")
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home", new { area = "ChuTroArea" }); // Chuyển hướng đến layout của Chủ trọ
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed
            return Page();
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Không thể tạo instance của '{nameof(ApplicationUser)}'. " +
                    $"Hãy đảm bảo rằng '{nameof(ApplicationUser)}' không phải là lớp abstract và có constructor không tham số, hoặc " +
                    $"ghi đè trang đăng ký trong /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}