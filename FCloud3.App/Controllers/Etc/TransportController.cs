using FCloud3.Entities.Transport;
using FCloud3.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCloud3.App.Controllers.Etc;

public class TransportController(
    IWebHostEnvironment env,
    FCloudContext db,
    ILogger<TransportController> logger) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly FCloudContext _db = db;
    private readonly ILogger<TransportController> _logger = logger;

    // 返回当前登录用户的 transport 列表（仅本人可见）
    public async Task<IActionResult> GetList()
    {
        var uid = GetCurrentUserId();
        if (uid <= 0) return this.ApiFailedResp("未登录");
        var list = await _db.TransportItems.Where(x => x.OwnerId == uid).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return this.ApiResp(list);
    }

    // multipart/form-data 上传 HTML 文件
    public async Task<IActionResult> Upload(IFormFile file, string Name, string? Color)
    {
        var uid = GetCurrentUserId();
        if (uid <= 0) return this.ApiFailedResp("未登录");
        if (file == null) return this.ApiFailedResp("未选择文件");
        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
        if (ext != ".html" && ext != ".htm") return this.ApiFailedResp("仅允许上传 HTML 文件");

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var dir = Path.Combine(webRoot, "transports", uid.ToString());
        Directory.CreateDirectory(dir);

        // 生成安全文件名，避免覆盖
        var safeFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
        var filePath = Path.Combine(dir, safeFileName);
        try
        {
            using var fs = System.IO.File.Create(filePath);
            await file.CopyToAsync(fs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存上传文件失败");
            return this.ApiFailedResp("保存文件失败");
        }

        var item = new TransportItem
        {
            OwnerId = uid,
            Name = string.IsNullOrWhiteSpace(Name) ? Path.GetFileNameWithoutExtension(file.FileName) : Name,
            Color = Color,
            FileUrl = $"/transports/{uid}/{safeFileName}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.TransportItems.Add(item);
        await _db.SaveChangesAsync();
        return this.ApiResp(item);
    }

    public async Task<IActionResult> Edit([FromBody] EditTransportDto dto)
    {
        var uid = GetCurrentUserId();
        if (uid <= 0) return this.ApiFailedResp("未登录");
        var it = await _db.TransportItems.FindAsync(dto.Id);
        if (it == null) return this.ApiFailedResp("未找到");
        if (it.OwnerId != uid && !IsAdmin()) return this.ApiFailedResp("没有权限");
        it.Name = dto.Name ?? it.Name;
        it.Color = dto.Color ?? it.Color;
        it.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return this.ApiResp(true);
    }

    public async Task<IActionResult> Remove(int id)
    {
        var uid = GetCurrentUserId();
        if (uid <= 0) return this.ApiFailedResp("未登录");
        var it = await _db.TransportItems.FindAsync(id);
        if (it == null) return this.ApiFailedResp("未找到");
        if (it.OwnerId != uid && !IsAdmin()) return this.ApiFailedResp("没有权限");

        // 尝试删除文件
        try
        {
            if (!string.IsNullOrWhiteSpace(it.FileUrl))
            {
                var rel = it.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var full = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), rel);
                if (System.IO.File.Exists(full)) System.IO.File.Delete(full);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "删除 transport 文件时出错");
        }

        _db.TransportItems.Remove(it);
        await _db.SaveChangesAsync();
        return this.ApiResp(true);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    private bool IsAdmin()
    {
        try
        {
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")) return true;
        }
        catch { }
        return false;
    }

    public class EditTransportDto { public int Id { get; set; } public string? Name { get; set; } public string? Color { get; set; } }
}
