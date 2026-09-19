using Infrastructure.Repositories;
using Modules.Communities.Ports;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;

public class CommunityMemberRepository : BaseRepository<AppDbContext, CommunityMemberEntity>, ICommunityMemberRepository
{
    public CommunityMemberRepository(AppDbContext context) : base(context)
    {

    }
}