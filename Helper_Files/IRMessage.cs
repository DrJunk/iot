using System;

public class IRMessage
{
    List<long> intervalList;
    bool startingState;
    static char seperator = '@';

	public IRMessage(List<long> intervalList, bool startingState)
	{
        this.intervalList = intervalList;
        this.startingState = startingState;
	}

    public IRMessage(string encoding)
    {
        string [] parts = encoding.Split(seperator);

        this.startingState = Convert.ToBoolean(parts[1]);
        string[] longList = parts[1].Split(",");
        this.intervalList = new List<long>();
        foreach (string element in longList)
        {
            this.intervalList.Add(Convert.ToInt64(element));
        }
    }

    public string encode()
    {
        econding = "";
        foreach (long element in this.intervalList)
        {
            encoding += element.ToString + ",";
        }
        encoding += seperator + startingState.ToString;
        return ecnoding;
    }
}
