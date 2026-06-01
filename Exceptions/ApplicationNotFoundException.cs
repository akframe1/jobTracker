public class ApplicationNotFoundException : Exception
{
    public int Id { get; }

    public ApplicationNotFoundException(int id)
        : base($"Application with ID {id} was not found.")
    {
        Id = id;
    }
}