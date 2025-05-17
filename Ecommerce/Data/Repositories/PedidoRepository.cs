using Ecommerce.Objects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Ecommerce.Data.Builders;
using Ecommerce.Data.Interfaces;

namespace Ecommerce.Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Pedido> _dbSet;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Pedido>();
    }

    public async Task<IEnumerable<Pedido>> Get()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<Pedido> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task Add(Pedido pedido)
    {
        await _dbSet.AddAsync(pedido);
        await SaveChanges();
    }

    public async Task Update(Pedido pedido)
    {
        // Recupera a chave primária (supondo que seja 'Id')
        var pedidoId = _context.Entry(pedido).Property("Id").CurrentValue;

        // Verifica se a entidade com o mesmo Id já está sendo rastreada
        var trackedEntity = _context.ChangeTracker.Entries<Pedido>()
            .FirstOrDefault(e => e.Property("Id").CurrentValue.Equals(pedidoId));

        // Se a entidade já estiver sendo rastreada, desanexa
        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
        }

        // Anexa a nova entidade e marca como 'Modified'
        _context.Entry(pedido).State = EntityState.Modified;

        // Salva as alteraçõees no banco de dados
        await SaveChanges();
    }

    public async Task<bool> SaveChanges()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
