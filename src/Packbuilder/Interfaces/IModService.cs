using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface IModService
    {
        public Task<Mod> GetOrCreateModAsync(ExternalModSummary modSummary);
    }
}