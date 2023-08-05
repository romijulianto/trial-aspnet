namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    /// <summary>
    /// Interface IChartRenderer
    /// </summary>
    public interface IChartRenderer
    {
        string GetContainer(int width, int height);
        string GetScript(int width, int height);
    }
} // End Partial class
