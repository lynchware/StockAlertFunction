
namespace StockAlertFunction.Services
{
	public interface IHttpService
	{
		Task<IDictionary<string, decimal>> GetStockPercentageChangesOverHour(string[] stockSymbols);
	}
}