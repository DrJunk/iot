using System;
using System.Collections.Generic;

public class IRMessage
{
    List<double> intervalList;
    bool startingState;
    static char seperator = '@';

	public IRMessage(List<double> intervalList, bool startingState)
	{
        this.intervalList = intervalList;
        this.startingState = startingState;
	}

    public IRMessage(string encoding)
    {
        string [] parts = encoding.Split(seperator);

        this.startingState = Convert.ToBoolean(parts[1]);
        string[] doubleList = parts[0].Split(',');
        this.intervalList = new List<double>();
        foreach (string element in doubleList)
        {
            Console.WriteLine(element);
            if (element.Length != 0)
                this.intervalList.Add(Convert.ToInt64(element));
        }
    }

    public string encode()
    {
        string encoding = "";
        foreach (double element in this.intervalList)
        {
            encoding += element.ToString() + ",";
        }
        encoding += seperator + startingState.ToString();
        return encoding;
    }
}
