using AutoMapper;
using MediatR;
using StoreApp.Application.Email.Commands;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using System;
namespace StoreApp.Application.Email.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly StoreAppDbContext _context;
        private readonly IMapper _mapper;
        public RegisterUserCommandHandler(StoreAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _mapper.Map<User>(request);
                string code = new Random().Next(100000, 999999).ToString();

                user.ConfirmCode = code;
                user.IsConfirmed = false;

                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}