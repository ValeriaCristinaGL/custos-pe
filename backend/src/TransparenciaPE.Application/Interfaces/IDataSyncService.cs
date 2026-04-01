namespace TransparenciaPE.Application.Interfaces;

public interface IDataSyncService
{
    Task<int> SyncEmpenhosAsync(int ano);
    Task<int> SyncContratosAsync(int ano);
    Task<SyncResultDto> SyncAllAsync(int ano);
}

public class SyncResultDto
{
    public int EmpenhosProcessados { get; set; }
    public int ContratosProcessados { get; set; }
    public DateTime SyncedAt { get; set; }
}
