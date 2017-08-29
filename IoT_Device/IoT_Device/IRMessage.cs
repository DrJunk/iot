using System;
using System.Collections.Generic;

public class IRMessage
{
    public List<double> intervalList;

	public IRMessage(List<double> intervalList)
	{
        this.intervalList = intervalList;
	}

    public IRMessage(string encoding)
    {
        string[] doubleList = encoding.Split(',');
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
        return encoding;
    }

    // for visual use only!
    override public string ToString()
    {
        string str = "";
        foreach (double element in this.intervalList)
        {
            str += element.ToString("#.00") + ",";
        }
        return str;
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
}
