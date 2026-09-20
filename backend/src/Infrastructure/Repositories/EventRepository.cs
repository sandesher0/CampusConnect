using Modules.Events.Domain;
using Modules.Events.Ports;
using SharedKernel.Interfaces;

namespace Infrastructure.Repositories;

public class EventRepository : BaseRepository<AppDbContext, EventEntity>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context)
    {

    }
}