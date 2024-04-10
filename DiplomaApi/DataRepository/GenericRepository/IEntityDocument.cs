using System.ComponentModel.DataAnnotations;

namespace DiplomaApi.DataRepository.GenericRepository
{
    public interface IEntityDocument
    {
        [Key]
        int Id { get; set; }
    }
}
