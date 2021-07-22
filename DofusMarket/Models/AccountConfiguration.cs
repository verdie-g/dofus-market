using System;

namespace DofusMarket.Models
{
    internal class AccountConfiguration
    {
        public string AccountName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int CertificateId { get; set; }
        public string CertificateHash { get; set; } = string.Empty;
        public CharacterConfiguration[] Characters { get; set; } = Array.Empty<CharacterConfiguration>();
    }
}
