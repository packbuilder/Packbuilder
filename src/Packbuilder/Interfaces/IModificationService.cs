using Packbuilder.Dto.Create;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface IModificationService
    {
        public Task CreateModificationAsync(CreateModificationDto dto, int suggestionId);
        public Task CreateModificationsAsync(List<CreateModificationDto> dtos, int suggestionId);
        public Task DeleteModificationAsync(int modificationId, int suggestionId);
    }
}