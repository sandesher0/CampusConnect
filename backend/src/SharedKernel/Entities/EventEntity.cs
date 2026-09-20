    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using SharedKernel.Constants;
    using SharedKernel.Entities;

    namespace Modules.Events.Domain;

    public class EventEntity
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public required Guid CommunityId { get; set; }
        public required Guid CreatedBy { get; set; }
        public required DateTimeOffset EventDate { get; set; }
        public required DateTimeOffset EventEndDate { get; set; }
        public required EventVisibility Visibility { get; set; }
        public required EventCategory Category { get; set; }
        public required DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        [ForeignKey(nameof(CommunityId))]
        public virtual CommunityEntity Community { get; set; } = default!;
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserEntity User { get; set; } = default!;
    }