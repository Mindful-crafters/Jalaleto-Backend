﻿using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ApiResponse> Singup(SignUpRequestModel request)
        {
            return await _userRepository.SignUp(request);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ApiResponse> Login(LoginRequestModel request)
        {
            return await _userRepository.Login(request);
        }

        [HttpPost]
        [Route("CheckUsername")]
        public async Task<ApiResponse> CheckUsername(string userName)
        {
            return await _userRepository.CheckUsername(userName);
        }

    }
}
