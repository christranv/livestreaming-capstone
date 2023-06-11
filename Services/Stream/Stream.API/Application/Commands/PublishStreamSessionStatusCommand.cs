using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MediatR;
using Stream.API.Application.Models;

namespace Stream.API.Application.Commands
{
    public class PublishStreamSessionStatusCommand : IRequest<bool>
    {
        [DataMember] public string StreamerName { get; set; }
        [DataMember] public string StreamerImageUrl { get; set; }
        [DataMember] public string StreamerIdentityGuid { get; set; }
        [DataMember] public string SubEventId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public string Announcement { get; set; }

        [DataMember] public string LanguageName { get; set; }

        [DataMember] public CategoryDTO Category { get; set; }
        [DataMember] public List<TagDTO> Tags { get; set; } = new();
    }
}