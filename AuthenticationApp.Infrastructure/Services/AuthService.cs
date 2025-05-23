using AuthenticationApp.Application.Common;
using AuthenticationApp.Application.DTOs;
using AuthenticationApp.Application.Interfaces;
using AuthenticationApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AuthenticationApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IDomainEventDispatcher _dispatcher;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenGenerator tokenGenerator,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository, IDomainEventDispatcher dispatcher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _dispatcher = dispatcher;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if the email is already taken
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = new List<string> { "Email already in use" }
                };
            }
            // Create the user entity
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName
            };
            // Create user in Identity store
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
            // ✅ Assign the default "User" role to the new user
            await _userManager.AddToRoleAsync(user, "User");

            // ✅ Generate email confirmation token and encode it
            var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(rawToken);

            // ✅ Raise domain event for email confirmation
            user.RegisterUser(encodedToken);

            // ✅ Save changes and dispatch domain events
            await _unitOfWork.SaveChangesAsync();
            await _dispatcher.DispatchAsync(user.DomainEvents);

            // ✅ Get roles for token generation
            var roles = await _userManager.GetRolesAsync(user);

            // ✅ Generate and return JWT token
            return new AuthResponse
            {
                Success = true,
                Token = _tokenGenerator.GenerateToken(user, roles),
                RefreshToken = _tokenGenerator.GenerateRefreshToken()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = new List<string> { "Invalid credentials" }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = new List<string> { "Invalid credentials" }
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponse
            {
                Success = true,
                Token = _tokenGenerator.GenerateToken(user, roles),
                RefreshToken = _tokenGenerator.GenerateRefreshToken()
            };
        }

        public Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            // ⚠️ يتم تنفيذه لاحقًا إذا أردت دعم تخزين وإدارة refresh tokens في قاعدة البيانات
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Errors = new List<string> { "Refresh token logic not implemented." }
            });
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
    }

}
