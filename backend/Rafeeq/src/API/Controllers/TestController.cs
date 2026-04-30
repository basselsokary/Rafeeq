using API.Controllers.Base;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace API.Controllers;

[Route("api/[controller]")]
public class TestController() : ApiBaseController
{
    [HttpPost("send-email")]
    public async Task<IActionResult> Public(
        string email,
        string userName,
        [FromServices] IEmailService emailService)
    {
        await emailService.SendWelcomeEmailAsync(email, userName);
        return Ok("Email sent!");
    }

    /// This endpoint is for testing purposes only. It can be used to verify that the authentication
    /// mechanism is working correctly. It requires a valid access token to be included in the request header.
    /// If the token is valid, it will return a success message. Otherwise, it will return an unauthorized error.
    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        return Ok("You are authenticated!");
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(
        IFormFile file,
        [FromServices] IFileStorageService fileStorageService,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        
        using var stream = file.OpenReadStream();

        var storageKey = $"test-uploads/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var result = await fileStorageService.UploadAsync(stream, storageKey, cancellationToken);
        if (result.Failed)
            return StatusCode(500, "Failed to upload file.");

        // Here you would typically save the file to storage and return the URL
        // For demonstration, we'll just return a success message with the file name
        return Ok($"File '{result.Url}' uploaded successfully!");
    }

    [HttpPost("upload-images")]
    public async Task<IActionResult> UploadImages(
        IFormFileCollection files,
        [FromServices] IFileStorageService fileStorageService,
        CancellationToken cancellationToken = default)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded.");
        
        var uploadTasks = new List<Task<IActionResult>>();
        foreach (var file in files)
        {
            uploadTasks.Add(UploadSingleImage(file, fileStorageService, cancellationToken));
        }

        await Task.WhenAll(uploadTasks);
        return Ok($"Files uploaded successfully!");
    }

    private async Task<IActionResult> UploadSingleImage(
        IFormFile file,
        IFileStorageService fileStorageService,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        
        using var stream = file.OpenReadStream();

        var storageKey = $"test-uploads/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        Result<UploadedFileResult> result = await fileStorageService.UploadAsync(stream, storageKey, cancellationToken);
        if (result.Failed)
            return StatusCode(500, "Failed to upload file.");

        Console.WriteLine($"File '{result.Value.Url}' uploaded successfully!");
        return Ok($"File '{result.Value.Url}' uploaded successfully!");
    }

    [HttpPost("scan-image")]
    public async Task<IActionResult> ScanImage(
        IFormFile file,
        [FromServices] IImageScannerService scannerService,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        
        using var stream = file.OpenReadStream();

        var result = await scannerService.ScanAsync(stream, file.ContentType, cancellationToken);
        if (result.Failed)
            return StatusCode(500, "Failed to scan image.");

        return Ok(result.Value);
    }

}
