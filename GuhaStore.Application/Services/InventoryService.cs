using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GuhaStore.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public InventoryService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<Inventory>> GetAllInventoriesAsync()
    {
        return await _context.Inventories
            .Include(i => i.Account)
            .Include(i => i.InventoryDetails)
                .ThenInclude(id => id.Product)
            .OrderByDescending(i => i.InventoryDate)
            .ToListAsync();
    }

    public async Task<Inventory?> GetInventoryByIdAsync(int id)
    {
        return await _context.Inventories
            .Include(i => i.Account)
            .Include(i => i.InventoryDetails)
                .ThenInclude(id => id.Product)
            .FirstOrDefaultAsync(i => i.InventoryId == id);
    }

    public async Task<IEnumerable<Inventory>> GetInventoriesByStatusAsync(int status)
    {
        return await _context.Inventories
            .Include(i => i.Account)
            .Where(i => i.InventoryStatus == status)
            .OrderByDescending(i => i.InventoryDate)
            .ToListAsync();
    }

    public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.Inventories.AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            // Add inventory details and update product quantities
            foreach (var detail in inventory.InventoryDetails)
            {
                detail.InventoryCode = inventory.InventoryCode;
                await _unitOfWork.InventoryDetails.AddAsync(detail);

                // Update product variant quantity
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == detail.ProductId);

                if (variant != null)
                {
                    variant.VariantQuantity += detail.ProductQuantity;
                    variant.UpdatedAt = DateTime.Now;
                    _context.ProductVariants.Update(variant);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return inventory;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateInventoryAsync(Inventory inventory)
    {
        await _unitOfWork.Inventories.UpdateAsync(inventory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductStatus == 1 && 
                (p.ProductQuantity < threshold || 
                 p.ProductVariants.Any(pv => pv.VariantQuantity < threshold && pv.VariantStatus)))
            .ToListAsync();
    }
}

