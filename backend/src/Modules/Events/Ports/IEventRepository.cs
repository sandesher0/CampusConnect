

using SharedKernel.Interfaces;
using SharedKernel.Entities;
using Modules.Events.Domain;

namespace Modules.Events.Ports;

public interface IEventRepository : IBaseRepository<EventEntity>
{

}