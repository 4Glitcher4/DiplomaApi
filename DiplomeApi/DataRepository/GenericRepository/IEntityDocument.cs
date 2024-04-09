using System.ComponentModel.DataAnnotations;

namespace DiplomeApi.DataRepository.GenericRepository
{
    public interface IEntityDocument
    {
        [Key]
        int Id { get; set; }
    }
}
