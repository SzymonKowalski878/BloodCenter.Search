﻿using BloodCenter.Search.Domain.Interfaces;
using Nest;

namespace BloodCenter.Search.Domain.Models
{
    [ElasticsearchType(IdProperty = nameof(Id), RelationName = "user")]
    public class UserDocument: IDocumentBase
    {
        [Keyword]
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        [Keyword]
        public string Role { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
