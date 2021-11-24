﻿using ContactsAPI.Entities;
using ContactsAPI.Persistance;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContactsAPI.Application.Kisiler.Commands
{
    public class DeleteKisiCommand : IRequest<bool>
    {
        public Guid KisiId { get; set; }
    }

    public class DeleteKisiHandle : IRequestHandler<DeleteKisiCommand, bool>
    {
        private readonly DatabaseContext db;

        public DeleteKisiHandle(DatabaseContext db)
        {
            this.db = db;
        }

        public async Task<bool> Handle(DeleteKisiCommand request, CancellationToken cancellationToken)
        {
            Kisi saved = await db.Kisiler.FindAsync(request.KisiId);
            db.Kisiler.Remove(saved);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
