using System;
using System.Collections.Generic;
class View
{
    public View()
    {
        Console.WriteLine("\tWelcome!");
    }
    public string Command()
    {
        Console.Write("\nEnter command ('output', 'dataBase', 'statistics', 'exit'): ");
        string answer = Console.ReadLine();
        if(answer == "output")
        {
            Console.Write("\nEnter command ('cars', 'index'): ");
            string a = Console.ReadLine();
            if(a == "cars")
            {
                return answer + " " + a;
            }
            else
            {
                return answer + " " + a;
            }
        }
        else if(answer == "dataBase")
        {
            // TODO копіювання та резервування
            Console.Write("\nEnter command ('insert', 'update', 'delete', 'random', 'backup', 'reservation'): ");
            answer = Console.ReadLine();
            if(answer == "insert" || answer == "update" || answer == "delete" || answer == "random")
            {
                Console.Write("\nEnter entity ('camera', 'car', 'carowner'): ");
                string entity = Console.ReadLine();
                if(entity == "camera")
                {
                    if(answer == "insert")
                    {
                        Console.Write("\nEnter location: ");
                        string location = Console.ReadLine();
                        return answer + " " + entity + " " + location;
                    }
                    else if(answer == "update")
                    {
                        Console.Write("\nEnter location: ");
                        string location = Console.ReadLine();
                        Console.Write("\nEnter camera id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + location+ " " + id;
                    }
                    else if(answer == "delete")
                    {
                        Console.Write("\nEnter camera id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + id;
                    }
                    else
                    {
                        Console.Write("\nHow many cameras? ");
                        string num = Console.ReadLine();
                        return answer + " " + entity + " " + num;;
                    }
                }
                else if(entity == "car")
                {
                    if(answer == "insert")
                    {
                        Console.Write("\nEnter 'car number' 'owner id' 'camera id' 'speed': ");
                        string a = Console.ReadLine();
                        string[] s = a.Split(" ");
                        return answer + " " + entity + " " + s[0] + " " + s[1] + " " + s[2] + " " + s[3];
                    }
                    else if(answer == "update")
                    {
                        Console.Write("\nEnter 'car number' 'owner id' 'camera id' 'speed': ");
                        string a = Console.ReadLine();
                        string[] s = a.Split(" ");
                        Console.Write("\nEnter car id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + s[0] + " " + s[1] + " " + s[2] + " " + s[3] + " " + id;
                    }
                    else if(answer == "delete")
                    {
                        Console.Write("\nEnter car id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + id;
                    }
                    else
                    {
                        Console.Write("\nHow many cars? ");
                        string num = Console.ReadLine();
                        return answer + " " + entity + " " + num;
                    }
                }
                else if(entity == "carowner")
                {
                    if(answer == "insert")
                    {
                        Console.Write("\nEnter fullname: ");
                        string fullname = Console.ReadLine();
                        return answer + " " + entity + " " + fullname;
                    }
                    else if(answer == "update")
                    {
                        Console.Write("\nEnter fullname: ");
                        string fullname = Console.ReadLine();
                        Console.Write("\nEnter car owner id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + fullname + " " + id;
                    }
                    else if(answer == "delete")
                    {
                        Console.Write("\nEnter car owner id: ");
                        string id = Console.ReadLine();
                        return answer + " " + entity + " " + id;
                    }
                    else
                    {
                        Console.Write("\nHow many owners? ");
                        string num = Console.ReadLine();
                        return answer + " " + entity + " " + num;
                    }
                }
                else
                {
                    return "";
                }
            }
            else if(answer == "backup")
            {
                return answer;
            }
            else if(answer == "reservation")
            {
                Console.Write("Enter file name: ");
                string a = Console.ReadLine();
                return answer + " " + a;
            }
            else
            {
                return "";
            }
        }
        else if(answer == "statistics")
        {
            Console.Write("\nEnter command ('speed', 'cars-owner', 'streets-cars'): ");
            answer = Console.ReadLine();
            if(answer == "speed" || answer == "cars-owner" || answer == "streets-cars")
            {
                return "graphic " + answer;
            }
            else
            {
                return "";
            }
        }
        else if(answer == "exit")
        {
            return answer;
        }
        else
        {
            return "";
        }
    }
    public void Exit()
    {
        Console.WriteLine("\n\tBye!");
        return;
    }
    public void EnterException()
    {
        throw new EnterException("Incorrectly entered data!");
    }
    public void GraphicSaved(string file)
    {
        Console.WriteLine($"Statistics successfully conducted. The graph is saved to '{file}'");
    } 
    public void OutputCars(List<string[]> list)
    {
        foreach (var item in list)
        {
            Console.WriteLine($"{item[0]}. {item[1]}\n\towner: {item[2]}\n\tstreet: {item[3]}\n\tspeed: {item[4]} km/h\n\ttime: {item[5]}");
        }
    }
}
class EnterException : ArgumentException
{
    public EnterException(string message)
        : base(message)
    {}
}