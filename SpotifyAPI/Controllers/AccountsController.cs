using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;
using SpotifyAPI.Exceptions;
using SpotifyAPI.Models.Errors;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountsController(
    UserManager<User> userManager,
    IMapper mapper,
    IJwtTokenService jwtTokenService,
    IValidator<RegisterVm> registerValidator,
    IValidator<UserUpdateVm> updateValidator,
    IAccountsControllerService service,
    IScopedIdentityService identityService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetInfo()
    {
        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        var result = mapper.Map<UserVm>(identityService.User);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginVm model)
    {
        User? user = await userManager.FindByEmailAsync(model.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Wrong authentication data");

        return Ok(new JwtTokenResponse
        {
            Token = await jwtTokenService.CreateTokenAsync(user)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] RegisterVm vm)
    {
        var validationResult = await registerValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var user = await service.SignUpAsync(vm);

            return Ok(new JwtTokenResponse
            {
                Token = await jwtTokenService.CreateTokenAsync(user)
            });
        }
        catch (IdentityException e)
        {
            return StatusCode(500, e.IdentityResult.Errors);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVm vm)
    {
        try
        {
            await service.GeneratePasswordResetTokenAsync(vm.Email);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new ErrorResponse { Message = e.Message, StatusCode = 500 });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVm vm)
    {
        try
        {
            await service.ResetPasswordAsync(vm.Email, vm.Token, vm.Password);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new ErrorResponse { Message = e.Message, StatusCode = 500 });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Update([FromForm] UserUpdateVm vm)
    {
        var validationResult = await updateValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        await service.UpdateAsync(vm, identityService.User);

        return Ok();
    }
}