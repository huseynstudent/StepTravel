using StoreApp.Application.CQRS.Users.Command.Request;
using StoreApp.Application.CQRS.Users.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreApp.Repository.Comman;
using Microsoft.AspNetCore.Hosting;

namespace StoreApp.Application.CQRS.Users.Handler;

public class EditProfileCommandHandler
    : IRequestHandler<EditProfileCommandRequest, ResponseModel<EditProfileCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EditProfileCommandHandler> _logger;
    private readonly IWebHostEnvironment _env;

    public EditProfileCommandHandler(
        StoreAppDbContext db,
        ILogger<EditProfileCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment env)
    {
        _db = db;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _env = env;
    }

    public async Task<ResponseModel<EditProfileCommandResponse>> Handle(
        EditProfileCommandRequest request, CancellationToken cancellationToken)
    {
        // 1. İstifadəçini bazadan tapırıq
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("EditProfile: user {UserId} not found.", request.UserId);
            return new ResponseModel<EditProfileCommandResponse>(null);
        }

        // 2. Email unikallıq yoxlaması
        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            bool emailTaken = await _db.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId && !u.IsDeleted,
                          cancellationToken);
            if (emailTaken)
            {
                _logger.LogWarning("EditProfile: email {Email} already in use.", request.Email);
                return new ResponseModel<EditProfileCommandResponse>(null);
            }
        }

        // 3. Şəkil yüklənməsi
        if (request.ProfilePicture is not null)
        {
            // FIX: WebRootPath null ola bilər — wwwroot qovluğu mövcud deyilsə
            // ContentRootPath həmişə mövcuddur, onun altında wwwroot yaradırıq
            var webRoot = _env.WebRootPath
                          ?? Path.Combine(_env.ContentRootPath, "wwwroot");

            var uploads = Path.Combine(webRoot, "uploads", "avatars");
            Directory.CreateDirectory(uploads); // qovluq yoxdursa avtomatik yarat

            var ext = Path.GetExtension(request.ProfilePicture.FileName);
            var fileName = $"{user.Id}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploads, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await request.ProfilePicture.CopyToAsync(stream, cancellationToken);

            // Köhnə faylı sil
            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                var oldPath = Path.Combine(webRoot, user.ProfilePicture.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            user.ProfilePicture = $"/uploads/avatars/{fileName}";
        }

        // 4. Məlumatları yeniləyirik
        user.Name = request.Name;
        user.Surname = request.Surname;
        user.Email = request.Email;
        user.Birthday = request.Birthday;
        user.Fin = request.Fin;

        // 5. Update + SaveChanges
        _unitOfWork.UserRepository.Update(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("EditProfile: user {UserId} updated.", user.Id);

        // 6. Cavab — ProfilePicture daxil olmaqla
        return new ResponseModel<EditProfileCommandResponse>(new EditProfileCommandResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Birthday = user.Birthday,
            Fin = user.Fin,
            ProfilePicture = user.ProfilePicture, 
        });
    }
}