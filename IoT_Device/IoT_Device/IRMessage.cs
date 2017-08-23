using System;
using System.Collections.Generic;

public class IRMessage
{
    public List<double> intervalList;
    public bool startingState;
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

    public string Encode()
    {
        string encoding = "";
        foreach (double element in this.intervalList)
        {
            encoding += element.ToString() + ",";
        }
        encoding += seperator + startingState.ToString();
        return encoding;
    }

    public string ParseToBits()
    {
        string parsed = "";
        foreach (double d in intervalList)
        {
            if (d < 1.6)
                parsed += "0";
            else
                parsed += "1";
        }
        return parsed;
    }

    public string ParseToRemoteButton()
    {
        string input = this.ParseToBits();
        if (input.StartsWith("100010000111011111010000001011111"))
            return "Up\n";
        if (input.StartsWith("100010000111011110000000011111111"))
            return "Down\n";
        if (input.StartsWith("100010000111011110001000011101111"))
            return "Left\n";
        if (input.StartsWith("100010000111011111000000001111111"))
            return "Right\n";
        if (input.StartsWith("100010000111011111101100000100111"))
            return "Off\n";
        if (input.StartsWith("100010000111011111111100000000111"))
            return "A\n";
        if (input.StartsWith("100010000111011110111100010000111"))
            return "B\n";
        if (input.StartsWith("100010000111011110101100010100111"))
            return "C\n";
        if (input.StartsWith("100010000111011110010000011011111"))
            return "Please don't press this button....\n";
        return "Not a button";
    }
}
