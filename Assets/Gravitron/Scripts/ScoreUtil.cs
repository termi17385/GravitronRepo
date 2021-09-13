
public enum ScoreTypes
{
	Fuel,
	Shield,
	SpaceMen,
	BonusTime,
	None
}
public static class ScoreUtil
{
	public static ScoreTypes score = ScoreTypes.None; 
	public static int ScoreHandler(ScoreTypes _score, int _amt)
	{
		var total = 0;
		switch(_score)
		{
			case ScoreTypes.Fuel:      total = (_amt * 10);  break;
			case ScoreTypes.Shield:    total = (_amt * 10);  break;
			case ScoreTypes.SpaceMen:  total = (_amt * 100); break;
			case ScoreTypes.BonusTime: total = _amt;         break;
			case ScoreTypes.None:      total = 0;            break;
		}
		return total;
	}
}
