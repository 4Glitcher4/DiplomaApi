namespace DiplomaApi.DataRepository.GenericRepository
{
    public class EntityDocument : IEntityDocument
    {
        public int Id { get; set; }

        public enum IntentStatus
        {
            Block,
            Allow
        }
    }
}
