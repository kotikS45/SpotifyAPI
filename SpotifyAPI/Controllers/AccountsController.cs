using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;
using SpotifyAPI.Exceptions;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountsController(
    UserManager<User> userManager,
    IJwtTokenService jwtTokenService,
    IValidator<RegisterVm> registerValidator,
    IAccountsControllerService service) : ControllerBase
{

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
}