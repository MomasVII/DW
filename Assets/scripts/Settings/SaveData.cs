using System;
using System.Collections.Generic;

[Serializable]
public struct SaveData
{
	public List<CarListData> CarList;
}

[Serializable]
public class CarListData
{
	public int carId;
	public List<string> carColors;
	public int currentColor;
	public int currentSpecColor;
	public int speed;
	public int acceleration;
	public int handling;
}
